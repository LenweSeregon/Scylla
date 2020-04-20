namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SceneMarshaller : IMarshaller<InternalSceneData, SceneData>
    {
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public SceneMarshaller()
        {
            
        }
        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods
        
        public InternalSceneData Marshall(SceneData entry)
        {
            if (entry == null)
                return null;
            
            InternalSceneData sceneData = new InternalSceneData(entry.SceneName, entry.IsMarked, entry.IsSuppressible);
            return sceneData;
        }

        public SceneData Unmarshall(InternalSceneData entry)
        {
            if (entry == null)
                return null;
            
            SceneData sceneData = new SceneData(entry.SceneName, entry.IsMarked, entry.IsSuppressible);
            sceneData.FeedBundleIdentifier(entry.BundleIdentifier);
            return sceneData;
        }
        
        #endregion
    }
}