namespace Scylla.CommonModules.IOModule
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class FileUtilityFactory
    {
        public static FileUtility GetFileUtility()
        {
            #if UNITY_STANDALONE || UNITY_EDITOR
                return new StandaloneFileUtility();
            #endif

            return null;
        }
    }
}