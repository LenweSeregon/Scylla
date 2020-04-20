using System.Linq;

namespace Scylla.SceneManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneLoaderRequestUnload : SceneLoaderRequest
    {
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private UnloadStrategy _unloadStrategy;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        public SceneLoaderRequestUnload(UnloadStrategy unloadStrategy, SceneLoaderProxy.SceneLoaderRequestCallback callback = null, bool forceNotSuppressible = false):
            base(callback, forceNotSuppressible)
        {
            _unloadStrategy = unloadStrategy;
        }

        #endregion
        
        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods

        internal override SceneLoaderRequestResult Inspection(SceneCollection collection)
        {
            return _unloadStrategy.Inspection(collection, _forceNotSuppressible);
        }

        internal override Queue<InternalSceneRequest> BuildRequests(SceneCollection collection)
        {
            // Beforehand, we check once again that the request is valid
            // If it's not, this is a development error, warn user and return
            if (Inspection(collection).IsSuccess == false)
            {
                Debug.LogError("Development error -- Contact developer -- Initial inspection went wrong, this request should have been denied");
                return null;
            }

            Queue<InternalSceneRequest> requests = _unloadStrategy.CreateRequests(collection, _forceNotSuppressible);
            return requests;
        }

        #endregion
    }
}