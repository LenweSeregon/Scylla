namespace Scylla.SceneManagement.Editor
{
    using Scylla.Editor;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(SceneLoaderProxy))]
    public class SceneLoaderProxyEditor : Editor
    {
        #region Constantes
        private readonly string PROPERTY_NAME_CANCEL_OLDER = "_requestCancelOlderRequest";
        private readonly string PROPERTY_NAME_FADER = "_fader";
        private readonly string PROPERTY_NAME_SCENE_LOADER = "_sceneLoader";
        #endregion
        
        
        #region Internal Fields

        private SerializedProperty _propertyCancelOlder;
        private SerializedProperty _propertyFader;
        private SerializedProperty _propertySceneLoader;
        private SceneLoaderProxyEditorData _data;
        #endregion
        
        
        private void OnEnable()
        {
            _data = new SceneLoaderProxyEditorData();
        }

        public override void OnInspectorGUI()
        {
            _propertyCancelOlder = serializedObject.FindProperty(PROPERTY_NAME_CANCEL_OLDER);
            _propertyFader = serializedObject.FindProperty(PROPERTY_NAME_FADER);
            _propertySceneLoader = serializedObject.FindProperty(PROPERTY_NAME_SCENE_LOADER);
            
            serializedObject.Update();
            OnInspectorGUIObjectField();
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.Space();
                EditorGUILayoutUtils.LabelField("Scylla Scene Management == SceneLoaderProxy", TextAnchor.MiddleCenter, FontStyle.Bold);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_propertyCancelOlder, new GUIContent("Cancel older requests"));
            EditorGUILayout.PropertyField(_propertyFader, new GUIContent("Fader"));
            EditorGUILayout.PropertyField(_propertySceneLoader, new GUIContent("Scene loader proxy"));
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();
            
            serializedObject.ApplyModifiedProperties();
        }

        private void OnInspectorGUIObjectField()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((SceneLoaderProxy)target), typeof(SceneLoaderProxy), false);
            GUI.enabled = true;
        }
    }
}