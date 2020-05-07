namespace Scylla.CommonModules.IOModule
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class StandaloneWriteBytesOperation : WriteOperation<byte[]>
    {
        public StandaloneWriteBytesOperation(string filepath, byte[] content) : 
            base(filepath, content)
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
                using (FileStream sourceStream = File.Open(_filepath, FileMode.OpenOrCreate))
                {
                    sourceStream.Seek(0, SeekOrigin.End);
                    await sourceStream.WriteAsync(_content, 0, _content.Length);
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Standalone Write Bytes OperationAsync error : " + e.Message);
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
                File.WriteAllBytes(_filepath, _content);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Standalone Write Bytes OperationSync error : " + e.Message);
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