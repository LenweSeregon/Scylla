namespace Scylla.SceneManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// SceneLoaderGUI is not mandatory itself in the process of loading a scene, but it is mandatory in term of
    /// visual aspect. Indeed, if your game / application doesn't define a proper way to display a GUI during a
    /// loading / unloading process, it will appear that your game is frozen / crash or visually incorrect because
    /// you'll see the unloading process.
    ///
    /// To help Scylla's SceneManagement users in this process, SceneLoaderGUI represent an abstract class that is
    /// already subscribing to events and properly manage if the SceneLoaderProxy is using a fader (and thus is delaying
    /// the loading / unloading process).
    /// All methods in SceneLoaderGUI are virtual and can be override if you need a specific behaviour, but if your
    /// loading screen is quite basic, you'll only need to override OnLoaderProcessUpdate which is an event that is trigger
    /// periodically to inform the process progress.
    ///
    /// It is not mandatory to inherit this class or to simply use this class, but if you do so, and use Scylla's
    /// SceneManagement, be aware that you need to properly subscribe to SceneLoaderEvents or SceneLoaderProxy's Fader
    /// when it exists in the same manner as SceneLoaderGUI's Start to ensure that your GUI is displaying at the right
    /// moment.
    /// </summary>
    public abstract class SceneLoaderGUI : MonoBehaviour
    {
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        #region SerializedFields
        [SerializeField] private Transform _guiContainer;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//.
        #region Lifecycle Methods
        private void Awake()
        {
            _guiContainer.gameObject.SetActive(false);
        }

        private void Start()
        {
            if (Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.Fader != null)
            {
                Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.OnFadeInCompletedProcessStart += OnProxyFadeInCompletedProcessStart;
                Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.OnFadeInCompletedProcessFinish += OnProxyFadeInCompletedProcessFinish;
            }
            else
            {
                Scylla.SceneManagement.SceneLoaderEvents.OnLoaderProcessFinish += OnLoaderProcessFinish;
                Scylla.SceneManagement.SceneLoaderEvents.OnLoaderProcessStart += OnLoaderProcessStart;
            }

            Scylla.SceneManagement.SceneLoaderEvents.OnLoaderProcessUpdate += OnLoaderProcessUpdate;
        }

        private void OnDestroy()
        {
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.OnFadeInCompletedProcessStart -= OnProxyFadeInCompletedProcessStart;
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.OnFadeInCompletedProcessFinish -= OnProxyFadeInCompletedProcessFinish;
            
            Scylla.SceneManagement.SceneLoaderEvents.OnLoaderProcessStart -= OnLoaderProcessStart;
            Scylla.SceneManagement.SceneLoaderEvents.OnLoaderProcessFinish -= OnLoaderProcessFinish;
            Scylla.SceneManagement.SceneLoaderEvents.OnLoaderProcessUpdate -= OnLoaderProcessUpdate;
        }
        #endregion
        
        //=============================================================================//
        //============ Private / Protected Methods
        //=============================================================================//
        #region Private / Protected Methods
        
        protected virtual void OnProxyFadeInCompletedProcessStart()
        {
            _guiContainer.gameObject.SetActive(true);
        }

        protected virtual void OnProxyFadeInCompletedProcessFinish()
        {
            _guiContainer.gameObject.SetActive(false);
        }
        
        protected virtual void OnLoaderProcessStart()
        {
            _guiContainer.gameObject.SetActive(true);
        }

        protected virtual void OnLoaderProcessFinish()
        {
            _guiContainer.gameObject.SetActive(false);
        }
        
        protected abstract void OnLoaderProcessUpdate(string description, float progress);

        #endregion

    }
}