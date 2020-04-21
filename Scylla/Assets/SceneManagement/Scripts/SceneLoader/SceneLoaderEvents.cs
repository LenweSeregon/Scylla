namespace Scylla.SceneManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// SceneLoaderEvents is a static class that has the responsibility to provide a event that everyone can subscribe
    /// to listen to SceneLoader process. It will indeed allows to be notice about process running, scene loading /
    /// unloading.
    ///
    /// This class is static because there is no reason to instantiate that one, and we should be able from anywhere
    /// to subscribe to those events.
    /// Observe that concerning OnSceneLoaded/OnSceneUnloaded events, there is two versions. When external class from
    /// Scylla want to be notice when there is a scene loaded / unloaded (basically scripts games), they will subscribe
    /// to OnSceneLoaded/OnSceneUnloaded. But for internal purpose, because internally, Scylla is working with
    /// InternalSceneData, we subscribe to OnSceneLoadedInternal/OnSceneUnloadedInternal that works with InternalSceneData.
    /// </summary>
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
        internal static event Action<InternalSceneData> _onSceneLoadedInternal = null;
        internal static event Action<InternalSceneData> _onSceneUnloadedInternal = null;
        
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
        internal static event Action<InternalSceneData> OnSceneLoadedInternal
        {
            add
            {
                _onSceneLoadedInternal -= value;
                _onSceneLoadedInternal += value;
            }
            remove
            {
                _onSceneLoadedInternal -= value;
            }
        }
        internal static event Action<InternalSceneData> OnSceneUnloadedInternal
        {
            add
            {
                _onSceneUnloadedInternal -= value;
                _onSceneUnloadedInternal += value;
            }
            remove
            {
                _onSceneUnloadedInternal -= value;
            }
        }
        #endregion
        
        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods

        internal static void TriggerOnLoaderProcessStart()
        {
            _onLoaderProcessStart?.Invoke();
        }
        
        internal static void TriggerOnLoaderProcessFinish()
        {
            _onLoaderProcessFinish?.Invoke();
        }

        internal static void TriggerOnLoaderProcessUpdate(string description, float progress)
        {
            _onLoaderProcessUpdate?.Invoke(description, progress);
        }
        
        internal static void TriggerOnSceneLoaded(string sceneName)
        {
            _onSceneLoaded?.Invoke(sceneName);
        }
        
        internal static void TriggerOnSceneUnloaded(string sceneName)
        {
            _onSceneUnloaded?.Invoke(sceneName);
        }

        internal static void TriggerOnSceneLoadedInternal(InternalSceneData sceneData)
        {
            _onSceneLoadedInternal?.Invoke(sceneData);
        }
        
        internal static void TriggerOnSceneUnloadedInternal(InternalSceneData sceneData)
        {
            _onSceneUnloadedInternal?.Invoke(sceneData);
        }
        
        #endregion
    }
}
