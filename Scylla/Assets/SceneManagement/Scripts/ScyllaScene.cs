namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using System;
    using UnityEngine.SceneManagement;
    
    /// <summary>
    /// ScyllaScene is not mandatory to works with Scylla SceneManagement strictly speaking because SceneManager and
    /// SceneLaoder doesn't know the existence of ScyllaScene (for the moment).
    /// But in order to force load the manager scene WHICH NEED TO BE AT INDEX 0 IN BUILD SETTINGS, when you press
    /// play from any scene, your scene need to have a ScyllaScene component which will take care of it in its Awake method
    /// to force the loading of the main scene if it is not already loaded.
    ///
    /// It'll basically checks if the scene at index 0 in build settings is loaded or not. If it is not, ScyllaScene
    /// will ask to load NOT asynchronously the first scene and give an action on the SceneManager which need to be call
    /// when the SceneManager is ready. In this action, ScyllaScene ask the SceneManager to modify its first
    /// scene to be loaded to load the current scene (which is the one from where the developer clicked play).
    ///
    /// Note that, this hacky way to works is only needed in editor mode, because, when playing the game in build mode,
    /// the scene at index 0 in build settings is automatically the first one loaded.
    /// </summary>
    public class ScyllaScene : MonoBehaviour
    {
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        #region Serialized Fields
        [SerializeField] private bool _forceLoadMainScene = true;
        #endregion
        
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private string _currentSceneName;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        protected virtual void Awake()
        {
            _currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            bool mainSceneLoaded = UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(0).isLoaded;
            if (mainSceneLoaded == false && _forceLoadMainScene)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(0, LoadSceneMode.Single);
                Scylla.SceneManagement.SceneManager._onSceneManagerReady += OnSceneManagerReady;
            }
        }

        #endregion
        
        //=============================================================================//
        //============ Private Methods
        //=============================================================================//
        #region Private Methods

        private void OnSceneManagerReady()
        {
            Scylla.SceneManagement.SceneManager._onSceneManagerReady -= OnSceneManagerReady;
            SceneManager.Instance.ModifySceneToLoadAtStart(new SceneDataBundle(new List<SceneData> {new SceneData(_currentSceneName)}));            
        }
        #endregion
    }    
}