namespace Scylla.PersistenceManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class Save
    {
        [Serializable]
        public struct MetaData
        {
            public int gameVersion;
            public string creationDate;
            public string timePlayed;
        }

        [Serializable]
        public struct Data
        {
            public string guid;
            public string data;
            public string scene;
            public string information;
        }

        [SerializeField] private MetaData _metaData;
        [SerializeField] private List<Data> _saveDatas;

        private TimeSpan _timePlayed;
        private int _gameVersion;
        private DateTime _creationDate;
        private Dictionary<string, int> _saveDatasCache;

        public Save()
        {
            _saveDatasCache = new Dictionary<string, int>(StringComparer.Ordinal);
        }
        
        public void OnWriteDone()
        {
            if (_creationDate == null)
            {
                _creationDate = DateTime.Now;
            }

            _metaData.creationDate = _creationDate.ToString();
            _metaData.gameVersion = _gameVersion;
            _metaData.timePlayed = _timePlayed.ToString();
        }

        public void OnReadDone()
        {
            _gameVersion = _metaData.gameVersion;
            DateTime.TryParse(_metaData.creationDate, out _creationDate);
            TimeSpan.TryParse(_metaData.timePlayed, out _timePlayed);

            if (_saveDatas.Count > 0)
            {
                int dataCount = _saveDatas.Count;
                for (int i = dataCount - 1; i >= 0; i--)
                {
                    if (string.IsNullOrEmpty(_saveDatas[i].data))
                    {
                        _saveDatas.RemoveAt(i);
                    }
                }

                for (int i = 0; i < _saveDatas.Count; i++)
                {
                    _saveDatasCache.Add(_saveDatas[i].guid, i);
                }
            }
        }


        public void Remove(string guid)
        {
            if(_saveDatasCache.TryGetValue(guid, out var indexInSaveData))
            {
                _saveDatas[indexInSaveData] = new Data();
                _saveDatasCache.Remove(guid);
            }
        }
        
        public void Set(string guid, string data, string information)
        {
            if (_saveDatasCache.TryGetValue(guid, out var indexInSaveData))
            {
                _saveDatas[indexInSaveData] = new Data() {guid = guid, data = data, information = information};
            }
            else
            {
                Data newData = new Data(){guid = guid, data = data, information = information};
                
                _saveDatas.Add(newData);
                _saveDatasCache.Add(guid, _saveDatas.Count - 1);
            }
        }

        public string Get(string guid)
        {
            return 
                (_saveDatasCache.TryGetValue(guid, out var indexInSaveData)) ? 
                (_saveDatas[indexInSaveData].data) : 
                (null);
        }
    }
}