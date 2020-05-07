using System;
using UnityEditor;

namespace Scylla.PersistenceManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [ExecuteInEditMode]
    public abstract class AStorable : MonoBehaviour
    {
        private void Reset()
        {
            #if UNITY_EDITOR
                
                Storable storable = GetComponent<Storable>();
                if (storable != null)
                {
                    storable.AddStorable(this);
                    EditorUtility.SetDirty(storable);
                } 
                
            #endif
        }

        private void OnDisable()
        {
            #if UNITY_EDITOR
            if (Application.isPlaying == false && Application.isEditor && Time.timeSinceLevelLoad > 0f)
            {
                Storable storable = GetComponent<Storable>();
                if (storable != null)
                {
                    storable.RemoveStorable(this);
                    EditorUtility.SetDirty(storable);
                } 
            }
            #endif
        }

        public abstract string Save();
        public abstract void Load(string stringData);
    }
}