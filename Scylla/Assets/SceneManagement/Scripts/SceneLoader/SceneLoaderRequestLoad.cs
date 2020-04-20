namespace Scylla.SceneManagement
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneLoaderRequestLoad : SceneLoaderRequest
    {
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private SceneDataBundle _bundle;
        private UnloadStrategy _unloadStrategy;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        public SceneLoaderRequestLoad(string sceneName, UnloadStrategy unloadStrategy = null, SceneLoaderProxy.SceneLoaderRequestCallback callback = null, bool forceNotSuppressible = false):
            base(callback, forceNotSuppressible)
        {
            SceneData asSceneData = new SceneData(sceneName);
            List<SceneData> asSceneDataList = new List<SceneData>() {asSceneData};
            SceneDataBundle asBundle = new SceneDataBundle(asSceneDataList, null, new List<string>() {sceneName});

            _bundle = asBundle;
            _unloadStrategy = unloadStrategy ?? new UnloadStrategyNone();
        }
        
        public SceneLoaderRequestLoad(SceneData scene, UnloadStrategy unloadStrategy = null, SceneLoaderProxy.SceneLoaderRequestCallback callback = null, bool forceNotSuppressible = false):
            base(callback, forceNotSuppressible)
        {
            List<SceneData> asSceneDataList = new List<SceneData>() {scene};
            SceneDataBundle asBundle = new SceneDataBundle(asSceneDataList, null, new List<string>(){scene.SceneName});
            
            _bundle = asBundle;
            _unloadStrategy = unloadStrategy ?? new UnloadStrategyNone();
        }
        
        public SceneLoaderRequestLoad(List<string> scenesName, UnloadStrategy unloadStrategy = null, SceneLoaderProxy.SceneLoaderRequestCallback callback = null, bool forceNotSuppressible = false):
            base(callback, forceNotSuppressible)
        {
            List<SceneData> asSceneDataList = scenesName.ConvertAll(sceneName => new SceneData(sceneName));
            SceneDataBundle asBundle = new SceneDataBundle(asSceneDataList, null, null);
            
            _bundle = asBundle;
            _unloadStrategy = unloadStrategy ?? new UnloadStrategyNone();
        }
        
        public SceneLoaderRequestLoad(List<SceneData> scenes, UnloadStrategy unloadStrategy = null, SceneLoaderProxy.SceneLoaderRequestCallback callback = null, bool forceNotSuppressible = false):
            base(callback, forceNotSuppressible)
        {
            SceneDataBundle asBundle = new SceneDataBundle(scenes, null, null);
            
            _bundle = asBundle;
            _unloadStrategy = unloadStrategy ?? new UnloadStrategyNone();
        }
        
        public SceneLoaderRequestLoad(SceneDataBundle bundle, UnloadStrategy unloadStrategy = null, SceneLoaderProxy.SceneLoaderRequestCallback callback = null, bool forceNotSuppressible = false):
            base(callback, forceNotSuppressible)
        {
            _bundle = bundle;
            _unloadStrategy = unloadStrategy ?? new UnloadStrategyNone();
        }
        #endregion
        
        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods
        internal override SceneLoaderRequestResult Inspection(SceneCollection collection)
        {
            SceneLoaderRequestResult resultSceneInspection = ScenesExistsInspection(collection);
            if (resultSceneInspection.IsSuccess == false) return resultSceneInspection;
            
            SceneLoaderRequestResult resultBundleInspection = BundleExistsInspection(collection);
            if (resultBundleInspection.IsSuccess == false) return resultBundleInspection;
            
            SceneLoaderRequestResult resultAdditionalInspection = AdditionalInspection(collection);
            if (resultAdditionalInspection.IsSuccess == false) return resultAdditionalInspection;
            
            return (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.Accepted]);
        }
        
        internal override Queue<InternalSceneRequest> BuildRequests(SceneCollection collection)
        {
            // Beforehand, we check once again that the request is valid
            // If it's not, this is a development error, warn user and return
            Queue<InternalSceneRequest> requestsUnload = _unloadStrategy.CreateRequests(collection, _forceNotSuppressible);
            if (Inspection(collection).IsSuccess == false)
            {
                Debug.LogError("Development error -- Contact developer -- Initial inspection went wrong, this request should have been denied");
                return null;
            }
            if (requestsUnload == null)
            {
                Debug.LogError("Development error -- Contact developer -- Initial inspection went wrong, this request should have been denied");
                return null;
            }
            
            // First of all, let's marshall all data input to convert them into an internal format
            // Then, from marshalled data, feeded bundle properties to scenes datas
            List<InternalSceneData> marshalledScenesToLoad = MarshallToInternalFormat(_bundle.Scenes);
            List<InternalSceneData> bundleScenesToLoad = BundleIdentifierAttribution(collection, marshalledScenesToLoad);
            
            Queue<InternalSceneRequest> requests = new Queue<InternalSceneRequest>();
            requests.AddRange(requestsUnload);
            bundleScenesToLoad.ForEach(data => requests.Enqueue(new InternalSceneRequestLoad(data)));

            return requests;
        }

        #endregion
        
        //=============================================================================//
        //============ Private / Protected Methods
        //=============================================================================//
        #region Private / Protected Methods

        private List<InternalSceneData> BundleIdentifierAttribution(SceneCollection collection, List<InternalSceneData> scenesData)
        {
            string bundleIdentifier = (string.IsNullOrEmpty(_bundle.BundleIdentifier)) ? (collection.GetAvailableBundleIdentifier()) : (_bundle.BundleIdentifier);
            List<InternalSceneData> scenesDataBundleAttributed = new List<InternalSceneData>(scenesData);
            scenesDataBundleAttributed.ForEach(sceneData => sceneData.FeedBundleInformation(bundleIdentifier, _bundle.BundleMainSceneNames.Contains(sceneData.SceneName)));
            
            return scenesDataBundleAttributed;
        }
        
        private SceneLoaderRequestResult ScenesExistsInspection(SceneCollection collection)
        {
            List<string> scenesName = _bundle.Scenes.ConvertAll(scene => scene.SceneName);
            bool atLeast1SceneExists = collection.ScenesExists(scenesName, false);
            
            return  
                (atLeast1SceneExists) ? 
                (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.LoadRejectedSceneExists]) : 
                (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.Accepted]);
        }

        private SceneLoaderRequestResult BundleExistsInspection(SceneCollection collection)
        {
            string bundleIdentifier = _bundle.BundleIdentifier;
            bool bundleIdentifierIsNull = string.IsNullOrEmpty(bundleIdentifier);
            bool bundleIdentifierExists = bundleIdentifierIsNull == false && collection.BundleExists(bundleIdentifier);

            return
                (bundleIdentifierExists) ?  
                (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.LoadRejectedBundleExists]) : 
                (SceneLoaderConstants.REQUEST_RESULTS[SceneLoaderReturnType.Accepted]);
        }

        private SceneLoaderRequestResult AdditionalInspection(SceneCollection collection)
        {
            return _unloadStrategy.Inspection(collection, _forceNotSuppressible);
        }
        
        #endregion

    }
}