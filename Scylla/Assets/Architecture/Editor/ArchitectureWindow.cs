using System;
using System.Linq;
using Scylla.GenerationModule.Editor;
using Scylla.UtilsModule;

namespace Scylla.Architecture.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public class ArchitectureWindow : EditorWindow
    {
        private string folderVariables;
        private string folderEvents;
        private string folderListeners;
        private string folderUnityEvents;
        private string typeName;
        private string namespaceName;
        
        [MenuItem ("Scylla/Architecture/Create bundle")]
        public static void  ShowWindow () 
        {
            GetWindow(typeof(ArchitectureWindow), false, "Architecture creator");
        }

        private void OnGUI()
        {
            // Variables
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = false;
            EditorGUILayout.TextField(new GUIContent("Folder variables"), folderVariables);
            GUI.enabled = true;
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select folder variables", Application.dataPath, folderVariables);
                folderVariables = path;
                Repaint();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Events
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = false;
            EditorGUILayout.TextField(new GUIContent("Folder Events"), folderEvents);
            GUI.enabled = true;
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select folder events", Application.dataPath, folderEvents);
                folderEvents = path;
                Repaint();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Listeners
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = false;
            EditorGUILayout.TextField(new GUIContent("Folder Listeners"), folderListeners);
            GUI.enabled = true;
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select folder listeners", Application.dataPath, folderListeners);
                folderListeners = path;
                Repaint();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Unity Events
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = false;
            EditorGUILayout.TextField(new GUIContent("Folder Unity Events"), folderUnityEvents);
            GUI.enabled = true;
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select folder Unity Events", Application.dataPath, folderUnityEvents);
                folderUnityEvents = path;
                Repaint();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Type name & namespace
            typeName = EditorGUILayout.TextField ("Type", typeName);
            namespaceName = EditorGUILayout.TextField("Namespace", namespaceName);
            
            // Generate
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Generate", GUILayout.Width(120)))
            {
                string[] usings = {"using System;","using UnityEngine; using Scylla.Architecture;"};
                string typeUpper = typeName.FirstCharacterToUpper();
                string typeVariable = typeUpper + "Variable";
                string typeEvent = typeUpper + "Event";
                string typeListener = typeUpper + "Listener";
                string typeUnityEvent = typeUpper + "UnityEvent";
                
                // Generate variable
                string attributeVariable = "[CreateAssetMenu(fileName = \""+typeVariable+"\", menuName = \"Variables/"+typeVariable+"\")]";
                ClassGenerator generatorVariable = 
                    new ClassGenerator(new[]{attributeVariable}, usings,folderVariables, typeVariable, 
                    typeVariable, namespaceName, "BaseVariable<"+typeName+","+typeEvent+">", null, 
                    null);
                
                // Generate Event
                string attributeEvent = "[CreateAssetMenu(fileName = \""+typeEvent+"\", menuName = \"Variables/"+typeEvent+"\")]";
                ClassGenerator generatorEvent = 
                    new ClassGenerator(new[]{attributeEvent}, usings,folderEvents, typeEvent, 
                        typeEvent, namespaceName, "BaseEvent<"+typeName+">", null, 
                        null);
                
                // Generate Unity Event
                string attributeUnityEvent = "[Serializable]";
                List<string> usingsUnityEvent = new List<string>(usings);
                usingsUnityEvent.Add("using UnityEngine.Events;");
                ClassGenerator generatorUnityEvent = 
                    new ClassGenerator(new[]{attributeUnityEvent}, usingsUnityEvent.ToArray(),folderUnityEvents, typeUnityEvent, 
                        typeUnityEvent, namespaceName, "UnityEvent<"+typeName+">", null, 
                        null);
                
                // Generate Listener
                ClassGenerator generatorListener = 
                    new ClassGenerator(null, usings,folderListeners, typeListener, 
                        typeListener, namespaceName, "BaseListener<"+typeName+", "+ typeEvent+", " + typeUnityEvent+">", 
                        null, 
                        null);

                
                generatorVariable.GenerateClass();
                generatorEvent.GenerateClass();
                generatorListener.GenerateClass();
                generatorUnityEvent.GenerateClass();
            }

            EditorGUILayout.EndHorizontal();
        }
    } 
}

