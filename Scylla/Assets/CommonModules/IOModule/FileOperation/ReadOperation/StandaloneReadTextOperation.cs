using System.Text;

namespace Scylla.CommonModules.IOModule
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class StandaloneReadTextOperation : ReadOperation<string>
    {
        public StandaloneReadTextOperation(string filepath) : 
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
                using (StreamReader reader = File.OpenText(_filepath))
                {
                    _content = await reader.ReadToEndAsync();
                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Standalone Read Text OperationAsync error : " + e.Message);
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
                _content = File.ReadAllText(_filepath, Encoding.UTF8);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("Standalone Read Text OperationSync error : " + e.Message);
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