namespace Scylla.SceneManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneManager : Singleton<SceneManager>
    {
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        #region Serialized Fields
        [SerializeField] private SceneLoaderProxy _sceneLoaderProxy = null;
        #endregion

        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public SceneLoaderProxy SceneLoaderProxy => _sceneLoaderProxy;
        #endregion
    }
}

