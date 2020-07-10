namespace Scylla.Architecture
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IListener<T>
    {
        void Raise(T value);
    }
}