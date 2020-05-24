using System.Linq;

namespace Scylla.PersistenceManagement
{
    using System.Globalization;
    using Scylla.CommonModules.IOModule;
    using System.IO;
    using UnityEngine.SceneManagement;

    using System;
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Consider that PersistenceManager will ALWAYS works with Application.persistentDataPath in order to be able to
    /// be platform independent.
    /// </summary>
    public class StorageManager : MonobehaviourSingleton<StorageManager>
    {
        //=============================================================================//
        //============ Events & Delegates
        //=============================================================================//
        #region Events & Delegates
        private event Action<string> _onSaved = null;
        private event Action<string> _onLoaded = null;
        
        public event Action<string> OnSaved
        {
            add
            {
                _onSaved -= value;
                _onSaved += value;
            }
            remove
            {
                _onSaved -= value;
            }
        }
        public event Action<string> OnLoaded
        {
            add
            {
                _onLoaded -= value;
                _onLoaded += value;
            }
            remove
            {
                _onLoaded -= value;
            }
        }
        #endregion
        
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        #region SerializedFields
        [SerializeField] private StorageConfiguration _configuration = null;
        [SerializeField] private string _persistenceSavegameVersion = "1.0";
        [SerializeField] private string _persistenceOptionVersion = "1.0";
        #endregion
        
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields

        private bool _mIsQuitting = false;
        private Dictionary<Scene, StorageScene> _storageScenes = new Dictionary<Scene, StorageScene>();
        private List<Storage> _storages = new List<Storage>();
        private Storage _currentStorage = null;
        
        private List<PersistenceObserver> _storableListeners = new List<PersistenceObserver>();
        private string _currentFilepath;
        #endregion
        
        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties

        public string CurrentFilepath
        {
            get => _currentFilepath;
            set =>
                _currentFilepath = 
                    (value.StartsWith(Application.persistentDataPath)) ? 
                    (value) : 
                    (Path.Combine(Application.persistentDataPath, value));
        }

        public string GlobalSaveFolderPath => Path.Combine(Application.persistentDataPath, _configuration.SaveFolderName);

        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//.
        #region Lifecycle Methods

        protected override void Awake()
        {
            base.Awake();

            _storableListeners = new List<PersistenceObserver>();
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            _currentFilepath = null;
            _currentStorage = null;
            
            LoadMostRecent();
            SyncLoad();
        }

        private void Start()
        {
            Storage mostRecent = FindMostRecent();
            if (mostRecent == null)
            {
                string saveFilename = "testing";

                Storage newStorage = CreateStorage(saveFilename);
                _currentStorage = newStorage;
                _currentStorage.Save();
            }
            else
            {
                _currentStorage = mostRecent;
            }

            LoadMostRecent();
            SyncLoad();
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }
        
        private void OnApplicationQuit()
        {
            _mIsQuitting = true;
            SyncSave();
        }

        #endregion
        
        //=============================================================================//
        //============ Private / Protected Methods
        //=============================================================================//
        #region Private / Protected Methods

        private void RefreshStorages()
        {
            _storages = new List<Storage>();
            
            FileUtility utility = FileUtilityFactory.GetFileUtility();
            
            List<string> foldersSavePath = utility.GetAllFoldersInFolder(GlobalSaveFolderPath);
            
            foreach (string folderSave in foldersSavePath)
            {
                List<string> saveFilesPaths = utility.GetAllFilesInFolder(folderSave, "*" + _configuration.SaveFileExtension);
                if (saveFilesPaths.Count != 1)
                {
                    Debug.LogError("StorageManager == RefreshStorage -- Folder exists but there is no save in it : " + folderSave);
                    continue;
                }

                string saveFilePath = saveFilesPaths[0];
                string saveFilename = Path.GetFileName(saveFilePath);
                string saveFilenameWithoutExtension = Path.GetFileNameWithoutExtension(saveFilename);
                string saveFilenameAsMeta = saveFilenameWithoutExtension + _configuration.SaveFileMetaExtension;
                string saveFilePathAsMeta = Path.Combine(folderSave, saveFilenameAsMeta);

                _storages.Add(
                    (utility.FileExists(saveFilePathAsMeta)) ? 
                    (new Storage(saveFilenameWithoutExtension, folderSave, saveFilePath, saveFilePathAsMeta)) : 
                    (new Storage(saveFilenameWithoutExtension, folderSave, saveFilePath)));
            }
        }

        private bool IsCurrentFilepathValid()
        {
            return string.IsNullOrEmpty(_currentFilepath) == false;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            LoadMostRecent();
            SyncLoad();
        }

        private void OnSceneUnloaded(Scene scene)
        {
            SyncSave();
        }

        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods

        public Storage FindMostRecent()
        {
            RefreshStorages();

            if (_storages.Count == 0)
            {
                Debug.Log("There is no save at " + GlobalSaveFolderPath);
                return null;
            }
            
            if(_storages.Count == 1)
            {
                return _storages[0];
            }
            
            if (_configuration.GenerateMetaFile)
            {
                Storage mostRecent = null;
                DateTime mostRecentLastUpdatedDate = default;
                
                foreach (Storage storage in _storages)
                {
                    // If storage PathToMetadata is null, there is an issue because system should have generated metadata file
                    // And so PathToMetadata should not be null.
                    // Ignore this storage if it is the case
                    if (string.IsNullOrEmpty(storage.PathToMetadata))
                    {
                        Debug.LogError("Configuration is set to generate meta file, but metafile is missing for : " + storage.SaveName);
                        continue;
                    }
                    
                    // We need to load metadata file associated with the storage. We firstly load the metadata file
                    // Then we try to get the metadata corresponding to the date when the game has been last updated
                    // If this metadata doesn't exits, we ignore this storage.
                    storage.LoadMetadata();
                    string storageLastUpdatedString = storage.GetMetadata(StorageConstants.METADATA_LAST_UPDATE);
                    if (storageLastUpdatedString == null)
                    {
                        Debug.LogError("Metadata for last updated date has not been specified");
                        continue;
                    }
                    DateTime storageLastUpdatedDate = DateTime.Parse(storageLastUpdatedString, CultureInfo.InvariantCulture);
                    
                    if (mostRecent == null)
                    {
                        mostRecent = storage;
                        mostRecentLastUpdatedDate = storageLastUpdatedDate;
                    }
                    else
                    {
                        if (storageLastUpdatedDate >= mostRecentLastUpdatedDate)
                        {
                            mostRecent = storage;
                            mostRecentLastUpdatedDate = storageLastUpdatedDate;
                        }
                    }
                }
                
                return mostRecent;
            }
            else
            {
                Debug.LogError("StorageManager - LoadMostRecent == Cannot call LoadMostRecent if you are not generating metafile or having a single save.\n" +
                    "You should use LoadAt(string) if you want to manage yourself");
                return null;
            }
        }

        public bool DestroyedExplicitly(GameObject gameObject)
        {
            return gameObject.scene.isLoaded && _mIsQuitting == false;
        }

        public void SetCurrentStorage(Storage storage)
        {
            _currentStorage = storage;
        }

        public void LoadMostRecent()
        {
            SetCurrentStorage(FindMostRecent());
        }


        public Storage CreateStorage(string saveName)
        {
            if (_storages.Count >= _configuration.MaxSlot)
            {
                Debug.LogError("No more space for a new storage");
                return null;
            }
            
            if (FileUtilityFactory.GetFileUtility().FolderExists(GlobalSaveFolderPath) == false)
            {
                FileUtilityFactory.GetFileUtility().CreateFolder(GlobalSaveFolderPath);
            }

            string saveFolderPath = Path.Combine(GlobalSaveFolderPath, saveName);

            if (FileUtilityFactory.GetFileUtility().FolderExists(saveFolderPath) == false)
            {
                FileUtilityFactory.GetFileUtility().CreateFolder(saveFolderPath);
            }
            
            string filePath = Path.Combine(saveFolderPath, saveName + _configuration.SaveFileExtension);
            string metadataPath = Path.Combine(saveFolderPath, saveName + _configuration.SaveFileMetaExtension);
            DateTime now = DateTime.UtcNow;
            
            Storage newStorage =
                (_configuration.GenerateMetaFile) ? 
                (new Storage(saveName, saveFolderPath, filePath, metadataPath)) : 
                (new Storage(saveName, saveFolderPath, filePath));
            
            newStorage.UpdateMetadata(StorageConstants.METADATA_VERSION, "1.0");
            newStorage.UpdateMetadata(StorageConstants.METADATA_CREATION, now.ToString("G", CultureInfo.InvariantCulture));
            newStorage.UpdateMetadata(StorageConstants.METADATA_TIMESPAN, "0");
            newStorage.UpdateMetadata(StorageConstants.METADATA_LAST_UPDATE, now.ToString("G", CultureInfo.InvariantCulture));

            _storages.Add(newStorage);
            return newStorage;
        }
        
        public void RegisterStorable(PersistenceObserver storable)
        {
            if (storable != null && _currentStorage != null)
            {
                storable.LoadRequest(_currentStorage);
            }
            _storableListeners.Add(storable);
            
            if (storable is StorageScene scene)
                _storageScenes.Add(scene.gameObject.scene, scene);
        }

        public void UnregisterStorable(PersistenceObserver storable)
        {
            if (storable is StorageScene scene)
                _storageScenes.Remove(scene.gameObject.scene);
            
            if (_storableListeners.Remove(storable))
            {
                if (storable != null && _currentStorage != null)
                {
                    storable.SaveRequest(_currentStorage);
                }
            }
        }

        public void SpawnRuntimeInstance(Scene scene, RuntimeSource source, string path, string guid = null, string information = null)
        {
            if (_storageScenes.ContainsKey(scene) == false)
            {
                Debug.LogError("Trying to spawn an instance on a scene that doesn't has a StorageScene component");
                return;
            }

            _storageScenes[scene].SpawnRuntimeInstance(source, path, guid, information);
        }

        public void WipeStorable(Storable storable)
        {
            if (_currentStorage != null)
            {
                storable.WipeRequest(_currentStorage);
            }
        }
        
        public void SyncLoad()
        {
            if (_currentStorage == null)
            {
                Debug.Log("Warning -- Current save is actually null");
                return;
            }

            _currentStorage.LoadData();
            
            foreach (PersistenceObserver storable in _storableListeners)
            {
                storable.LoadRequest(_currentStorage);
            }

            _onSaved?.Invoke("");
        }

        public void SyncSave()
        {
            if (_currentStorage == null)
            {
                Debug.Log("Warning -- Current save is actually null");
                return;
            }

            foreach (PersistenceObserver storable in _storableListeners)
            {
                storable.SaveRequest(_currentStorage);
            }
            
            _currentStorage.Save();
            
            _onLoaded?.Invoke("");
        }
        
        #endregion
    }  
}