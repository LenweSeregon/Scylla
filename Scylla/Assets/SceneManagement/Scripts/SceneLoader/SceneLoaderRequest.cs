using System.Collections.ObjectModel;

namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class SceneLoaderRequest
    {
        //=============================================================================//
        //============ Internal Fields
        //=============================================================================//
        #region Internal Fields
        protected SceneLoaderProxy.SceneLoaderRequestCallback _callback;
        protected bool _forceNotSuppressible;
        #endregion
        
        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public SceneLoaderProxy.SceneLoaderRequestCallback Callback => _callback;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public SceneLoaderRequest(SceneLoaderProxy.SceneLoaderRequestCallback callback, bool forceNotSuppressible)
        {
            _callback = callback;
            _forceNotSuppressible = forceNotSuppressible;
        }
        #endregion

        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods
        
        internal static List<InternalSceneData> MarshallToInternalFormat(List<SceneData> scenesDatas)
        {
            SceneMarshaller marshaller = new SceneMarshaller();
            return scenesDatas.ConvertAll(sceneData => marshaller.Marshall(sceneData));
        }

        internal abstract SceneLoaderRequestResult Inspection(SceneCollection collection);
        internal abstract Queue<InternalSceneRequest> BuildRequests(SceneCollection collection);
        
        #endregion
    }
}
