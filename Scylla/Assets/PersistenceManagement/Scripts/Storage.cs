namespace Scylla.PersistenceManagement
{
    using Scylla.CommonModules.IOModule;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    public class Storage
    {
        [Serializable]
        public struct MetadataElement
        {
            public string key;
            public string value;

            public MetadataElement(string key, string value)
            {
                this.key = key;
                this.value = value;
            }
        }
        
        [Serializable]
        public struct DataElement
        {
            public string guid;
            public string data;
            public string information;

            public DataElement(string guid, string data, string information)
            {
                this.guid = guid;
                this.data = data;
                this.information = information;
            }
        }

        public struct DataGlobalElement
        {
            public string key;
            public string value;

            public DataGlobalElement(string key, string value)
            {
                this.key = key;
                this.value = value;
            }
        }
        
        [Serializable]
        public class MetadataContainer
        {
            public List<MetadataElement> metadata;

            public MetadataContainer()
            {
                metadata = new List<MetadataElement>();
            }
        }

        [Serializable]
        public class DataContainer
        {
            public List<DataElement> data;
            public List<DataGlobalElement> globalData;

            public DataContainer()
            {
                data = new List<DataElement>();
                globalData = new List<DataGlobalElement>();
            }
        }

        [SerializeField] private MetadataContainer _metadata;
        [SerializeField] private DataContainer _data;
        
        private Dictionary<string, int> _cacheData;
        private Dictionary<string, int> _cacheMetadata;
        private Dictionary<string, int> _cacheGlobalData;
        
        private string _saveName;
        private string _pathToFolder;
        private string _pathToMetadata;
        private string _pathToSaveFile;

        
        public string SaveName => _saveName;
        public string PathToMetadata => _pathToMetadata;
        public string PathToSaveFile => _pathToSaveFile;

        public Storage(string saveName, string pathToFolder, string pathToSaveFile, string pathToMetadata)
        {
            _cacheData = new Dictionary<string, int>();
            _cacheMetadata = new Dictionary<string, int>();
            _cacheGlobalData = new Dictionary<string, int>();
            _saveName = saveName;
            _pathToFolder = pathToFolder;
            _pathToMetadata = pathToMetadata;
            _pathToSaveFile = pathToSaveFile;

            _metadata = new MetadataContainer();
            _data = new DataContainer();
            
        }

        public Storage(string saveName, string pathToFolder, string pathToSaveFile)
        {
            _cacheData = new Dictionary<string, int>();
            _cacheMetadata = new Dictionary<string, int>();
            _cacheGlobalData = new Dictionary<string, int>();
            _saveName = saveName;
            _pathToFolder = pathToFolder;
            _pathToSaveFile = pathToSaveFile;
            _pathToMetadata = null;
            
            _metadata = new MetadataContainer();
            _data = new DataContainer();
        }

        public void Delete()
        {
            FileUtilityFactory.GetFileUtility().DeleteFolder(_pathToFolder, true);
        }

        public void Load()
        {
            LoadData();
            LoadMetadata();
        }

        public void Save()
        {
            SaveData();
            SaveMetadata();
        }
        
        public void LoadData()
        {
            try
            {
                ReadOperation<string> operation = IOFactory.GetReadTextOperation(_pathToSaveFile);
                operation.DoOperationSync();
                string result = operation.GetResult();
                _data = JsonUtility.FromJson<DataContainer>(result);

                for (int i = 0; i < _data.data.Count; i++)
                {
                    DataElement element = _data.data[i];
                    _cacheData.Add(element.guid, i);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error at LoadData : " + e.Message);
                throw;
            }
        }

        public void SaveData()
        {
            try
            {
                string content = JsonUtility.ToJson(_data, true);
                WriteOperation<string> operation = IOFactory.GetWriteTextOperation(_pathToSaveFile, content);
                operation.DoOperationSync();
            }
            catch (Exception e)
            {
                Debug.Log("Error at SaveData : " + e.Message);
                throw;
            }
        }
        
        public void LoadMetadata()
        {
            try
            {
                ReadOperation<string> operation = IOFactory.GetReadTextOperation(_pathToMetadata);
                operation.DoOperationSync();
                string result = operation.GetResult();
                _metadata = JsonUtility.FromJson<MetadataContainer>(result);

                for (int i = 0; i < _metadata.metadata.Count; i++)
                {
                    MetadataElement element = _metadata.metadata[i];
                    _cacheMetadata.Add(element.key, i);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error at LoadMetadata : " + e.Message);
                throw;
            }
        }

        public void SaveMetadata()
        {
            try
            {
                string content = JsonUtility.ToJson(_metadata, true);
                WriteOperation<string> operation = IOFactory.GetWriteTextOperation(_pathToMetadata, content);
                operation.DoOperationSync();
            }
            catch (Exception e)
            {
                Debug.Log("Error at SaveMetadata :" + e.Message);
                throw;
            }
        }

        public void UpdateGlobalData(string key, string value)
        {
            if (_cacheGlobalData.TryGetValue(key, out var index))
            {
                DataGlobalElement globalData = new DataGlobalElement(key, value);
                _data.globalData[index] = globalData;
            }
            else
            {
                DataGlobalElement globalData = new DataGlobalElement(key, value);
                _data.globalData.Add(globalData);
                _cacheGlobalData.Add(key, _data.globalData.Count - 1);
            }
        }

        public void UpdateData(string guid, string data, string information)
        {
            if (_cacheData.TryGetValue(guid, out var index))
            {
                DataElement newData = new DataElement(guid, data, information);
                _data.data[index] = newData;
            }
            else
            {
                DataElement newData = new DataElement(guid, data, information);
                _data.data.Add(newData);
                _cacheData.Add(guid, _data.data.Count - 1);
            }
        }

        public void UpdateMetadata(string key, string value)
        {
            if (_cacheMetadata.TryGetValue(key, out var index))
            {
                MetadataElement metadata = new MetadataElement(key, value);
                _metadata.metadata[index] = metadata;
            }
            else
            {
                MetadataElement metadata = new MetadataElement(key, value);
                _metadata.metadata.Add(metadata);
                _cacheMetadata.Add(key, _metadata.metadata.Count - 1);
            }
        }

        public string GetData(string key)
        {
            return (_cacheData.ContainsKey(key)) ? (_data.data[_cacheData[key]].data) : (null);
        }
        
        public string GetMetadata(string key)
        {
            return (_cacheMetadata.ContainsKey(key)) ? (_metadata.metadata[_cacheMetadata[key]].value) : (null);
        }
    }
}

