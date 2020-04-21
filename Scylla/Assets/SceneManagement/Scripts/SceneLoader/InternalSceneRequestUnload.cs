namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class InternalSceneRequestUnload : InternalSceneRequest
    {
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public InternalSceneRequestUnload(InternalSceneData sceneData) :
            base(sceneData)
        {
            
        }
        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Publics Methods
        public override AsyncOperation DoOperation()
        {
            return UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(_sceneData.SceneName);
        }
        
        public override void DoEvents()
        {
            SceneLoaderEvents.TriggerOnSceneUnloaded(_sceneData.SceneName);
            SceneLoaderEvents.TriggerOnSceneUnloadedInternal(_sceneData);
        }
        
        public override string GetDescription()
        {
            return "Unloading " + _sceneData.SceneName;
        }
        #endregion
        
    }
}