namespace Scylla.CommonModules.IOModule
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class StandaloneReadBytesOperation : ReadOperation<byte[]>
    {
        public StandaloneReadBytesOperation(string filepath) : 
            base(filepath)
        {
        }

        protected override async Task<bool> MountAsync()
        {
            // Mount is managed by OperationAsync directly
            return true;
        }

        protected override async Task<bool> OperationAsync()
        {
            try
            {
                using (FileStream sourceStream = File.Open(_filepath, FileMode.Open))
                {
                    _content = new byte[sourceStream.Length];
                    await sourceStream.ReadAsync(_content, 0, (int)sourceStream.Length);
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Standalone Read Bytes OperationAsync error : " + e.Message);
                return false;
            }
        }

        protected override async Task<bool> UnmountAsync()
        {
            // Unmount is managed by OperationAsync directly
            return true;
        }

        protected override bool MountSync()
        {
            // Mount is managed by OperationSync directly
            return true;
        }

        protected override bool OperationSync()
        {
            try
            {
                _content = File.ReadAllBytes(_filepath);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Standalone Read Bytes OperationSync error : " + e.Message);
                return false;
            }
        }

        protected override bool UnmountSync()
        {
            // Unmount is managed by OperationSync directly
            return true;
        }
    }
}