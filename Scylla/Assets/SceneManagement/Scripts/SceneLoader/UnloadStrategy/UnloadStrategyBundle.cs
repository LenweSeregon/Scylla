namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnloadStrategyBundle : UnloadStrategy
    {
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private string _bundleIdentifier;
        private bool _includeMains;
        #endregion
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public UnloadStrategyBundle(string bundleIdentifier, bool includeMains)
        {
            _bundleIdentifier = bundleIdentifier;
            _includeMains = includeMains;
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
            List<InternalSceneData> scenes = collection.GetAllScenesAtBundle(_bundleIdentifier, true);
            scenes.ForEach(scene => requests.Enqueue(new InternalSceneRequestUnload(scene)));

            return requests;
        }

        internal override SceneLoaderRequestResult Inspection(SceneCollection collection, bool forceNotSuppressible)
        {
            List<InternalSceneData> bundleScenes = collection.GetAllScenesAtBundle(_bundleIdentifier, _includeMains);
            bool scenesNotSuppressible = bundleScenes?.Find(scene => scene.IsSuppressible == false) != null;
            
            return 
                (forceNotSuppressible == false && scenesNotSuppressible) ? 
                    (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.LoadRejectedUnloadBundleNotSuppressible]) : 
                    (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.Accepted]);
        }
        
        #endregion
    }
}

