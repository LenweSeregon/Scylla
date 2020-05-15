using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Scylla.PersistenceManagement
{
    using System;
    using Scylla.ObjectManagement;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    [DisallowMultipleComponent]
    public class Storable : MonoBehaviour
    {
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        [SerializeField, HideInInspector] private List<AStorable> _storables = null;

        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        private void Start()
        {
            StorageManager.Instance.RegisterStorable(this);
        }

        private void OnDestroy()
        {
            #if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    HierarchyInspector.Remove(this);
                }
            #endif

            if(StorageManager.Instance != null)
                StorageManager.Instance.UnregisterStorable(this);
        }

        private void OnValidate()
        {
            HierarchyInspector.Add(this);
            RetrieveStorables();
        }

        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods
        
        public void RefreshStorables()
        {
            RetrieveStorables();
        }

        private void RetrieveStorables()
        {
            if (_storables == null)
                _storables = new List<AStorable>();
            
            List<AStorable> storablesFetched = GetComponentsInChildren<AStorable>().ToList();
            
            // Remove all storable which were in list but no longer in the fetched list
            _storables.RemoveAll(storable => storablesFetched.Contains(storable) == false);
            
            // Add all storable which are in fetched list but not in list
            foreach (AStorable storable in storablesFetched)
            {
                if (_storables.Find(storableReference => storableReference == storable) == null)
                {
                    _storables.Add(storable);
                }
            }
        }
        
        public void LoadRequest(Storage storage)
        {
            foreach (AStorable storable in _storables)
            {
                string data = storage.GetData(storable.Guid);
                storable.Load(data);
            }
        }

        public void SaveRequest(Storage storage)
        {
            foreach (AStorable storable in _storables)
            {
                storage.UpdateData(storable.Guid, storable.Save(), storable.Information);
            }
        }
        
        #endregion
    }
}
