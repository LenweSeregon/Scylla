namespace Scylla.ObjectManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.SceneManagement;
    using UnityEditor.SceneManagement;

    [ExecuteInEditMode, DisallowMultipleComponent]
    public class UniqueIdentifier : MonoBehaviour
    {
        [SerializeField] private SO_UniqueIdentifierAsset _asset;

        public string Guid => _asset.Guid;
        
        private SO_UniqueIdentifierAsset _previousReferencedAsset = null;
        private bool _observingEditorClosing = false;
        private bool _editorWillClose = false;

        private void ObserveEditorClosing()
        {
            if (_observingEditorClosing == false)
            {
                EditorApplication.quitting += Quit;
            }
        }
        
        private void OnValidate()
        {
            ObserveEditorClosing();
            
            if (_asset == null)
            {
                if (_previousReferencedAsset != null)
                {
                    UniqueIdentifierManager.DecreaseReferenceTo(_previousReferencedAsset);
                }

                GenerateUniqueAsset();
            }
            else
            {
                if (_previousReferencedAsset != _asset)
                {
                    if (_previousReferencedAsset != null)
                    {
                        UniqueIdentifierManager.DecreaseReferenceTo(_previousReferencedAsset);
                        UniqueIdentifierManager.IncreaseReferenceTo(_asset);
                    }

                    _previousReferencedAsset = _asset;
                }
            }
        }

        private void Awake()
        {
            ObserveEditorClosing();
            
            if (_asset != null)
            {
                if (UniqueIdentifierManager.IsUnique(_asset.Guid) == false)
                {
                    _asset = null;
                }
            }
            
            if (_asset == null)
            {
                GenerateUniqueAsset();
            }
        }

        private void Quit()
        {
            _editorWillClose = true;
        }
        
        private void OnDestroy()
        {
           if (Application.isPlaying == false && Time.timeSinceLevelLoad > 0f && _editorWillClose == false)
           {
               if (_asset != null)
               {
                   UniqueIdentifierManager.DecreaseReferenceTo(_asset);
               }
           }
        }

        private void GenerateUniqueAsset()
        {
            if (_asset == null)
            {
#if UNITY_EDITOR
                // if in editor, make sure we aren't a prefab of some kind
                if (IsAssetOnDisk())
                {
                    return;
                }
#endif
                _asset = UniqueIdentifierManager.GenerateUniqueIdentifier();
                _previousReferencedAsset = _asset;
                
#if UNITY_EDITOR
                // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
                // force a save of the modified prefab instance properties
                if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                }
#endif
            }
        }
        
#if UNITY_EDITOR
        private bool IsEditingInPrefabMode()
        {
            if (EditorUtility.IsPersistent(this))
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }
            else
            {
                // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
                var mainStage = StageUtility.GetMainStageHandle();
                var currentStage = StageUtility.GetStageHandle(gameObject);
                if (currentStage != mainStage)
                {
                    var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                    if (prefabStage != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool IsAssetOnDisk()
        {
            return PrefabUtility.IsPartOfPrefabAsset(this) || IsEditingInPrefabMode();
        }
#endif
        
    }
}

