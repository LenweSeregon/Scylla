namespace Scylla.ObjectManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SO_UniqueIdentifierAsset : ScriptableObject
    {
        [SerializeField] private string _guid;
        [SerializeField] private string _informations;
        [SerializeField] private bool _shareable;

        public string Guid => _guid;
        public string Informations => _informations;
        public bool Shareable => _shareable;
        
        public void Populate(string uniqueId, string informations, bool shareable)
        {
            _guid = uniqueId;
            _informations = informations;
            _shareable = shareable;
        }
    }
}

