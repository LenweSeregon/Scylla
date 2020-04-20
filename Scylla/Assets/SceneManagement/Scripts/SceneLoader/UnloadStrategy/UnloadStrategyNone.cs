namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class UnloadStrategyNone : UnloadStrategy
    {
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public UnloadStrategyNone()
        {
        }
        
        #endregion
        
        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods
        internal override Queue<InternalSceneRequest> CreateRequests(SceneCollection collection, bool forceNotSuppressible)
        {
            return new Queue<InternalSceneRequest>();
        }

        internal override SceneLoaderRequestResult Inspection(SceneCollection collection, bool forceNotSuppressible)
        {
            return SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.Accepted];
        }
        
        #endregion
    }
}