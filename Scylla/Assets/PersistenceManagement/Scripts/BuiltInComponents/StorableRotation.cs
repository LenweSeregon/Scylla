namespace Scylla.PersistenceManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("Scylla/Saving/Components/Save Rotation"), DisallowMultipleComponent]
    public class StorableRotation : MonoBehaviour, IStorable
    {
        [Serializable]
        private struct Data
        {
            public Vector3 rotation;
        }
        
        public string Save()
        {            
            Data data = new Data()
            {
                rotation = transform.rotation.eulerAngles
            };
            
            return JsonUtility.ToJson(data);
        }

        public void Load(string stringData)
        {            
            Data data = JsonUtility.FromJson<Data>(stringData);
            
            transform.rotation = Quaternion.Euler(data.rotation);
        }
    }
}