using Scylla.CommonModules.Identification;

namespace Scylla.PersistenceManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Scylla/Saving/Components/StorablePosition"), DisallowMultipleComponent]
    public class StorablePosition : MonoBehaviour, IStorable
    {
        [Serializable]
        private struct Data
        {
            public Vector3 position;
        }

        public string Save()
        {
            Data data = new Data()
            {
                position = transform.position
            };
            
            return JsonUtility.ToJson(data);
        }

        public void Load(string stringData)
        {
            Data data = JsonUtility.FromJson<Data>(stringData);
            
            transform.position = data.position;
        }
    }
}