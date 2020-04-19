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
        
        public override void DoEvent()
        {
            SceneLoaderEvents.TriggerOnSceneUnloaded(_sceneData.SceneName);
        }
        
        public override void DoComplete(SceneLoader sceneLoader)
        {
            sceneLoader.RemoveSceneFromLoadedScene(_sceneData.SceneName);
        }

        public override string GetDescription()
        {
            return "Unloading " + _sceneData.SceneName;
        }
        #endregion
        
    }
}