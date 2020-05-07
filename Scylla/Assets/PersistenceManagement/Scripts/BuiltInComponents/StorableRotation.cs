namespace Scylla.PersistenceManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Scylla/Saving/Components/Save Rotation"), DisallowMultipleComponent]
    public class StorableRotation : AStorable
    {
        [Serializable]
        private struct Data
        {
            public Vector3 rotation;
        }
        
        public override string Save()
        {            
            Data data = new Data()
            {
                rotation = transform.rotation.eulerAngles
            };
            
            return JsonUtility.ToJson(data, true);
        }

        public override void Load(string stringData)
        {            
            Data data = JsonUtility.FromJson<Data>(stringData);
            
            transform.rotation = Quaternion.Euler(data.rotation);
        }
    }
}