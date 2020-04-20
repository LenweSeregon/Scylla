namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnloadStrategySimple : UnloadStrategy
    {        
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private List<SceneData> _scenes;
        private SceneMarshaller _sceneMarshaller;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        public UnloadStrategySimple(string sceneName)
        {
            SceneData asSceneData = new SceneData(sceneName);
            _scenes = new List<SceneData>() {asSceneData};
            _sceneMarshaller = new SceneMarshaller();
        }

        public UnloadStrategySimple(List<string> scenesName)
        { 
            _scenes = scenesName.ConvertAll(sceneName => new SceneData(sceneName));
            _sceneMarshaller = new SceneMarshaller();
        }

        #endregion

        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods
        internal override Queue<InternalSceneRequest> CreateRequests(SceneCollection collection, bool forceNotSuppressible)
        {
            if (Inspection(collection, forceNotSuppressible).IsSuccess == false)
            {
                Debug.LogError("Development error -- Contact developer -- Initial inspection went wrong, this request should have been denied");
                return null;
            }

            Queue<InternalSceneRequest> requests = new Queue<InternalSceneRequest>();
            List<InternalSceneData> marshalledSceneData = _scenes.ConvertAll(scene => _sceneMarshaller.Marshall(scene));
            marshalledSceneData.ForEach(scene => requests.Enqueue(new InternalSceneRequestUnload(scene)));

            return requests;
        }

        internal override SceneLoaderRequestResult Inspection(SceneCollection collection, bool forceNotSuppressible)
        {
            if (_scenes == null || _scenes.Count == 0)
                return SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.UnloadRejectedSceneNotExists];
            
            List<string> scenesName = _scenes.ConvertAll(scene => scene.SceneName);
            bool allExists = collection.ScenesExists(scenesName, true);

            return 
                (allExists)
                ? (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.Accepted])
                : (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.UnloadRejectedSceneNotExists]);
        }
        
        #endregion
    }
}