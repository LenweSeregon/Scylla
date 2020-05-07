namespace Scylla.PersistenceManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    
    [CreateAssetMenu(fileName = "StorageConfiguration", menuName = "Scylla/PersistenceManagement/Configuration")]
    public class StorageConfiguration : ScriptableObject
    {
        [SerializeField] private string _saveFolderName = null;
        [SerializeField] private string _saveFileExtension = ".store";
        [SerializeField] private string _saveFileMetaExtension = ".meta";
        [SerializeField] private bool _generateMetaFile = true;
        [SerializeField] private int _maxSlot = 10;
        
        public string SaveFolderName => _saveFolderName;
        public string SaveFileExtension => _saveFileExtension;
        public string SaveFileMetaExtension => _saveFileMetaExtension;

        public bool GenerateMetaFile => _generateMetaFile;

        public int MaxSlot => _maxSlot;
    }
}