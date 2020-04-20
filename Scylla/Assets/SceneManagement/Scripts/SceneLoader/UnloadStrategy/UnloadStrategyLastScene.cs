namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnloadStrategyLastScene : UnloadStrategy
    {
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
            requests.Enqueue(new InternalSceneRequestUnload(collection.GetLastScene()));
            return requests;
        }

        internal override SceneLoaderRequestResult Inspection(SceneCollection collection, bool forceNotSuppressible)
        {
            InternalSceneData lastScene = collection.GetLastScene();
            bool lastSceneNotSuppressible = lastScene != null && forceNotSuppressible == false && lastScene.IsSuppressible == false;
            
            return 
                (lastSceneNotSuppressible) ? 
                    (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.LoadRejectedUnloadLastSceneNotSuppressible]) : 
                    (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.Accepted]);
        }
        
        #endregion
    }
}