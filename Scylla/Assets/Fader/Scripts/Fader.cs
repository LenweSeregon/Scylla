using System;

namespace Scylla
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Animator))]
    public class Fader : MonoBehaviour
    {
        //=============================================================================//
        //============ Events
        //=============================================================================//
        #region Events
        public event Action _onFadeInCompleted = null;
        public event Action _onFadeOutCompleted = null;

        public event Action OnFadeInCompleted
        {
            add
            {
                _onFadeInCompleted -= value;
                _onFadeInCompleted += value;
            }
            remove
            {
                _onFadeInCompleted -= value;
            }
        }
        public event Action OnFadeOutCompleted
        {
            add
            {
                _onFadeOutCompleted -= value;
                _onFadeOutCompleted += value;
            }
            remove
            {
                _onFadeOutCompleted -= value;
            }
        }
        #endregion
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        #region Serialized Fields
        [SerializeField] private Canvas _canvas = null;
        [SerializeField] private string _triggerNameFadeIn = null;
        [SerializeField] private string _triggerNameFadeOut = null;
        #endregion
        
        //=============================================================================//
        //============ Internal Fields
        //=============================================================================//
        #region Internal Fields
        private Animator _animator;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _canvas.gameObject.SetActive(false);
        }
        #endregion
        
        //=============================================================================//
        //============ Private Methods
        //=============================================================================//
        #region Private Methods

        private void FadeInCompleted()
        {
            _canvas.gameObject.SetActive(false);
            _onFadeInCompleted?.Invoke();
        }

        private void FadeOutCompleted()
        {
            _canvas.gameObject.SetActive(false);
            _onFadeOutCompleted?.Invoke();
        }
        
        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods

        public void FadeIn()
        {
            _canvas.gameObject.SetActive(true);
            _animator.SetTrigger(_triggerNameFadeIn);
        }

        public void FadeOut()
        {
            _canvas.gameObject.SetActive(true);
            _animator.SetTrigger(_triggerNameFadeOut);
        }
        
        #endregion
    }
}

