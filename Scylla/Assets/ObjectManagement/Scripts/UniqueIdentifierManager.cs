namespace Scylla.ObjectManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class UniqueIdentifierManager
    {
        private static Dictionary<string, SO_UniqueIdentifierAsset> _cache;
        private static SO_UniqueIdentifierDatabase _database;
        private static UniqueIdentifierManager _instance;
        

        public static UniqueIdentifierManager GetInstance()
        {
            if (_instance != null && _instance.DatabaseIsValid() == false)
            {
                _instance = null;
            }

            return _instance ?? (_instance = new UniqueIdentifierManager());
        }

        private bool DatabaseIsValid()
        {
            string[] guids = AssetDatabase.FindAssets("t:SO_UniqueIdentifierDatabase");
            return guids.Length == 1;
        }
        
        private UniqueIdentifierManager()
        {
            string[] guids = AssetDatabase.FindAssets("t:SO_UniqueIdentifierDatabase");
            if (guids.Length > 1)
            {
                Debug.LogError("There is a problem in your project setup, you should not have more than 1 SO_UniqueIdentifierDatabase");
            }
            else if (guids.Length == 0)
            {    
                if (AssetDatabase.IsValidFolder("Assets/UniqueIdentifierAssets") == false)
                {
                    AssetDatabase.CreateFolder("Assets", "UniqueIdentifierAssets");
                    AssetDatabase.SaveAssets();
                }
                
                SO_UniqueIdentifierDatabase assetDatabase = ScriptableObject.CreateInstance<SO_UniqueIdentifierDatabase>();
                AssetDatabase.CreateAsset(assetDatabase,"Assets/UniqueIdentifierAssets/UniqueIdentifierDatabase.asset");
                AssetDatabase.SaveAssets();

                _database = assetDatabase;
            }
            else
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                _database = AssetDatabase.LoadAssetAtPath<SO_UniqueIdentifierDatabase>(path);
            }
        }

        public static bool IsUnique(string guid)
        {
            return GetInstance().InternalIsUnique(guid);
        }

        public static SO_UniqueIdentifierAsset GenerateUniqueIdentifier(string info = null, bool shareable = false)
        {
            return GetInstance().InternalGenerateUniqueIdentifier(info, shareable);
        }

        public static void IncreaseReferenceTo(SO_UniqueIdentifierAsset asset)
        {
            GetInstance().InternalIncreaseReferenceTo(asset);
        }
        
        public static void DecreaseReferenceTo(SO_UniqueIdentifierAsset asset)
        {
            GetInstance().InternalDecreaseReferenceTo(asset);
        }

        private bool InternalIsUnique(string guid)
        {
            return _database.IsUnique(guid);
        }
        
        private void InternalIncreaseReferenceTo(SO_UniqueIdentifierAsset asset)
        {
            _database.IncreaseReferenceTo(asset);
        }
        
        private SO_UniqueIdentifierAsset InternalGenerateUniqueIdentifier(string information, bool shareable)
        {
            return _database.GenerateUniqueAsset(information, shareable);
        }

        private void InternalDecreaseReferenceTo(SO_UniqueIdentifierAsset asset)
        {
            _database.DecreaseReferenceTo(asset);
        }
        
        public static T[] GetAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:"+ typeof(T).Name);  //FindAssets uses tags check documentation for more info
            T[] a = new T[guids.Length];
            for(int i =0;i<guids.Length;i++)         //probably could get optimized 
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
 
            return a;
 
        }
    }
}