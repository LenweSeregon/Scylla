namespace Scylla.ObjectManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    
    public class SO_UniqueIdentifierDatabase : ScriptableObject, ISerializationCallbackReceiver
    {    
        [Serializable]
        private class SO_UniqueIdentifierDatabaseInformation
        {
            public SO_UniqueIdentifierAsset _guidAsset;
            public int _references;
        }

        [SerializeField]
        private List<SO_UniqueIdentifierDatabaseInformation> _serialized = new List<SO_UniqueIdentifierDatabaseInformation>();

        private Dictionary<string, SO_UniqueIdentifierDatabaseInformation> _unserialized = new Dictionary<string, SO_UniqueIdentifierDatabaseInformation>();

        public SO_UniqueIdentifierAsset GenerateUniqueAsset(string information, bool shareable)
        {
            Guid guid = Guid.NewGuid();
            string guidString = guid.ToString();
            SO_UniqueIdentifierAsset uniqueAsset = ScriptableObject.CreateInstance<SO_UniqueIdentifierAsset>();
            uniqueAsset.Populate(guidString, "", shareable);
            if (AssetDatabase.IsValidFolder("Assets/UniqueIdentifierAssets") == false)
            {
                AssetDatabase.CreateFolder("Assets", "UniqueIdentifierAssets");
            }

            if (AssetDatabase.IsValidFolder("Assets/UniqueIdentifierAssets/IdentifierAssets") == false)
            {
                AssetDatabase.CreateFolder("Assets/UniqueIdentifierAssets", "IdentifierAssets");
            }

            AssetDatabase.CreateAsset(uniqueAsset,
                "Assets/UniqueIdentifierAssets/IdentifierAssets/" + guidString + ".asset");
            AssetDatabase.SaveAssets();

            _unserialized.Add(guidString, new SO_UniqueIdentifierDatabaseInformation(){_guidAsset = uniqueAsset, _references = 1});

            EditorUtility.SetDirty(this);
            
            return uniqueAsset;
        }

        public bool IsUnique(string guid)
        {
            return _unserialized.ContainsKey(guid);
        }

        public void IncreaseReferenceTo(SO_UniqueIdentifierAsset asset)
        {
            if (_unserialized.TryGetValue(asset.Guid, out var assetInformation))
            {
                assetInformation._references++;
            }
            
            AssetDatabase.SaveAssets();
        }

        public void DecreaseReferenceTo(SO_UniqueIdentifierAsset asset)
        {
            if(_unserialized.TryGetValue(asset.Guid, out var assetInformation))
            {
                assetInformation._references--;
                if (assetInformation._references == 0)
                {
                    _unserialized.Remove(asset.Guid);
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(assetInformation._guidAsset));
                    AssetDatabase.SaveAssets();
                }
            }

            EditorUtility.SetDirty(this);
        }

        public void OnBeforeSerialize()
        {
            _serialized = new List<SO_UniqueIdentifierDatabaseInformation>();
            foreach (var entry in _unserialized)
            {
                _serialized.Add(new SO_UniqueIdentifierDatabaseInformation(){_guidAsset = entry.Value._guidAsset, _references = entry.Value._references});
            }
        }

        public void OnAfterDeserialize()
        {
            _unserialized = new Dictionary<string, SO_UniqueIdentifierDatabaseInformation>();
            foreach (SO_UniqueIdentifierDatabaseInformation information in _serialized)
            {
                _unserialized.Add(information._guidAsset.Guid, information);
            }
        }
    }
}