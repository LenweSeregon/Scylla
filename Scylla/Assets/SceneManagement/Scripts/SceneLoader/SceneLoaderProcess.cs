namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SceneLoaderProcess
    {
        //=============================================================================//
        //============ Internal Fields
        //=============================================================================//
        #region Internal Fields
        private SceneLoader _sceneLoader;
        private float _minimumProcessTime;
        private Queue<InternalSceneRequest> _requests;

        private InternalSceneRequest _currentRequest;
        private AsyncOperation _currentOperation;
        private float _currentProcessTimer;
        private float _currentProcessProgress;
        private float _globalProcessProgress;
        #endregion
        
        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public bool ProcessFinish => _requests.Count == 0 && _currentOperation == null;
        public float RoughProgress => Scylla.MathUtils.Normalize(_globalProcessProgress + _currentProcessProgress);
        public float SmoothProgress
        {
            get
            {
                float smooth = (_minimumProcessTime > 0) ? (_currentProcessTimer / _minimumProcessTime) : (_currentProcessTimer);
                float value = (smooth > RoughProgress) ? (RoughProgress) : (smooth);
                float clamped = Mathf.Clamp01(value);
                return clamped;
            }
        }
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public SceneLoaderProcess(float minimumProcessTime, SceneLoader sceneLoader, Queue<InternalSceneRequest> requests)
        {
            _sceneLoader = sceneLoader;
            _minimumProcessTime = minimumProcessTime;
            _currentProcessTimer = 0f;
            _currentProcessProgress = 0f;
            _requests = requests;
        }
        #endregion

        //=============================================================================//
        //============ Private Methods
        //=============================================================================//
        #region Private Methods
        private bool IsLastRequest()
        {
            return _requests.Count == 0;
        }
        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods

        public void UpdateProcess()
        {
            // Exit update method if there is not current operation processing and all
            // requests has been treated
            if (_requests.Count == 0 && _currentOperation == null)
                return;
            
            // We are sure that there is still request to treat, so if no operation is processing
            // start a new one by dequeuing the next request
            if (_currentRequest == null)
            {
                _currentRequest = _requests.Dequeue();
                _currentOperation = _currentRequest.DoOperation();
                _currentOperation.allowSceneActivation = false;
            }

            // Processing the current operation. 
            // We ensure that allowSceneActivation is only set to true if the request is not the last to be treated, or 
            // if we have exceeded the minimum loading time required.
            // If the current operation is done, we trigger the event and clean to process a new request.
            bool isNotLastRequest = IsLastRequest() == false;
            bool minimumTimerExceeded = _currentProcessTimer >= _minimumProcessTime;
            bool allowSceneActivation = isNotLastRequest || minimumTimerExceeded;
            _currentOperation.allowSceneActivation = allowSceneActivation;
            _currentProcessProgress = _currentOperation.progress;
            SceneLoaderEvents.TriggerOnLoaderProcessUpdate(_currentRequest.GetDescription(), SmoothProgress);
            
            if (_currentOperation.isDone && allowSceneActivation)
            {
                _globalProcessProgress += 1f;
                _currentRequest.DoComplete(_sceneLoader);
                _currentRequest.DoEvent();
                _currentRequest = null;
                _currentOperation = null;
            }

            _currentProcessTimer += Time.deltaTime;
        }
        #endregion
    }
}
