namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    internal class InternalSceneRequestLoad : InternalSceneRequest
    {
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public InternalSceneRequestLoad(InternalSceneData sceneData) :
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
            return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_sceneData.SceneName, LoadSceneMode.Additive);
        }

        public override void DoEvent()
        {
            SceneLoaderEvents.TriggerOnSceneLoaded(_sceneData.SceneName);
        }

        public override void DoComplete(SceneLoader sceneLoader)
        {
            sceneLoader.AddSceneToLoadedScene(_sceneData);
        }

        public override string GetDescription()
        {
            return "Loading " + _sceneData.SceneName;
        }
        #endregion
    }
}

