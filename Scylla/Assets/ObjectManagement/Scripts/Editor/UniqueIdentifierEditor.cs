using System.Xml;

namespace Scylla.ObjectManagement.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor((typeof(UniqueIdentifier)))]
    public class UniqueIdentifierEditor : Editor
    {
        private readonly string PROPERTY_NAME_UNIQUE_ASSET = "_asset";
        
        private SerializedProperty _propertyUniqueAsset;
        private SO_UniqueIdentifierAsset _previousAsset;
        
        public override void OnInspectorGUI()
        {
            _propertyUniqueAsset = serializedObject.FindProperty(PROPERTY_NAME_UNIQUE_ASSET);
            _previousAsset = _propertyUniqueAsset.objectReferenceValue as SO_UniqueIdentifierAsset;
            
            serializedObject.Update();
            OnInspectorGUIObjectField();
            EditorGUILayout.Space();

            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnInspectorGUIObjectField()
        {
            
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((UniqueIdentifier)target), typeof(UniqueIdentifier), false);
            GUI.enabled = true;

            EditorGUILayout.Space();
            
            EditorGUILayout.PropertyField(_propertyUniqueAsset, new GUIContent("Unique Asset"));

            /*EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.PropertyField(_propertyUniqueAsset, new GUIContent("Unique Asset"));
            }
            if (EditorGUI.EndChangeCheck())
            {
                if (_previousAsset != null)
                {
                    UniqueIdentifierManager.DecreaseReferenceTo(_previousAsset);
                }

                SO_UniqueIdentifierAsset currentAsset = _propertyUniqueAsset.objectReferenceValue as SO_UniqueIdentifierAsset;
                if (currentAsset != null)
                {
                    UniqueIdentifierManager.IncreaseReferenceTo(currentAsset);
                }
            }*/
            
            
            
            /*EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_propertyUniqueAsset, new GUIContent("Unique Asset"));
            GUI.enabled = true;
            
            if (GUILayout.Button("Generate", GUILayout.Width(50)))
            {
                
            }

            if (GUILayout.Button("Picker", GUILayout.Width(50)))
            {
                EditorGUIUtility.ShowObjectPicker<SO_UniqueIdentifierAsset>(null, false, "t:SO_UniqueIdentifierAsset",0);
            }
            EditorGUILayout.EndHorizontal();*/
            
        }
    }
}

