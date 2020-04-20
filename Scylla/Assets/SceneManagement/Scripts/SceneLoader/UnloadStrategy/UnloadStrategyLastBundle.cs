namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnloadStrategyLastBundle : UnloadStrategy
    {
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private bool _includeMains;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public UnloadStrategyLastBundle(bool includeMains)
        {
            _includeMains = includeMains;
        }

        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods
        internal override Queue<InternalSceneRequest> CreateRequests(SceneCollection collection, bool forceNotSuppressible)
        {
            if (Inspection(collection, forceNotSuppressible).IsSuccess == false)
            {
                Debug.LogError("Development error -- Contact developer -- Initial inspection went wrong, this request should have been denied");
                return null;
            }

            Queue<InternalSceneRequest> requests = new Queue<InternalSceneRequest>();
            List<InternalSceneData> scenes = collection.GetAllScenesAtBundle(collection.GetLastScene()?.BundleIdentifier, true);
            scenes.ForEach(scene => requests.Enqueue(new InternalSceneRequestUnload(scene)));

            return requests;
        }

        internal override SceneLoaderRequestResult Inspection(SceneCollection collection, bool forceNotSuppressible)
        {
            InternalSceneData lastScene = collection.GetLastScene();
            string lastSceneBundleIdentifier = lastScene?.BundleIdentifier;
            List<InternalSceneData> bundleScenes = collection.GetAllScenesAtBundle(lastSceneBundleIdentifier, _includeMains);
            bool scenesNotSuppressible = bundleScenes?.Find(scene => scene.IsSuppressible == false) != null;

            return 
                (forceNotSuppressible == false && scenesNotSuppressible) ? 
                    (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.LoadRejectedUnloadBundleNotSuppressible]) : 
                    (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.Accepted]);
        }
        
        #endregion
    }
}