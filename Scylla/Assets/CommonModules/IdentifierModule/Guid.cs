namespace Scylla.CommonModules.Identification
{
    using Scylla.ObjectManagement;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum GuidType
    {
        Shareable,
        NonShareable
    }
    
    [Serializable]
    public class Guid
    {
#if UNITY_EDITOR
        [SerializeField] private string _guidName = null;
#endif
        [SerializeField] private SO_UniqueIdentifierAsset _assetShareable = null;
        
        public Guid(SO_UniqueIdentifierAsset asset)
        {
            _assetShareable = asset;
        }
        
        public string GetGuid => _assetShareable == null ? null : _assetShareable.Guid;
        public string GetInformation => _assetShareable == null ? null : _assetShareable.Informations;
        
#if UNITY_EDITOR
        public void SetGuidName(string name)
        {
            _guidName = name;
        }        
#endif
        
    }
}