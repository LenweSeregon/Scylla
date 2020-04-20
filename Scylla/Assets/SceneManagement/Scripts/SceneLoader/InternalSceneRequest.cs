namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// 
    /// </summary>
    internal abstract class InternalSceneRequest
    { 
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        protected readonly InternalSceneData _sceneData;
        #endregion
        
        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public InternalSceneData SceneData => _sceneData;
        #endregion

        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        protected InternalSceneRequest(InternalSceneData sceneData)
        {
            _sceneData = sceneData;
        }
        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods
        public abstract AsyncOperation DoOperation();
        public abstract void DoEvent();
        public abstract void DoComplete(SceneLoader sceneLoader);
        public abstract string GetDescription();
        #endregion

    }
}

