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
        //============ Classes
        //=============================================================================//
        #region Classes
        
        [Serializable]
        public class AStorableReference
        {
            public AStorable _storable;
            public Scylla.CommonModules.Identification.Guid _guid;

            public AStorableReference(AStorable storable, Scylla.CommonModules.Identification.Guid guid)
            {
                _storable = storable;
                _guid = guid;
            }
        }
        
        #endregion
        
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        [SerializeField] private Scylla.CommonModules.Identification.Guid _guid = null;
        [SerializeField, HideInInspector] private List<AStorableReference> _storables = null;
        
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        private UniqueIdentifier _uniqueIdentifier;
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        private void Awake()
        {
            _uniqueIdentifier = GetComponent<UniqueIdentifier>();
        }

        private void Start()
        {
            StorageManager.Instance.RegisterStorable(this);
        }

        private void OnDestroy()
        {
            StorageManager.Instance.UnregisterStorable(this);
        }

        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods

        public void AddStorable(AStorable storable)
        {
            _storables.Add(new AStorableReference(storable, null));
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }

        public void RemoveStorable(AStorable storable)
        {
            _storables.RemoveAll(reference => reference._storable == storable);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }

        public void RefreshStorables()
        {
            RetrieveStorables();
        }

        private void RetrieveStorables()
        {
            if (_storables == null)
                _storables = new List<AStorableReference>();
            
            List<AStorable> storablesFetched = GetComponentsInChildren<AStorable>().ToList();
            
            // Remove all storable which were in list but no longer in the fetched list
            _storables.RemoveAll(storable => storablesFetched.Contains(storable._storable) == false);
            
            // Add all storable which are in fetched list but not in list
            foreach (AStorable storable in storablesFetched)
            {
                if (_storables.Find(storableReference => storableReference._storable == storable) == null)
                {
                    _storables.Add(new AStorableReference(storable, null));
                }
            }

            EditorUtility.SetDirty(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            EditorSceneManager.SaveOpenScenes();
            Debug.Log("toto");
        }
        
        public void LoadRequest(Storage storage)
        {
            Debug.Log(storage.GetData(_uniqueIdentifier.Guid));
        }

        public void SaveRequest(Storage storage)
        {
            storage.UpdateData(_uniqueIdentifier.Guid, "Toto", "Testing the information field");
        }
        
        #endregion
    }
}
