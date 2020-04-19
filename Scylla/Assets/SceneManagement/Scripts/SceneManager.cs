namespace Scylla.SceneManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneManager : Singleton<SceneManager>
    {
        #region Serialized Fields
        [SerializeField] private SceneLoaderProxy _sceneLoaderProxy = null;
        #endregion

        #region Properties

        public SceneLoaderProxy SceneLoaderProxy => _sceneLoaderProxy;

        #endregion

        #region Methods

        #endregion
    }
}

