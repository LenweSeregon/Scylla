namespace Scylla.CommonModules.IOModule
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class IOFactory
    {
        public static ReadOperation<string> GetReadTextOperation(string filepath)
        {
            #if UNITY_STANDALONE || UNITY_STANDALONE_WIN || UNITY_EDITOR_64
                return new StandaloneReadTextOperation(filepath);
            #endif

            return null;
        }

        public static ReadOperation<byte[]> GetReadBytesOperation(string filepath)
        {
            #if UNITY_STANDALONE || UNITY_EDITOR_64
                return new StandaloneReadBytesOperation(filepath);
            #endif

            return null;
        }

        public static WriteOperation<string> GetWriteTextOperation(string filepath, string content)
        {
            #if UNITY_STANDALONE || UNITY_EDITOR
                return new StandaloneWriteTextOperation(filepath, content);
            #endif

            return null;
        }

        public static WriteOperation<byte[]> GetWriteBytesOperation(string filepath, byte[] content)
        {
            #if UNITY_STANDALONE || UNITY_EDITOR
                return new StandaloneWriteBytesOperation(filepath, content);
            #endif

            return null;
        }
    }
}