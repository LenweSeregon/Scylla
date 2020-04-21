namespace Scylla.SceneManagement
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneLoaderProxy : MonoBehaviour
    {
        //=============================================================================//
        //============ Events & Delegates
        //=============================================================================//
        #region Events & Delegates
        private event Action _onFadeInCompletedProcessStart = null;
        private event Action _onFadeInCompletedProcessFinish = null;
        
        public event Action OnFadeInCompletedProcessStart
        {
            add
            {
                _onFadeInCompletedProcessStart -= value;
                _onFadeInCompletedProcessStart += value;
            }
            remove
            {
                _onFadeInCompletedProcessStart -= value;
            }
        }
        public event Action OnFadeInCompletedProcessFinish
        {
            add
            {
                _onFadeInCompletedProcessFinish -= value;
                _onFadeInCompletedProcessFinish += value;
            }
            remove
            {
                _onFadeInCompletedProcessFinish -= value;
            }
        }
        
        public delegate void SceneLoaderRequestCallback(bool accepted);
        #endregion
        
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        #region Serialized Fields
        [SerializeField] private bool _requestCancelOlderRequest = true;
        [SerializeField] private Fader _fader = null;
        [SerializeField] private SceneLoader _sceneLoader = null;
        #endregion
        
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private bool _processLoaderFinish;
        private bool _isFading;
        private Queue<InternalSceneRequest> _builtRequestsForLoader;
        private SceneLoaderRequest _currentRequest;
        private Queue<SceneLoaderRequest> _requests;
        private SceneMarshaller _marshaller;
        #endregion
        
        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public Fader Fader => _fader;
        #endregion

        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        private void OnDestroy()
        {
            if (_fader != null)
            {
                _fader.OnFadeInCompleted -= FadeInCompletedProcessStart;
                _fader.OnFadeInCompleted -= FadeInCompletedProcessFinish;
                _fader.OnFadeOutCompleted -= FadeOutCompletedProcessStart;
                _fader.OnFadeOutCompleted -= FadeOutCompletedProcessFinish;
            }

            SceneLoaderEvents.OnLoaderProcessFinish -= OnLoaderProcessFinish;
        }
        
        private void Awake()
        {
            _processLoaderFinish = false;
            _builtRequestsForLoader = null;
            _isFading = false;
            _currentRequest = null;
            _marshaller = new SceneMarshaller();
            _requests = new Queue<SceneLoaderRequest>();
        }

        private void Start()
        {
            SceneLoaderEvents.OnLoaderProcessFinish += OnLoaderProcessFinish;
        }

        private void Update()
        {
            if (_processLoaderFinish && _isFading == false)
            {
                _processLoaderFinish = false;
                if (_fader != null)
                {
                    _isFading = true;
                    _fader.OnFadeInCompleted += FadeInCompletedProcessFinish;
                    _fader.FadeIn();
                }
            }
            
            if (_currentRequest == null && _requests.Count > 0 && _sceneLoader.IsBusy == false)
            {
                _currentRequest = _requests.Dequeue();
                SceneLoaderRequestResult inspectionResult = _currentRequest.Inspection(_sceneLoader.Collection);
                _currentRequest.Callback?.Invoke(inspectionResult.IsSuccess);
                if (inspectionResult.IsSuccess)
                {
                    Queue<InternalSceneRequest> requests = _currentRequest.BuildRequests(_sceneLoader.Collection);
                    if (requests != null && requests.Count > 0)
                    {
                        if (_fader != null)
                        {
                            _builtRequestsForLoader = requests;
                            _isFading = true;
                            _fader.OnFadeInCompleted += FadeInCompletedProcessStart;
                            _fader.FadeIn();
                        }
                        else
                        {
                            _sceneLoader.SendRequest(requests);
                        }
                    }
                }
                
                _currentRequest = null;
            }
        }

        #endregion
        
        //=============================================================================//
        //============ Private / Protected Methods
        //=============================================================================//
        #region Private / Protected Methods

        private void OnLoaderProcessFinish()
        {
            _processLoaderFinish = true;
        }

        private void FadeInCompletedProcessFinish()
        {
            _fader.OnFadeInCompleted -= FadeInCompletedProcessFinish;
            _fader.OnFadeOutCompleted += FadeOutCompletedProcessFinish;
            _onFadeInCompletedProcessFinish?.Invoke();
            _fader.FadeOut();
        }

        private void FadeOutCompletedProcessFinish()
        {
            _fader.OnFadeOutCompleted -= FadeOutCompletedProcessFinish;
            _isFading = false;
        }

        private void FadeInCompletedProcessStart()
        {
            _fader.OnFadeInCompleted -= FadeInCompletedProcessStart;
            _fader.OnFadeOutCompleted += FadeOutCompletedProcessStart;
            _onFadeInCompletedProcessStart?.Invoke();
            _fader.FadeOut();
            _sceneLoader.SendRequest(_builtRequestsForLoader);
        }

        private void FadeOutCompletedProcessStart()
        {
            _fader.OnFadeOutCompleted -= FadeOutCompletedProcessStart;
            _isFading = false;
        }
        
        private void AttemptClearOlderRequests()
        {
            if (_requestCancelOlderRequest)
            {
                _requests.Clear();
            }
        }

        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods
        public void PostRequest(SceneLoaderRequest loadRequest)
        {
            AttemptClearOlderRequests();
            _requests.Enqueue(loadRequest);
        }

        public bool Exists(string sceneName)
        {
            return GetScene(sceneName) != null;
        }
        
        public SceneData GetScene(string sceneName)
        {
            InternalSceneData scene = _sceneLoader.Collection.GetScene(sceneName);
            SceneData sceneConverted = _marshaller.Unmarshall(scene);
            return sceneConverted;
        }

        public SceneData GetLastScene()
        {
            InternalSceneData scene = _sceneLoader.Collection.GetLastScene();
            return _marshaller.Unmarshall(scene);
        }

        public List<SceneData> GetAllScenes()
        {
            List<InternalSceneData> scenes = _sceneLoader.Collection.GetAllScenes(false);
            List<SceneData> scenesConverted = scenes.ConvertAll(scene => _marshaller.Unmarshall(scene));
            return scenesConverted;
        }
        
        public List<SceneData> GetAllScenesAtBundle(string bundleIdentifier)
        {
            List<InternalSceneData> scenes = _sceneLoader.Collection.GetAllScenesAtBundle(bundleIdentifier, true);
            List<SceneData> scenesConverted = scenes.ConvertAll(scene => _marshaller.Unmarshall(scene));
            return scenesConverted;
        }

        public List<string> GetAllBundleIdentifiers()
        {
            List<string> bundleIdentifiers = _sceneLoader.Collection.GetAllBundleIdentifiers();
            return bundleIdentifiers;
        }
        #endregion
    }
}