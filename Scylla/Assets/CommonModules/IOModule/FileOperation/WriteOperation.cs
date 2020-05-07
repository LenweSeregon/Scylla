namespace Scylla.CommonModules.IOModule
{
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class WriteOperation<T> : FileOperation
    {
        protected T _content;
        
        protected WriteOperation(string filepath, T content) : 
            base(filepath)
        {
            _content = content;
        }
    }
}