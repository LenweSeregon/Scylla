namespace Scylla.Architecture
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class BaseVariable<T, E> : ScriptableObject where E : BaseEvent<T>
    {
        [SerializeField] private T _initialValue;
        [SerializeField] private T _value;
        [SerializeField] private E _eventChanged;

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_eventChanged != null)
                {
                    _eventChanged.Raise(_value);
                }
            }
        }
        
        private void OnEnable()
        {
            _value = _initialValue;
        }
    }
}

