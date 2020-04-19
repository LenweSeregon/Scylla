namespace Scylla
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        private static T _mInstance;

        public static T Instance => _mInstance;

        protected virtual void Awake()
        {
            if (_mInstance != null && _mInstance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                _mInstance = (T) this ;
            }
        }
    }
}

