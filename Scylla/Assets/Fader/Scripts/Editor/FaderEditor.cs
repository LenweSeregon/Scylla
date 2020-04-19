namespace Scylla.Editor
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;
    using UnityEditor.Animations;

    [CustomEditor(typeof(Fader))]
    public class FaderEditor : Editor
    {
        #region Constantes
        private readonly string PROPERTY_NAME_CANVAS = "_canvas";
        private readonly string PROPERTY_NAME_TRIGGER_NAME_FADE_IN = "_triggerNameFadeIn";
        private readonly string PROPERTY_NAME_TRIGGER_NAME_FADE_OUT = "_triggerNameFadeOut";
        #endregion
        
        #region Internal Fields

        private SerializedProperty _propertyCanvas;
        private SerializedProperty _propertyTriggerFadeIn;
        private SerializedProperty _propertyTriggerFadeOut;
        #endregion
        
        #region Methods
        public override void OnInspectorGUI()
        {
            _propertyCanvas = serializedObject.FindProperty(PROPERTY_NAME_CANVAS);
            _propertyTriggerFadeIn = serializedObject.FindProperty(PROPERTY_NAME_TRIGGER_NAME_FADE_IN);
            _propertyTriggerFadeOut = serializedObject.FindProperty(PROPERTY_NAME_TRIGGER_NAME_FADE_OUT);
            Animator animator = ((MonoBehaviour) target).GetComponent<Animator>();

            serializedObject.Update();
            OnInspectorGUIObjectField();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(_propertyCanvas, new GUIContent("Canvas"));
            if (animator == null )
            {
                EditorGUILayout.HelpBox("An animator should be add as component beside Fader", MessageType.Error);
            }
            else if (animator.runtimeAnimatorController == null)
            {
                EditorGUILayout.HelpBox("Runtime animator is null in animator", MessageType.Error);
            }
            else
            {
                List<AnimatorControllerParameter> parameters = ((AnimatorController) animator.runtimeAnimatorController).parameters.ToList();
                parameters.RemoveAll(parameter => parameter.type != AnimatorControllerParameterType.Trigger);

                if (parameters.Count == 0)
                {
                    EditorGUILayout.HelpBox("Animator as no trigger parameter", MessageType.Error);
                }
                else
                {
                    List<string> triggerParametersName = parameters.ConvertAll(parameter => parameter.name);
            
                    int indexFadeIn = EditorGUILayoutUtils.Popup(_propertyTriggerFadeIn.stringValue, triggerParametersName, "Trigger FadeIn");
                    int indexFadeOut = EditorGUILayoutUtils.Popup(_propertyTriggerFadeOut.stringValue, triggerParametersName, "Trigger FadeOut");
                    _propertyTriggerFadeIn.stringValue = triggerParametersName[indexFadeIn];
                    _propertyTriggerFadeOut.stringValue = triggerParametersName[indexFadeOut];
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        
        private void OnInspectorGUIObjectField()
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((Fader)target), typeof(Fader), false);
            GUI.enabled = true;
        }
        #endregion
    }
}

