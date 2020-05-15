namespace Scylla.PersistenceManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Scylla/Saving/Components/Save Scale"), DisallowMultipleComponent]
    public class StorableScale : AStorable
    {
        [Serializable]
        private struct Data
        {
            public Vector3 scale;
        }
        
        public override string Save()
        {
            Data data = new Data()
            {
                scale = transform.localScale
            };
            
            return JsonUtility.ToJson(data);
        }

        public override void Load(string stringData)
        {
            Data data = JsonUtility.FromJson<Data>(stringData);
            
            transform.localScale = data.scale;
        }
    }
}