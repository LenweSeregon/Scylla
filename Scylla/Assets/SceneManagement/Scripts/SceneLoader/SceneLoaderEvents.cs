namespace Scylla.SceneManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class SceneLoaderEvents
    {
        //=============================================================================//
        //============ Events & Delegates
        //=============================================================================//
        #region Events & Delegates
        private static event Action _onLoaderProcessStart = null;
        private static event Action _onLoaderProcessFinish = null;
        private static event Action<string, float> _onLoaderProcessUpdate = null;
        private static event Action<string> _onSceneLoaded = null;
        private static event Action<string> _onSceneUnloaded = null;

        public static event Action OnLoaderProcessStart
        {
            add
            {
                _onLoaderProcessStart -= value;
                _onLoaderProcessStart += value;
            }
            remove
            {
               _onLoaderProcessStart -= value;
            }
        }
        public static event Action OnLoaderProcessFinish
        {
            add
            {
                _onLoaderProcessFinish -= value;
                _onLoaderProcessFinish += value;
            }
            remove
            {
                _onLoaderProcessFinish -= value;
            }
        }
        public static event Action<string, float> OnLoaderProcessUpdate
        {
            add
            {
                _onLoaderProcessUpdate -= value;
                _onLoaderProcessUpdate += value;
            }
            remove
            {
                _onLoaderProcessUpdate -= value;
            }
        }
        public static event Action<string> OnSceneLoaded
        {
            add
            {
                _onSceneLoaded -= value;
                _onSceneLoaded += value;
            }
            remove
            {
                _onSceneLoaded -= value;
            }
        }
        public static event Action<string> OnSceneUnloaded
        {
            add
            {
                _onSceneUnloaded -= value;
                _onSceneUnloaded += value;
            }
            remove
            {
                _onSceneUnloaded -= value;
            }
        }
        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods

        public static void TriggerOnLoaderProcessStart()
        {
            _onLoaderProcessStart?.Invoke();
        }
        
        public static void TriggerOnLoaderProcessFinish()
        {
            _onLoaderProcessFinish?.Invoke();
        }

        public static void TriggerOnLoaderProcessUpdate(string description, float progress)
        {
            _onLoaderProcessUpdate?.Invoke(description, progress);
        }
        
        public static void TriggerOnSceneLoaded(string sceneName)
        {
            _onSceneLoaded?.Invoke(sceneName);
        }
        
        public static void TriggerOnSceneUnloaded(string sceneName)
        {
            _onSceneUnloaded?.Invoke(sceneName);
        }
        
        #endregion
    }
}
