namespace Scylla.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    public static class EditorGUILayoutUtils
    {
        public static int Popup(string previousValue, List<string> options, string label)
        {
            int findBackIndex = options.IndexOf(previousValue);
            
            if (options.Count == 0)
                options.Add("");
            
            if (findBackIndex == -1)
                findBackIndex = 0;
            
            findBackIndex = EditorGUILayout.Popup(new GUIContent(label), findBackIndex, options.ToArray());
            return findBackIndex;
        }
        
        public static void LabelField(string label, GUILayoutOption[] options = null, TextAnchor anchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = -1)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = anchor;
            style.fontSize = fontSize > 0 ? fontSize : style.fontSize;
            style.fontStyle = fontStyle;
            
            EditorGUILayout.LabelField(label, style, options);
        }
        
        public static void LabelField(string label, TextAnchor anchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = -1)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = anchor;
            style.fontSize = fontSize > 0 ? fontSize : style.fontSize;
            style.fontStyle = fontStyle;
            
            EditorGUILayout.LabelField(label, style);
        }
        
        public static void LabelField(string label, Color color, GUILayoutOption[] options = null, TextAnchor anchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = -1)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = anchor;
            style.fontSize = fontSize > 0 ? fontSize : style.fontSize;
            style.fontStyle = fontStyle;
            style.normal.textColor = color;
            
            EditorGUILayout.LabelField(label, style, options);
        }
        
        public static void LabelField(string label, Color color, TextAnchor anchor = TextAnchor.MiddleLeft, FontStyle fontStyle = FontStyle.Normal, int fontSize = -1)
        {
            var style = new GUIStyle(GUI.skin.label);
            style.alignment = anchor;
            style.fontSize = fontSize > 0 ? fontSize : style.fontSize;
            style.fontStyle = fontStyle;
            style.normal.textColor = color;
            
            EditorGUILayout.LabelField(label, style);
        }

        public static void HorizontalSeparator()
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }
    }
}

