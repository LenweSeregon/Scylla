namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// SceneData is an internal plain data class that is only use to store information about a loaded scene.
    /// SceneData is intended to be store in a list in SceneLoader so he can retrieve information about scenes loaded.
    /// </summary>
    internal class InternalSceneData
    {
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private string _sceneName;
        private bool _isSuppressible;
        private bool _isMarked;
        private bool _isMainScene;
        private bool _isBundleMain;
        private string _bundleIdentifier;
        #endregion

        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public string SceneName => _sceneName;
        public bool IsMarked => _isMarked;
        public bool IsSuppressible => _isSuppressible;
        public bool IsMainScene => _isMainScene;
        public bool IsBundleMain => _isBundleMain;
        public string BundleIdentifier => _bundleIdentifier;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        
        /// <summary>
        /// Constructor of SceneData
        /// </summary>
        /// <param name="sceneName">name of the scene</param>
        /// <param name="isMarked">is the scene marked (in SceneLoader manner)</param>
        /// <param name="isSuppressible">is the scene deletable (in SceneLoader manner)</param>
        /// <param name="isMain">is the main scene (in SceneLoader manner)</param>
        public InternalSceneData(string sceneName, bool isMarked, bool isSuppressible, bool isMain, string bundleIdentifier, bool isBundleMain)
        {
            _sceneName = sceneName;
            _isMarked = isMarked;
            _isSuppressible = isSuppressible;
            _isMainScene = isMain;
            _bundleIdentifier = bundleIdentifier;
            _isBundleMain = isBundleMain;
        }

        public InternalSceneData(string sceneName, bool isMarked, bool isSuppressible, bool isMain)
        {
            _sceneName = sceneName;
            _isMarked = isMarked;
            _isSuppressible = isSuppressible;
            _isMainScene = isMain;
            _bundleIdentifier = "";
            _isBundleMain = false;
        }
        
        public InternalSceneData(string sceneName, bool isMarked, bool isSuppressible)
        {
            _sceneName = sceneName;
            _isMarked = isMarked;
            _isSuppressible = isSuppressible;
            _isMainScene = false;
            _bundleIdentifier = "";
            _isBundleMain = false;
        }
        
        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods
        public void FeedBundleInformation(string bundleIdentifier, bool isBundleMain)
        {
            _bundleIdentifier = bundleIdentifier;
            _isBundleMain = isBundleMain;
        }
        #endregion
    }
}

