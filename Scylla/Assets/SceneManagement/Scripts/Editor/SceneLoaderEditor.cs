using System;

namespace Scylla.SceneManagement.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using Scylla.Editor;

    [CustomEditor(typeof(SceneLoader))]
    public class SceneLoaderEditor : Editor
    {
        #region Constantes
        private readonly string PROPERTY_NAME_MIN_LOADING_TIME = "_minimumLoadingTime";
        #endregion
        
        #region Internal Fieldsas;
        private SceneLoaderEditorData _datas;
        private SceneLoader _loader;
        
        private SerializedProperty _propertyMinLoadingTime;
        #endregion

        private void OnEnable()
        {
            _datas = new SceneLoaderEditorData();
        }

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            _loader = target as SceneLoader;
            _propertyMinLoadingTime = serializedObject.FindProperty(PROPERTY_NAME_MIN_LOADING_TIME);
            
            serializedObject.Update();
            OnInspectorGUIObjectField();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.Space();
                EditorGUILayoutUtils.LabelField("Scylla Scene Management == SceneLoader", TextAnchor.MiddleCenter, FontStyle.Bold);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUI.indentLevel++;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_propertyMinLoadingTime, new GUIContent("Minimum loading time"));
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.Space();
                
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                _datas.FoldoutDebug = EditorGUILayout.BeginFoldoutHeaderGroup(_datas.FoldoutDebug, "Debug");
                GUILayout.Space(4);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();

                if (_datas.FoldoutDebug)
                {
                    if (_loader.Collection != null)
                    {
                        var list = _loader.Collection.GetAllScenes(true);
                        for (var i = 0; i < list.Count; i++)
                        {
                            InternalSceneData scene = list[i];
                            
                            EditorGUILayout.BeginVertical();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label(scene.SceneName, GUILayout.Width(180));
                            GUILayout.Label(scene.IsMainScene ? "Main Scene" : "Not main scene", GUILayout.Width(105));
                            GUILayout.Label(scene.IsMarked ? "Marked" : "Not marked", GUILayout.Width(105));
                            GUILayout.Label(scene.IsSuppressible ? "Suppressible" : "Not suppressible",
                                GUILayout.Width(105));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label(scene.BundleIdentifier, GUILayout.Width(180));
                            GUILayout.Label(scene.IsBundleMain ? "Bundle main" : "Not bundle main",
                                GUILayout.Width(105));
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.EndVertical();

                            if (i < list.Count - 1)
                            {
                                EditorGUI.indentLevel--;
                                EditorGUILayoutUtils.HorizontalSeparator();
                                EditorGUI.indentLevel++;
                            }
                                
                        }
                    }

                    /*EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(EditorGUI.indentLevel * 15 + 4);
                        
                    }
                    EditorGUILayout.EndHorizontal();*/
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnInspectorGUIObjectField()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((SceneLoader)target), typeof(SceneLoader), false);
            GUI.enabled = true;
        }
    }
}