using Scylla.CommonModules.Identification;

namespace Scylla.PersistenceManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Scylla/Saving/Components/StorablePosition"), DisallowMultipleComponent]
    public class StorablePosition : AStorable
    {
        [Serializable]
        private struct Data
        {
            public Vector3 position;
        }

        [SerializeField] private string toto;

        [SerializeField]
        private Scylla.CommonModules.Identification.Guid _guid;
        
        public override string Save()
        {
            Data data = new Data()
            {
                position = transform.position
            };
            
            return JsonUtility.ToJson(data, true);
        }

        public override void Load(string stringData)
        {
            Data data = JsonUtility.FromJson<Data>(stringData);
            
            transform.position = data.position;
        }
    }
}