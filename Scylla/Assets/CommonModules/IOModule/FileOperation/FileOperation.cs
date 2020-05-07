using System;

namespace Scylla.CommonModules.IOModule
{
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum FileOperationState
    {
        Success,
        Fail
    }
    
    public abstract class FileOperation
    {
        protected string _filepath;
        protected FileOperationState _state;
        
        protected FileOperation(string filepath)
        {
            _filepath = filepath;
        }

        protected abstract Task<bool> MountAsync();
        protected abstract Task<bool> OperationAsync();
        protected abstract Task<bool> UnmountAsync();

        protected abstract bool MountSync();
        protected abstract bool OperationSync();
        protected abstract bool UnmountSync();
        
        
        
        public async void DoOperationAsync()
        {
            await MountAsync();
            await OperationAsync();
            await UnmountAsync();
        }

        public void DoOperationSync()
        { 
            MountSync();
            OperationSync();
            UnmountSync();
        }
    }
}

