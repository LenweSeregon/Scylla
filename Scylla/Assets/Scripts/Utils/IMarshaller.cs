namespace Scylla
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public interface IMarshaller<T1,T2>
    {
        T1 Marshall(T2 entry);
        T2 Unmarshall(T1 entry);
    }
}

