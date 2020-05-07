namespace Scylla.PersistenceManagement.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(StorageManager))]
    public class StorageManagerEditor : Editor
    {
        #region Constantes
        private readonly string PROPERTY_NAME_SAVEGAME_VERSION = "_persistenceSavegameVersion";
        private readonly string PROPERTY_NAME_OPTION_VERSION = "_persistenceOptionVersion";
        private readonly string PROPERTY_NAME_CONFIGURATION = "_configuration";
        #endregion
        
        #region Internal Fields
        private SerializedProperty _propertySavegameVersion;
        private SerializedProperty _propertyOptionVersion;
        private SerializedProperty _propertyConfiguration;
        #endregion
        
        public override void OnInspectorGUI()
        {
            _propertySavegameVersion = serializedObject.FindProperty(PROPERTY_NAME_SAVEGAME_VERSION);
            _propertyOptionVersion = serializedObject.FindProperty(PROPERTY_NAME_OPTION_VERSION);
            _propertyConfiguration = serializedObject.FindProperty(PROPERTY_NAME_CONFIGURATION);
            
            serializedObject.Update();
            OnInspectorGUIObjectField();
            EditorGUILayout.Space();

            // SAVEGAME VERSION
            EditorGUILayout.BeginHorizontal(GUILayout.Height(40));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_propertySavegameVersion, new GUIContent("Savegame version"), GUILayout.ExpandHeight(true));
            GUI.enabled = true;
            if (GUILayout.Button("Upgrade\nmajor", GUILayout.Width(60), GUILayout.ExpandHeight(true)))
            {
                string[] split = _propertySavegameVersion.stringValue.Split('.');
                int majorVersionAsInt = int.Parse(split[0]);
                string newVersion = (majorVersionAsInt + 1) + "." + 0;
                _propertySavegameVersion.stringValue = newVersion;
            }
            if (GUILayout.Button("Upgrade\nminor", GUILayout.Width(60), GUILayout.ExpandHeight(true)))
            {
                string[] split = _propertySavegameVersion.stringValue.Split('.');
                int majorVersionAsInt = int.Parse(split[0]);
                int minorVersionAsInt = int.Parse(split[1]);
                string newVersion =  majorVersionAsInt + "." + (minorVersionAsInt + 1);
                _propertySavegameVersion.stringValue = newVersion;
            }
            if (GUILayout.Button("Reset", GUILayout.Width(60), GUILayout.ExpandHeight(true)))
            {
                _propertySavegameVersion.stringValue = "1.0";
            }
            EditorGUILayout.EndHorizontal();
            
            // OPTION VERSION
            EditorGUILayout.BeginHorizontal(GUILayout.Height(40));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(_propertyOptionVersion, new GUIContent("Option version"), GUILayout.ExpandHeight(true));
            GUI.enabled = true;
            if (GUILayout.Button("Upgrade\nmajor", GUILayout.Width(60), GUILayout.ExpandHeight(true)))
            {
                string[] split = _propertyOptionVersion.stringValue.Split('.');
                int majorVersionAsInt = int.Parse(split[0]);
                string newVersion = (majorVersionAsInt + 1) + "." + 0;
                _propertyOptionVersion.stringValue = newVersion;
            }
            if (GUILayout.Button("Upgrade\nminor", GUILayout.Width(60), GUILayout.ExpandHeight(true)))
            {
                string[] split = _propertyOptionVersion.stringValue.Split('.');
                int majorVersionAsInt = int.Parse(split[0]);
                int minorVersionAsInt = int.Parse(split[1]);
                string newVersion =  majorVersionAsInt + "." + (minorVersionAsInt + 1);
                _propertyOptionVersion.stringValue = newVersion;
            }
            if (GUILayout.Button("Reset", GUILayout.Width(60), GUILayout.ExpandHeight(true)))
            {
                _propertyOptionVersion.stringValue = "1.0";
            }
            EditorGUILayout.EndHorizontal();
            
            // CONFIGURATION
            EditorGUILayout.PropertyField(_propertyConfiguration, new GUIContent("Configuration"));
            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnInspectorGUIObjectField()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((StorageManager)target), typeof(StorageManager), false);
            GUI.enabled = true;
        }
    }
}

