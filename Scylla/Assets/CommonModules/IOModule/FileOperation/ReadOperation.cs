namespace Scylla.CommonModules.IOModule
{
    using System;
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class ReadOperation<T> : FileOperation
    {
        protected T _content;
        
        protected ReadOperation(string filepath) : 
            base(filepath)
        {
            
        }

        public T GetResult()
        {
            if (_content == null)
                throw new Exception("Something when wrong during read operation");

            return _content;
        }
    }
}