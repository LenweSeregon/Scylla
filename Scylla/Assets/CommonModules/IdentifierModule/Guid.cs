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
        [SerializeField] protected SO_UniqueIdentifierAsset _assetShareable;

        public string GetGuid => _assetShareable.Guid;
    }
}