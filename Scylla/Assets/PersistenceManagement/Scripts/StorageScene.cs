namespace Scylla.PersistenceManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class StorageScene : MonoBehaviour
    {
        [SerializeField] private Scylla.CommonModules.Identification.Guid _guid;

        public string Guid => _guid.GetGuid;
    }
}