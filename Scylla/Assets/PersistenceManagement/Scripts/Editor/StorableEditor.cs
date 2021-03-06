﻿using UnityEditor.SceneManagement;

namespace Scylla.PersistenceManagement.Editor
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;

	[CustomEditor(typeof(Storable), true)]
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
            
            var property = serializedObject.FindProperty("_storableReferences");
            
            SerializedProperty arraySizeProp = property.FindPropertyRelative("Array.size");

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 15;
            EditorGUILayout.TextField(string.Format("Component ({0})", arraySizeProp.intValue), EditorStyles.boldLabel);
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.TextField("Save Identifier", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            int arraySize = arraySizeProp.intValue;
            for (int i = 0; i < arraySize; i++)
            {
                var subProperty = property.GetArrayElementAtIndex(i);
                var storable = subProperty.FindPropertyRelative("monobehaviourStorable");
                var guid = subProperty.FindPropertyRelative("guid");

                EditorGUILayout.BeginHorizontal();
                GUI.enabled = false;
                EditorGUIUtility.labelWidth = 25;
                EditorGUILayout.PropertyField(storable, new GUIContent());
                EditorGUIUtility.labelWidth = 0;
                GUI.enabled = true;

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.PropertyField(guid, GUIContent.none);
                
                if (EditorGUI.EndChangeCheck())
                {
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
