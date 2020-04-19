namespace Scylla
{
    using System.IO;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public static class SearchFileUtility
    {
        public static string SearchScriptNamePath(string scriptName)
        {
            string scriptNameWithExtension = (Path.GetExtension(scriptName) == "cs")  ? (scriptName) : (Path.GetFileNameWithoutExtension(scriptName) + ".cs");
            string[] res = System.IO.Directory.GetFiles(Application.dataPath, scriptNameWithExtension, SearchOption.AllDirectories);
            if (res.Length == 0)
            {
                return null;
            }

            return res[0].Replace("\\", "/");
        }
    }
    
}

