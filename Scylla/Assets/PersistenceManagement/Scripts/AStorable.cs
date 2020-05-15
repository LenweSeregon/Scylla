using System;
using UnityEditor;

namespace Scylla.PersistenceManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class AStorable : MonoBehaviour
    {
        [SerializeField, HideInInspector] private Scylla.CommonModules.Identification.Guid _guid;

        public string Guid => _guid.GetGuid;
        public string Information => _guid.GetInformation;
        
        public abstract string Save();
        public abstract void Load(string stringData);
    }
}