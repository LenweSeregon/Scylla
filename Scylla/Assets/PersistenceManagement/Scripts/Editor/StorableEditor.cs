using UnityEditor.SceneManagement;

namespace Scylla.PersistenceManagement.Editor
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Storable))]
	public class StorableEditor : Editor
	{
        private Storable targetComponent;

        private void OnEnable()
        {
            targetComponent = target as Storable;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var serializedObject = new SerializedObject(targetComponent);
            var property = serializedObject.FindProperty("_storables");

            SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 15;
            EditorGUILayout.TextField(string.Format("Component ({0})", arraySizeProp.intValue), EditorStyles.boldLabel);
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.TextField("Save Identifier", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < arraySizeProp.intValue; i++)
            {
                var subProperty = property.GetArrayElementAtIndex(i);
                var guid = subProperty.FindPropertyRelative("_guid");
                var monoBehaviour = subProperty.FindPropertyRelative("_storable");

                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
                EditorGUIUtility.labelWidth = 25;
                EditorGUILayout.PropertyField(monoBehaviour, new GUIContent());
                EditorGUIUtility.labelWidth = 0;
                GUI.enabled = true;

                EditorGUI.BeginChangeCheck();

                //string identifierDrawer = EditorGUILayout.ObjectField(guid);
                EditorGUILayout.PropertyField(guid, GUIContent.none);
                
                if (EditorGUI.EndChangeCheck())
                {
                    //identifier.stringValue = identifierDrawer;
                    serializedObject.ApplyModifiedProperties();

                    EditorUtility.SetDirty(targetComponent);
                    EditorUtility.SetDirty(targetComponent.gameObject);
                    EditorSceneManager.SaveOpenScenes();
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Refresh"))
            {
                targetComponent.RefreshStorables();
            }
        }
    }
}
