namespace Scylla.CommonModules.IOModule
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class StandaloneWriteTextOperation : WriteOperation<string>
    {
        public StandaloneWriteTextOperation(string filepath, string content) : 
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
                using (StreamWriter writer = File.CreateText(_filepath))
                {
                    await writer.WriteAsync("Example text as string");
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Standalone Write Text OperationAsync error : " + e.Message);
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
                File.WriteAllText(_filepath, _content);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Standalone Write Text OperationSync error : " + e.Message);
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