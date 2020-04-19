
namespace Scylla.SceneManagement.Editor
{
    using Scylla.Editor;
    using System;
    using System.Reflection;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [CustomEditor(typeof(SceneManager))]
    public class SceneManagerEditor : Editor
    {
        #region Constantes
        private readonly string PROPERTY_NAME_SCENE_LOADER = "_sceneLoaderProxy";
        #endregion
        
        #region Internal Fields
        private SceneManagerEditorData _mDatas;
        
        private SerializedProperty _propertySceneLoader;
        #endregion
        
        #region Methods

        private void OnEnable()
        {
            _mDatas = new SceneManagerEditorData();
        }

        public override void OnInspectorGUI()
        {
            _propertySceneLoader = serializedObject.FindProperty(PROPERTY_NAME_SCENE_LOADER);
            
            serializedObject.Update();
            OnInspectorGUIObjectField();
            EditorGUILayout.Space();
            OnInspectorGUIAutoGenerateSection();
            serializedObject.ApplyModifiedProperties();
        }

        private bool HasType(string typeName, string namespaceName) {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == typeName && type.Namespace == namespaceName)
                    {
                        Debug.Log(type.FullName);
                        return true;
                    }
                        
                }
            }
     
            return false;
        }

        
        private void OnInspectorGUIObjectField()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((SceneManager)target), typeof(SceneManager), false);
            GUI.enabled = true;
        }
        
        private void OnInspectorGUIAutoGenerateSection()
        {
            // Drawing header Scylla's scene management
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.Space();
                EditorGUILayoutUtils.LabelField("Scylla's scene management", TextAnchor.MiddleCenter, FontStyle.Bold);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_propertySceneLoader, new GUIContent("Scene loader proxy"));
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.Space();
                
                if (string.IsNullOrEmpty(_mDatas.EnumerationFolderGeneration))
                {
                    EditorGUILayout.HelpBox("Auto-generated folder cannot be empty", MessageType.Error);
                }
                
                EditorGUILayout.Space();
                
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                 _mDatas.FoldoutAutoGenerationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_mDatas.FoldoutAutoGenerationSettings, "Auto-generation settings");
                 GUILayout.Space(4);
                 EditorGUILayout.EndHorizontal();
                 EditorGUILayout.Space();
                 
                if (_mDatas.FoldoutAutoGenerationSettings)
                {
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.enabled = false;
                            EditorGUILayout.TextField(new GUIContent("Auto-generated folder"), _mDatas.EnumerationFolderGeneration);
                            GUI.enabled = true;
                            if (GUILayout.Button("Browse", GUILayout.Width(60)))
                            {
                                string path = EditorUtility.OpenFolderPanel("Select auto-generated files's folder", Application.dataPath, _mDatas.EnumerationFolderGeneration);
                                _mDatas.EnumerationFolderGeneration = path;
                                serializedObject.ApplyModifiedProperties();
                                Repaint();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.BeginChangeCheck();
                        _mDatas.EnumerationName = EditorGUILayout.TextField(new GUIContent("Enumeration name"), _mDatas.EnumerationName);
                        _mDatas.EnumerationNamespace = EditorGUILayout.TextField(new GUIContent("Enumeration namespace"), _mDatas.EnumerationNamespace);
                        _mDatas.EnumerationFileName = EditorGUILayout.TextField(new GUIContent("Enumeration file name"), _mDatas.EnumerationFileName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Debug.Log("Bool exists : " + HasType(_mDatas.EnumerationName, _mDatas.EnumerationNamespace));
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                _mDatas.FoldoutAutoGenerationManualModification = EditorGUILayout.BeginFoldoutHeaderGroup(_mDatas.FoldoutAutoGenerationManualModification, "Auto-generation enumeration values");
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                
                if (_mDatas.FoldoutAutoGenerationManualModification)
                {
                    _mDatas.EnumerationValues = new List<string>();
                    for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
                    {
                        EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                        bool sceneEnabled = scene.enabled;
                        
                        EditorGUILayout.BeginHorizontal();
                        {
                            if(sceneEnabled)
                                _mDatas.EnumerationValues.Add(System.IO.Path.GetFileNameWithoutExtension(scene.path));
                            
                            string textIncluded = (sceneEnabled) ? ("Included") : ("Not included");
                            Color color = (sceneEnabled) ? (Color.green) : (Color.red);
                            EditorGUILayout.LabelField("Scene : " + i, System.IO.Path.GetFileNameWithoutExtension(scene.path));
                            EditorGUILayoutUtils.LabelField(textIncluded, color, new GUILayoutOption[] { GUILayout.Width(120) }, TextAnchor.MiddleRight);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndFoldoutHeaderGroup();
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                _mDatas.FoldoutAutoGenerationGenerate = EditorGUILayout.BeginFoldoutHeaderGroup(_mDatas.FoldoutAutoGenerationGenerate, "Auto-generation");
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_mDatas.FoldoutAutoGenerationGenerate)
                {                
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                        if (GUILayout.Button("Generate"))
                        {
                            EnumerationGenerator generator = new EnumerationGenerator(_mDatas.EnumerationFolderGeneration, _mDatas.EnumerationFileName, 
                                _mDatas.EnumerationName, _mDatas.EnumerationNamespace, _mDatas.EnumerationValues.ToArray());
                            generator.GenerateFile();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        
        #endregion
    }
}

