using Scylla.ObjectManagement;
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
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            float widthButton = 80f;
            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);
            
            EditorGUI.BeginChangeCheck();
            
            // Store old indent level and set it to 0, the PrefixLabel takes care of it
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Get properties
            SerializedProperty asset = property.FindPropertyRelative("_assetShareable");

            string contentButton = asset.objectReferenceValue == null ? "Generate" : "Delete";
            
            Rect fieldRect = new Rect(position.x, position.y, position.width - (widthButton+5), position.height);
            position.x += position.width - widthButton;
            Rect buttonRect = new Rect(position.x, position.y, widthButton, position.height);
            
            
            EditorGUI.PropertyField(fieldRect, asset, GUIContent.none);
            if (GUI.Button(buttonRect, contentButton))
            {
                if (asset.objectReferenceValue == null)
                {
                    asset.objectReferenceValue = UniqueIdentifierManager.GenerateUniqueIdentifier();
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