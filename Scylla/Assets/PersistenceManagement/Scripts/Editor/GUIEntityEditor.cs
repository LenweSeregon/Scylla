namespace Scylla.ObjectManagement.GUID.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(GUIDEntity))]
    public class GUIEntityEditor : Editor
    {
     
        private readonly string PROPERTY_NAME_SERIALIZED_GUID = "_serializedGuid";
        
        private SerializedProperty _propertySerializedGUID;
        private GUIDEntity _entityTarget;
        
        public override void OnInspectorGUI()
        {
            _entityTarget = target as GUIDEntity;
            _propertySerializedGUID = serializedObject.FindProperty(PROPERTY_NAME_SERIALIZED_GUID);
            
            serializedObject.Update();
            OnInspectorGUIObjectField();
            EditorGUILayout.Space();

            
            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnInspectorGUIObjectField()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((GUIDEntity)target), typeof(GUIDEntity), false);
            GUI.enabled = true;

            EditorGUILayout.Space();

            GUI.enabled = false;
            EditorGUILayout.TextField(new GUIContent("GUID"),_entityTarget.GetGuid().ToString());
            GUI.enabled = true;
        }
    }
}