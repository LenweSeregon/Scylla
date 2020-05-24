using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Scylla.PersistenceManagement
{
    using System;
    using Scylla.ObjectManagement;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class IStorableReference
    {
        [AllowDrawingInPrefabMode(true)] public Scylla.CommonModules.Identification.Guid guid;
        public MonoBehaviour monobehaviourStorable;

        public IStorableReference(Scylla.CommonModules.Identification.Guid guid, MonoBehaviour monobehaviourStorable)
        {
            this.guid = guid;
            this.monobehaviourStorable = monobehaviourStorable;
        }
    }

    public class IStorableInstance
    {
        public string guid;
        public string information;
        public IStorable storable;

        public IStorableInstance(string guid, string information, IStorable storable)
        {
            this.guid = guid;
            this.information = information;
            this.storable = storable;
        }
    }
    
    /// <summary>
    /// Because the way Unity store component, we cannot directly have a reference to our interface IStorable in
    /// editor script.
    /// Then, to manage the inspector display and also to access IStorable component in a efficient manner without
    /// casting the Monobehaviour each time we want to iterate on the list, we are working with 2 list.
    /// - _storableReferences which is serialized and store the Monobehaviour component and the guid
    /// - _storableInstances which is not serialized and store the IStorable component and the guid.
    ///
    /// _storableInstances is initialized in the Awake method, with the StorableReferences
    /// </summary>
    [DisallowMultipleComponent]
    public class Storable : PersistenceObserver
    {
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        [SerializeField, AllowDrawingInPrefabMode(false)] private Scylla.CommonModules.Identification.Guid _guid = null;
        [SerializeField, HideInInspector] private List<IStorable> _storables = null;
        [SerializeField, HideInInspector] private List<IStorableReference> _storableReferences = null;
        
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        private List<IStorableInstance> _storableInstances = null;
        
        //=============================================================================//
        //============ Properties
        //=============================================================================//
        public Scylla.CommonModules.Identification.Guid Guid
        {
            set => _guid = value;
            get => _guid;
        }
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        private void Awake()
        {
            _storableInstances = new List<IStorableInstance>();
            foreach(IStorableReference reference in _storableReferences)
            {
                string guid = _guid.GetGuid + StorageConstants.STORAGE_SEPARATOR_GUID + reference.guid.GetGuid;
                string information = _guid.GetInformation + StorageConstants.STORAGE_SEPARATOR_INFORMATION + reference.guid.GetInformation;
                IStorable storable = reference.monobehaviourStorable as IStorable;
                _storableInstances.Add(new IStorableInstance(guid, information, storable));
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            #if UNITY_EDITOR
                if (Application.isPlaying == false)
                {
                    HierarchyInspector.Remove(this);
                }
            #endif
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
            if (_storableReferences == null)
                _storableReferences = new List<IStorableReference>();
            
            List<IStorable> storablesFetched = GetComponentsInChildren<IStorable>().ToList();
            
            // Remove all storable which were in list but no longer in the fetched list
            _storableReferences.RemoveAll(storable => storablesFetched.Contains((IStorable) storable.monobehaviourStorable) == false);
            
            // Add all storable which are in fetched list but not in list
            foreach (IStorable storable in storablesFetched)
            {
                if (_storableReferences.Find(reference => reference.monobehaviourStorable == (MonoBehaviour) storable) == null)
                {
                    MonoBehaviour storableComponent = storable as MonoBehaviour;
                    if (storableComponent == null)
                    {
                        Debug.LogError("Storable is not a Monobehaviour - Should not happen");
                        continue;
                    }
                    
                    CommonModules.Identification.Guid guid = new CommonModules.Identification.Guid(null);
                    guid.SetGuidName(storableComponent.GetType().Name);

                    _storableReferences.Add(new IStorableReference(guid, storableComponent));
                }
            }
        }
        
        public override void LoadRequest(Storage storage)
        {
            foreach (IStorableInstance storableInstance in _storableInstances)
            {
                IStorable storable = storableInstance.storable;
                string guid = storableInstance.guid;
                string data = storage.GetData(guid);
                if(data != null)
                    storable.Load(data);
            }
        }

        public override void SaveRequest(Storage storage)
        {
            foreach (IStorableInstance storableInstance in _storableInstances)
            {
                IStorable storable = storableInstance.storable;
                string guid = storableInstance.guid;
                string info = storableInstance.information;
                storage.UpdateData(guid, storable.Save(), info);
            }
        }

        public void WipeRequest(Storage storage)
        {
            foreach (IStorableInstance storableInstance in _storableInstances)
            {
                string guid = storableInstance.guid;
                storage.RemoveData(guid);
            }

            storage.RemoveData(_guid.GetGuid);
        }
        
        #endregion
    }
}
