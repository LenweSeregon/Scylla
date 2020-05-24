using System;
using Scylla.CommonModules.Identification;
using Scylla.ObjectManagement;
using UnityEngine.SceneManagement;

namespace Scylla.PersistenceManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class RuntimeInstance
    {
        public RuntimeSource source;
        public string path;
        public string information;
        public string guid;

        public RuntimeInstance(RuntimeSource source, string path, string information, string guid)
        {
            this.source = source;
            this.path = path;
            this.information = information;
            this.guid = guid;
        }
    }

    [Serializable]
    public class RuntimePersistence
    {
        public int countHistory;
        public RuntimeInstance[] instances;
    }

    public enum RuntimeSource
    {
        Resource
    }
    
    public class StorageScene : PersistenceObserver
    {
        [SerializeField] protected Scylla.CommonModules.Identification.Guid _guid;

        private Dictionary<StorableRuntimeInstance, RuntimeInstance> _spawnedInstances = new Dictionary<StorableRuntimeInstance, RuntimeInstance>();
        private HashSet<string> _loadedRuntimeInstances = new HashSet<string>();
        private int _countHistory;

        public override void LoadRequest(Storage storage)
        {
            string key = string.Format(StorageConstants.STORAGE_SCENE_PREFIX + StorageConstants.STORAGE_SEPARATOR_GUID + _guid.GetGuid);
            string data = storage.GetData(key);
            if (data != null)
            {
                try
                {
                    RuntimePersistence persistence = JsonUtility.FromJson<RuntimePersistence>(data);
                    if (persistence == null || persistence.instances.Length <= 0)
                        return;

                    _countHistory = persistence.countHistory;

                    for (var i = 0; i < persistence.instances.Length; i++)
                    {
                        RuntimeInstance instance = persistence.instances[i];
                        
                        if (_loadedRuntimeInstances.Contains(instance.guid))
                            continue;

                        RuntimeSource source = instance.source;
                        string path = instance.path;
                        string guid = instance.guid;

                        StorableRuntimeInstance spawnedInstance = SpawnRuntimeInstance(source, path, guid);
                        _spawnedInstances.Add(spawnedInstance, instance);
                        _countHistory++;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("StorageScene == LoadRequest -- Json is not valid as RuntimePersistence : " + data);
                    throw;
                }
            }
        }

        public override void SaveRequest(Storage storage)
        {
            RuntimePersistence runtime = new RuntimePersistence();
            runtime.countHistory = _countHistory;
            runtime.instances = new RuntimeInstance[_spawnedInstances.Count];

            int iterator = 0;
            foreach (RuntimeInstance instance in _spawnedInstances.Values)
            {
                runtime.instances[iterator] = instance;
                iterator++;
            }

            string jsonData = JsonUtility.ToJson(runtime, true);
            storage.UpdateData(StorageConstants.STORAGE_SCENE_PREFIX + StorageConstants.STORAGE_SEPARATOR_GUID + _guid.GetGuid, jsonData, _guid.GetInformation);
        }

        public void DestroyRuntimeInstance(StorableRuntimeInstance instance, Storable storable)
        {
            if (_spawnedInstances.ContainsKey(instance))
            {
                _spawnedInstances.Remove(instance);
                _loadedRuntimeInstances.Remove(storable.Guid.GetGuid);
            }
        }

        public StorableRuntimeInstance SpawnRuntimeInstance(RuntimeSource source, string path, string guid = null, string information = null)
        {
            // Retrieve the source
            GameObject resource = null;

            switch (source)
            {
                case RuntimeSource.Resource:
                    resource = Resources.Load(path) as GameObject;
                    break;
                default:
                    break;
            }

            if (resource == null)
            {
                Debug.LogWarning("Invalid resource path : " + path);
                return null;
            }

            // Instantiate the gameobject and ensure to disable it first, to prevent any component from starting / awakening
            bool resourceState = resource.gameObject.activeSelf;
            resource.gameObject.SetActive(false);

            GameObject instance = GameObject.Instantiate(resource, resource.transform.position, resource.transform.rotation);
            SceneManager.MoveGameObjectToScene(instance.gameObject, this.gameObject.scene);

            resource.gameObject.SetActive(resourceState);

            // Ensure that our runtime prefab has a Storable component, if not create one and attach it
            Storable storable = instance.GetComponent<Storable>();
            if (storable == null)
            {
                storable = instance.AddComponent<Storable>();
                storable.RefreshStorables();
            }
            
            // Add a StorableRuntimeInstance component to the instance
            // This component will ensure to destroy properly the object from the game and from the save
            StorableRuntimeInstance storableRuntimeInstance = instance.AddComponent<StorableRuntimeInstance>();
            storableRuntimeInstance.Init(this, storable);
            
            // If there is no GUID attach to Storable component (which apply to all prefab) we need to assign one
            // We need to reassign them from the persistence
            string identifierGuid = null;
            string identifierInformation = null;
            if(string.IsNullOrEmpty(guid))
            {
                if (string.IsNullOrEmpty(storable.Guid.GetGuid) == false)
                {
                    Debug.LogError("You want to spawn a prefab instance which shouldn't have a GUID but it actually has one.");
                    return null;
                }
                
                identifierGuid = _guid.GetGuid + StorageConstants.STORAGE_SEPARATOR_GUID + _countHistory;
                identifierInformation = _guid.GetInformation + StorageConstants.STORAGE_SEPARATOR_INFORMATION;
                
                _spawnedInstances.Add(storableRuntimeInstance, new RuntimeInstance(source, path, identifierInformation, identifierGuid));
            }
            else
            {
                identifierGuid = guid;
                identifierInformation = information;
            }

            _loadedRuntimeInstances.Add(identifierGuid);
            SO_UniqueIdentifierAsset assetIdentification = ScriptableObject.CreateInstance<SO_UniqueIdentifierAsset>();
            assetIdentification.Populate(identifierGuid, identifierInformation);
            storable.Guid = new Scylla.CommonModules.Identification.Guid(assetIdentification);

            instance.gameObject.SetActive(true);
            return storableRuntimeInstance;
        }
        
    }
}