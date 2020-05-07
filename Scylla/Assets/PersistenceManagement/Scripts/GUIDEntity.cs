namespace Scylla.ObjectManagement.GUID
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEditor.Experimental.SceneManagement;
#endif

    [ExecuteInEditMode, DisallowMultipleComponent]
    public class GUIDEntity : MonoBehaviour
    {
        //=============================================================================//
        //============ Static Fields
        //=============================================================================//
        #region Static Fields
        private static Dictionary<string, GUIDEntity> _storableCache = new Dictionary<string, GUIDEntity>();
        #endregion

        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        #region Serialized Fields
        [SerializeField] private byte[] _serializedGuid;
        #endregion
        
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private System.Guid guid = System.Guid.Empty;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//.
        #region Lifecycle Methods

        public void OnDestroy()
        {
            GUIDManager.Remove(guid);
        }
        
        private void Awake()
        {
            CreateGuid();
        }
        
        void OnValidate()
        {
#if UNITY_EDITOR
            // similar to on Serialize, but gets called on Copying a Component or Applying a Prefab
            // at a time that lets us detect what we are
            if (IsAssetOnDisk())
            {
                _serializedGuid = null;
                guid = System.Guid.Empty;
            }
            else
#endif
            {
                CreateGuid();
            }
        }

        // We cannot allow a GUID to be saved into a prefab, and we need to convert to byte[]
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // This lets us detect if we are a prefab instance or a prefab asset.
            // A prefab asset cannot contain a GUID since it would then be duplicated when instanced.
            if (IsAssetOnDisk())
            {
                _serializedGuid = null;
                guid = System.Guid.Empty;
            }
            else
#endif
            {
                if (guid != System.Guid.Empty)
                {
                    _serializedGuid = guid.ToByteArray();
                }
            }
        }

        // On load, we can go head a restore our system guid for later use
        public void OnAfterDeserialize()
        {
            if (_serializedGuid != null && _serializedGuid.Length == 16)
            {
                guid = new System.Guid(_serializedGuid);
            }
        }

        
        #endregion
        
        //=============================================================================//
        //============ Private Methods
        //=============================================================================//.
        #region Private Methods

        void CreateGuid()
        {
            // if our serialized data is invalid, then we are a new object and need a new GUID
            if (_serializedGuid == null || _serializedGuid.Length != 16)
            {
#if UNITY_EDITOR
                // if in editor, make sure we aren't a prefab of some kind
                if (IsAssetOnDisk())
                {
                    return;
                }
                Undo.RecordObject(this, "Added GUID");
#endif
                guid = System.Guid.NewGuid();
                _serializedGuid = guid.ToByteArray();

#if UNITY_EDITOR
                // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
                // force a save of the modified prefab instance properties
                if (PrefabUtility.IsPartOfNonAssetPrefabInstance(this))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                }
#endif
            }
            else if (guid == System.Guid.Empty)
            {
                // otherwise, we should set our system guid to our serialized guid
                guid = new System.Guid(_serializedGuid);
            }

            // register with the GUID Manager so that other components can access this
            if (guid != System.Guid.Empty)
            {
                if (GUIDManager.Add(this) == false)
                {
                    // if registration fails, we probably have a duplicate or invalid GUID, get us a new one.
                    _serializedGuid = null;
                    guid = System.Guid.Empty;
                    CreateGuid();
                }
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

        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//.
        #region Public Methods
        
        public System.Guid GetGuid()
        {
            if (guid == System.Guid.Empty && _serializedGuid != null && _serializedGuid.Length == 16)
            {
                guid = new System.Guid(_serializedGuid);
            }

            return guid;
        }
        #endregion
        
    }
}