namespace Scylla.PersistenceManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Scylla/Saving/Components/Save Transform"), DisallowMultipleComponent]
    public class StorableTransform : AStorable
    {
        [Serializable]
        private struct Data
        {
            public Vector3 position;
            public Vector3 rotation;
            public Vector3 scale;
        }
        
        public override string Save()
        {
            Transform target = transform;
            
            Data data = new Data()
            {
                position = target.position, 
                rotation = target.rotation.eulerAngles, 
                scale = target.localScale
            };
            
            return JsonUtility.ToJson(data);
        }

        public override void Load(string stringData)
        {
            Data data = JsonUtility.FromJson<Data>(stringData);
            
            transform.position = data.position;
            transform.rotation = Quaternion.Euler(data.rotation);
            transform.localScale = data.scale;
        }
    }
}