namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneData
    {
        //=============================================================================//
        //============ Internal Fields
        //=============================================================================//
        #region Internal Fields
        private string _sceneName;
        private bool _isSuppressible;
        private bool _isMarked;
        private string _bundleIdentifier;
        #endregion

        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public string SceneName => _sceneName;
        public bool IsMarked => _isMarked;
        public bool IsSuppressible => _isSuppressible;
        public string BundleIdentifier => _bundleIdentifier;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        public SceneData(string sceneName)
        {
            _sceneName = sceneName;
            _isMarked = false;
            _isSuppressible = true;
            _bundleIdentifier = null;
        }
        
        public SceneData(string sceneName, bool isMarked, bool isSuppressible)
        {
            _sceneName = sceneName;
            _isMarked = isMarked;
            _isSuppressible = isSuppressible;
            _bundleIdentifier = null;
        }
        
        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods
        internal void FeedBundleIdentifier(string bundleIdentifier)
        {
            _bundleIdentifier = bundleIdentifier;
        }
        
        #endregion
        
        #endregion
    }
}