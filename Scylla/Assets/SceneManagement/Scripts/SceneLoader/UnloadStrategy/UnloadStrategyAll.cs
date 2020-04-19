namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnloadStrategyAll : UnloadStrategy
    {
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods
        internal override Queue<InternalSceneRequest> CreateRequests(SceneCollection collection, bool forceNotSuppressible)
        {
            if (Inspection(collection, forceNotSuppressible)._isSuccess == false)
            {
                Debug.LogError("Development error -- Contact developer -- Initial inspection went wrong, this request should have been denied");
                return null;
            }

            Queue<InternalSceneRequest> requests = new Queue<InternalSceneRequest>();
            List<InternalSceneData> scenes = collection.GetAllScenes(false);
            scenes.ForEach(scene => requests.Enqueue(new InternalSceneRequestUnload(scene)));

            return requests;
        }

        internal override SceneLoaderRequestResult Inspection(SceneCollection collection, bool forceNotSuppressible)
        {
            List<InternalSceneData> allScenes = collection.GetAllScenes(false);
            bool scenesNotSuppressible = allScenes?.Find(scene => scene.IsSuppressible == false) != null;

            return 
                (forceNotSuppressible == false && scenesNotSuppressible)
                    ? (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.LoadRejectedUnloadAllNotSuppressible])
                    : (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.Accepted]);
        }
        
        #endregion
    }
}

