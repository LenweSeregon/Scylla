using System.Reflection;
using Scylla.ObjectManagement;
using Scylla.PersistenceManagement;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;

namespace Scylla.CommonModules.Identification
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(Guid))]
    public class GuidDrawer : PropertyDrawer
    {
        private bool IsEditingInPrefabMode(GameObject obj)
        {
            if (EditorUtility.IsPersistent(obj))
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }
            else
            {
                // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
                var mainStage = StageUtility.GetMainStageHandle();
                var currentStage = StageUtility.GetStageHandle(obj);
                if (currentStage != mainStage)
                {
                    var prefabStage = PrefabStageUtility.GetPrefabStage(obj);
                    if (prefabStage != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsAssetOnDisk(GameObject obj)
        {
            return PrefabUtility.IsPartOfPrefabAsset(obj) || IsEditingInPrefabMode(obj);
        }
        
        public SO_UniqueIdentifierAsset GenerateUniqueAsset(string assetName, string information, bool shareable)
        {
            System.Guid guid = System.Guid.NewGuid();
            string guidString = guid.ToString();
            string assetFilename = string.IsNullOrEmpty(assetName) ? guidString : assetName + "Guid";
            
            if (AssetDatabase.IsValidFolder("Assets/UniqueIdentifierAssets") == false)
            {
                AssetDatabase.CreateFolder("Assets", "UniqueIdentifierAssets");
            }

            if (AssetDatabase.IsValidFolder("Assets/UniqueIdentifierAssets/IdentifierAssets") == false)
            {
                AssetDatabase.CreateFolder("Assets/UniqueIdentifierAssets", "IdentifierAssets");
            }
            
            string uniqueAssetFilename = AssetDatabase.GenerateUniqueAssetPath("Assets/UniqueIdentifierAssets/IdentifierAssets/"+assetFilename+".asset");
            SO_UniqueIdentifierAsset uniqueAsset = ScriptableObject.CreateInstance<SO_UniqueIdentifierAsset>();
            uniqueAsset.Populate(guidString, "", shareable);

            AssetDatabase.CreateAsset(uniqueAsset, uniqueAssetFilename);
            AssetDatabase.SaveAssets();
            
            return uniqueAsset;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty asset = property.FindPropertyRelative("_assetShareable");
            
            var obj = property.serializedObject.targetObject;
            var monoBehaviour = obj as MonoBehaviour;

            if (IsAssetOnDisk(monoBehaviour.gameObject))
            {
                EditorGUI.LabelField(position, "Cannot generate guid in prefab");
                asset.objectReferenceValue = null;
                return;
            }
            
            float widthButton = 80f;
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            
            EditorGUI.BeginChangeCheck();
            
            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            string contentButton = asset.objectReferenceValue == null ? "Generate" : "Delete";
            
            Rect fieldRect = new Rect(position.x, position.y, position.width - (widthButton+5), position.height);
            position.x += position.width - widthButton;
            Rect buttonRect = new Rect(position.x, position.y, widthButton, position.height);
            
            
            EditorGUI.PropertyField(fieldRect, asset, GUIContent.none);
            if (GUI.Button(buttonRect, contentButton))
            {
                if (asset.objectReferenceValue == null)
                {
                    string componentName = property.serializedObject.targetObject.GetType().Name;
                    string assetName = monoBehaviour.gameObject.name + componentName;
                    asset.objectReferenceValue = GenerateUniqueAsset(assetName, "", false);
                }
                else
                {
                    asset.objectReferenceValue = null;
                }
            }
            
            if (EditorGUI.EndChangeCheck())
                property.serializedObject.ApplyModifiedProperties();

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
    }
}