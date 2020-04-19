namespace Scylla
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class QueueExtension
    {
        public static void AddRange<T>(this Queue<T> queue, IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return;
            
            foreach (T obj in enumerable)
            {
                queue.Enqueue(obj);
            }
        }
    }
    
}

