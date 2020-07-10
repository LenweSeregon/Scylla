namespace Scylla.Architecture
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class BaseEvent<T> : ScriptableObject
    {
        
        private List<IListener<T>> _listener = new List<IListener<T>>();

        public void Register(IListener<T> listener)
        {
            _listener.Add(listener);
        }
        
        public void Unregister(IListener<T> listener)
        {
            _listener.Remove(listener);
        }
        
        public void Raise(T value)
        {
            for (int i = _listener.Count - 1; i >= 0; i--)
            {
                _listener[i].Raise(value);
            }
        }
    }
}
