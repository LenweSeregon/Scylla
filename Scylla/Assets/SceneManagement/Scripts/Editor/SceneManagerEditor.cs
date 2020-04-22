using System.IO;
using System.Linq;
using UnityEditor.SceneManagement;

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
        private readonly string PROPERTY_NAME_LOAD_SCENE_AT_START = "_loadSceneAtStart";
        private readonly string PROPERTY_NAME_SCENE_TO_LOAD_AT_START_NAME = "_sceneToLoadAtStartName";
        private readonly string PROPERTY_NAME_SCENE_TO_LOAD_AT_START_IS_MARKED = "_sceneToLoadAtStartIsMarked";
        private readonly string PROPERTY_NAME_SCENE_TO_LOAD_AT_START_IS_SUPPRESSIBLE = "_sceneToLoadAtStartIsSuppressible";
        private readonly string PROPERTY_NAME_SCENE_TO_LOAD_AT_START_BUNDLE_IDENTIFIER = "_sceneToLoadAtStartBundleIdentifier";
        
        
        #endregion
        
        #region Internal Fields
        private SceneManagerEditorData _datas;
        
        private SerializedProperty _propertySceneLoader;
        private SerializedProperty _propertyLoadSceneAtStart;
        private SerializedProperty _propertySceneToLoadAtStartName;
        private SerializedProperty _propertySceneToLoadAtStartIsMarked;
        private SerializedProperty _propertySceneToLoadAtStartIsSuppressible;
        private SerializedProperty _propertySceneToLoadAtStartBundleIdentifier;
        #endregion
        
        #region Methods

        private void OnEnable()
        {
            _datas = new SceneManagerEditorData();
        }

        public override void OnInspectorGUI()
        {
            _propertySceneLoader = serializedObject.FindProperty(PROPERTY_NAME_SCENE_LOADER);
            _propertyLoadSceneAtStart = serializedObject.FindProperty(PROPERTY_NAME_LOAD_SCENE_AT_START);
            _propertySceneToLoadAtStartName = serializedObject.FindProperty(PROPERTY_NAME_SCENE_TO_LOAD_AT_START_NAME);
            _propertySceneToLoadAtStartIsMarked = serializedObject.FindProperty(PROPERTY_NAME_SCENE_TO_LOAD_AT_START_IS_MARKED);
            _propertySceneToLoadAtStartIsSuppressible = serializedObject.FindProperty(PROPERTY_NAME_SCENE_TO_LOAD_AT_START_IS_SUPPRESSIBLE);
            _propertySceneToLoadAtStartBundleIdentifier = serializedObject.FindProperty(PROPERTY_NAME_SCENE_TO_LOAD_AT_START_BUNDLE_IDENTIFIER);
            
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
            bool canAutoGenerateEnumeration = true;
            
            // Drawing header Scylla's scene management
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.Space();
                EditorGUILayoutUtils.LabelField("Scylla Scene Management == SceneManager", TextAnchor.MiddleCenter, FontStyle.Bold);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            // Drawing fields property
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_propertySceneLoader, new GUIContent("Scene loader proxy"));
            EditorGUILayout.PropertyField(_propertyLoadSceneAtStart, new GUIContent("Load scene at start"));
            if (_propertyLoadSceneAtStart.boolValue == true)
            {
                EditorGUILayout.PropertyField(_propertySceneToLoadAtStartName, new GUIContent("Scene name"));
                EditorGUILayout.PropertyField(_propertySceneToLoadAtStartIsMarked, new GUIContent("Scene is marked"));
                EditorGUILayout.PropertyField(_propertySceneToLoadAtStartIsSuppressible, new GUIContent("Scene is suppressible"));
                EditorGUILayout.PropertyField(_propertySceneToLoadAtStartBundleIdentifier, new GUIContent("Scene bundle identifier"));
            }
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayoutUtils.LabelField("Builds settings & Management", TextAnchor.MiddleCenter, FontStyle.Bold);
                
                EditorGUILayout.Space();
                
                EditorGUI.indentLevel++;
                
                // BUILDS SETTINGS VISUALIZER
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                _datas.FoldoutBuildSettingsVisualizer = EditorGUILayout.BeginFoldoutHeaderGroup(_datas.FoldoutBuildSettingsVisualizer, "Builds Settings visualizer");
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_datas.FoldoutBuildSettingsVisualizer)
                {
                    for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
                    {
                        EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                        bool sceneEnabled = scene.enabled;
                        bool sceneDeleted = File.Exists(scene.path) == false;
                    
                        EditorGUILayout.BeginHorizontal();
                        {
                            string textIncluded = "";
                            Color color = Color.white;

                            if (sceneDeleted)
                            {
                                textIncluded = "Deleted";
                                color = Color.black;
                            }
                            else if (sceneEnabled)
                            {
                                textIncluded = "Included";
                                color = Color.green;
                            }
                            else
                            {
                                textIncluded = "Not included";
                                color = Color.red;
                            }

                            EditorGUILayout.LabelField("Scene : " + i, System.IO.Path.GetFileNameWithoutExtension(scene.path));
                            EditorGUILayoutUtils.LabelField(textIncluded, color, new GUILayoutOption[] { GUILayout.Width(120) }, TextAnchor.MiddleRight);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                
                EditorGUILayout.Space();
                
                // BUILDS SETTINGS MANAGEMENT
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                _datas.FoldoutBuildSettingsManagement = EditorGUILayout.BeginFoldoutHeaderGroup(_datas.FoldoutBuildSettingsManagement, "Builds Settings Management");
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_datas.FoldoutBuildSettingsManagement)
                {
                    // Button to delete from build settings deleted scenes
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                        if (GUILayout.Button("Delete deleted's scene in Build Settings"))
                        {
                            EditorBuildSettingsScene[] originalScenes = EditorBuildSettings.scenes;
                            List<EditorBuildSettingsScene> withoutDeleted = new List<EditorBuildSettingsScene>(originalScenes);
                            withoutDeleted.RemoveAll(scene => File.Exists(scene.path) == false);
                            EditorBuildSettings.scenes = withoutDeleted.ToArray();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayoutUtils.LabelField("Enumeration auto-generation section", TextAnchor.MiddleCenter, FontStyle.Bold);
                
                // Auto-generation enum
                EditorGUILayout.Space();
                
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                 _datas.FoldoutAutoGenerationSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_datas.FoldoutAutoGenerationSettings, "Auto-generation settings");
                 GUILayout.Space(4);
                 EditorGUILayout.EndHorizontal();
                 EditorGUILayout.Space();
                 
                if (_datas.FoldoutAutoGenerationSettings)
                {
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUI.enabled = false;
                            EditorGUILayout.TextField(new GUIContent("Auto-generated folder"), _datas.EnumerationFolderGeneration);
                            GUI.enabled = true;
                            if (GUILayout.Button("Browse", GUILayout.Width(60)))
                            {
                                string path = EditorUtility.OpenFolderPanel("Select auto-generated files's folder", Application.dataPath, _datas.EnumerationFolderGeneration);
                                _datas.EnumerationFolderGeneration = path;
                                serializedObject.ApplyModifiedProperties();
                                Repaint();
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUI.BeginChangeCheck();
                        _datas.EnumerationName = EditorGUILayout.TextField(new GUIContent("Enumeration name"), _datas.EnumerationName);
                        _datas.EnumerationNamespace = EditorGUILayout.TextField(new GUIContent("Enumeration namespace"), _datas.EnumerationNamespace);
                        _datas.EnumerationFileName = EditorGUILayout.TextField(new GUIContent("Enumeration file name"), _datas.EnumerationFileName);
                        if (EditorGUI.EndChangeCheck())
                        {
                            Debug.Log("Bool exists : " + HasType(_datas.EnumerationName, _datas.EnumerationNamespace));
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFoldoutHeaderGroup();

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                _datas.FoldoutAutoGenerationGenerate = EditorGUILayout.BeginFoldoutHeaderGroup(_datas.FoldoutAutoGenerationGenerate, "Auto-generation");
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_datas.FoldoutAutoGenerationGenerate)
                {                
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (string.IsNullOrEmpty(_datas.EnumerationFolderGeneration))
                        {
                            EditorGUILayout.HelpBox("Auto-generated folder cannot be empty", MessageType.Error);
                            canAutoGenerateEnumeration = false;
                        }
                        else if (string.IsNullOrEmpty(_datas.EnumerationFileName))
                        {
                            EditorGUILayout.HelpBox("Auto-generated enumeration filename cannot be empty", MessageType.Error);
                            canAutoGenerateEnumeration = false;
                        }
                        else if (string.IsNullOrEmpty(_datas.EnumerationName))
                        {
                            EditorGUILayout.HelpBox("Auto-generated enumeration name cannot be empty", MessageType.Error);
                            canAutoGenerateEnumeration = false;
                        }
                        else
                        {
                            canAutoGenerateEnumeration = true;
                            GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                            if (GUILayout.Button("Generate"))
                            {
                                GenerateEnumeration();
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.Space();
            }
            
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
            
            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayoutUtils.LabelField("Scene auto-generation section", TextAnchor.MiddleCenter, FontStyle.Bold);

                EditorGUILayout.Space();
                
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                _datas.FoldoutAutoGenerationSceneGlobalSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_datas.FoldoutAutoGenerationSceneGlobalSettings, "Auto-generation settings");
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_datas.FoldoutAutoGenerationSceneGlobalSettings)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUI.enabled = false;
                        EditorGUILayout.TextField(new GUIContent("Scene auto-generated folder"), _datas.SceneFolderGeneration);
                        GUI.enabled = true;
                        if (GUILayout.Button("Browse", GUILayout.Width(60)))
                        {
                            string path = EditorUtility.OpenFolderPanel("Select auto-generated files's folder", Application.dataPath, _datas.SceneFolderGeneration);
                            _datas.SceneFolderGeneration = path;
                            serializedObject.ApplyModifiedProperties();
                            Repaint();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    _datas.SceneAddToBuild = EditorGUILayout.Toggle(new GUIContent("Add scene to build settings"), _datas.SceneAddToBuild);
                    if (_datas.SceneAddToBuild)
                    {
                        _datas.RegenerateEnum = EditorGUILayout.Toggle(new GUIContent("Regenerate scenes enumeration"), _datas.RegenerateEnum);
                    }
                    else
                    {
                        _datas.RegenerateEnum = false;
                    }
                }
                
                EditorGUILayout.EndFoldoutHeaderGroup();
                
                EditorGUILayout.Space();
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                _datas.FoldoutAutoGenerationSceneSettings = EditorGUILayout.BeginFoldoutHeaderGroup(_datas.FoldoutAutoGenerationSceneSettings, "Auto-generation scene settings");
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_datas.FoldoutAutoGenerationSceneSettings)
                {
                    _datas.SceneName = EditorGUILayout.TextField(new GUIContent("Scene name"), _datas.SceneName);
                }
                
                EditorGUILayout.EndFoldoutHeaderGroup();
                
                EditorGUILayout.Space();
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                _datas.FoldoutAutoGenerationSceneGenerate = EditorGUILayout.BeginFoldoutHeaderGroup(_datas.FoldoutAutoGenerationSceneGenerate, "Auto-generation settings");
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_datas.FoldoutAutoGenerationSceneGenerate)
                {
                    if (string.IsNullOrEmpty(_datas.SceneFolderGeneration))
                    {
                        EditorGUILayout.HelpBox("Scene auto-generated folder cannot be empty", MessageType.Error);
                    }
                    else if (string.IsNullOrEmpty(_datas.SceneName))
                    {
                        EditorGUILayout.HelpBox("Scene auto-generated name cannot be empty", MessageType.Error);
                    }
                    else if (_datas.RegenerateEnum && canAutoGenerateEnumeration == false)
                    {
                        EditorGUILayout.HelpBox("Can't regenerate enum when auto-generation enumeration has issues", MessageType.Error);
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                            if (GUILayout.Button("Generate"))
                            {
                                string relativeFolderPath = "";

                                if (_datas.SceneFolderGeneration.StartsWith(Application.dataPath)) {
                                    relativeFolderPath=  "Assets" + _datas.SceneFolderGeneration.Substring(Application.dataPath.Length);
                                }

                                UnityEngine.SceneManagement.Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
                                scene.name = _datas.SceneName;
                                string relativePath = Path.Combine(relativeFolderPath,scene.name + ".unity");
                                GameObject scyllaSceneGO = new GameObject("ScyllaScene");
                                scyllaSceneGO.AddComponent(typeof(ScyllaScene));
                                EditorSceneManager.SaveScene(scene, relativePath);
                                EditorSceneManager.CloseScene(scene, true);

                                if (_datas.SceneAddToBuild)
                                {
                                    EditorBuildSettingsScene[] originalScenes = EditorBuildSettings.scenes;
                                    EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[originalScenes.Length + 1];
                                    Array.Copy(originalScenes, newScenes, originalScenes.Length);

                                    EditorBuildSettingsScene sceneToAdd = new EditorBuildSettingsScene(relativePath, true);
                                    newScenes[newScenes.Length - 1] = sceneToAdd; 
                                    EditorBuildSettings.scenes = newScenes;
                                }

                                if (_datas.RegenerateEnum)
                                {
                                    GenerateEnumeration();
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                
                EditorGUILayout.EndFoldoutHeaderGroup();
                
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }
        
        private void GenerateEnumeration()
        {
            _datas.EnumerationValues = new List<string>();
            for (var i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                EditorBuildSettingsScene scene = EditorBuildSettings.scenes[i];
                bool sceneEnabled = scene.enabled;
                bool sceneDeleted = File.Exists(scene.path) == false;
                
                if (sceneEnabled && sceneDeleted == false)
                    _datas.EnumerationValues.Add(System.IO.Path.GetFileNameWithoutExtension(scene.path));
            }
                                    
                                    
            EnumerationGenerator generator = new EnumerationGenerator(_datas.EnumerationFolderGeneration, _datas.EnumerationFileName, 
                _datas.EnumerationName, _datas.EnumerationNamespace, _datas.EnumerationValues.ToArray());
            generator.GenerateFile();
        }
        
        #endregion
    }
}

