namespace Scylla.PersistenceManagement
{
    using System;
    using Scylla.ObjectManagement;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    [RequireComponent(typeof(UniqueIdentifier))]
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
            public string _guid;

            public AStorableReference(AStorable storable, string guid)
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
        [SerializeField] private List<AStorableReference> _storables = null;
        
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
            _storables.Add(new AStorableReference(storable, Guid.NewGuid().ToString()));
        }

        public void RemoveStorable(AStorable storable)
        {
            _storables.RemoveAll(reference => reference._storable == storable);
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
