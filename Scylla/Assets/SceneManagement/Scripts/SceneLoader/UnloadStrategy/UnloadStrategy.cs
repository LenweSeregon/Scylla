namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class UnloadStrategy
    {
        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods
        internal abstract Queue<InternalSceneRequest> CreateRequests(SceneCollection collection, bool forceNotSuppressible);
        internal abstract SceneLoaderRequestResult Inspection(SceneCollection collection, bool forceNotSuppressible);
        
        #endregion
    }
}

