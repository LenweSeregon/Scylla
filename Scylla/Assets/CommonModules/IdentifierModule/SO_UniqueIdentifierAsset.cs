namespace Scylla.ObjectManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SO_UniqueIdentifierAsset : ScriptableObject
    {
        [SerializeField] private string _guid;
        [SerializeField] private string _informations;

        public string Guid => _guid;
        public string Informations => _informations;
        
        public void Populate(string uniqueId, string informations)
        {
            _guid = uniqueId;
            _informations = informations;
        }
    }
}

