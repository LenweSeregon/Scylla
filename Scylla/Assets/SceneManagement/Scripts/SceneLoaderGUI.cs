using Scylla.SceneManagement;

namespace Scylla
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class SceneLoaderGUI : MonoBehaviour
    {
        #region SerializedFields
        [SerializeField] private Transform _guiContainer;
        #endregion
        
        #region Methods

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