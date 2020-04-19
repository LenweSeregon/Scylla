namespace Scylla.SceneManagement.Editor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneManagerEditorData
    {
        #region Internal Fields
        #endregion
        
        #region Properties
        public bool FoldoutAutoGenerationSettings;
        public bool FoldoutAutoGenerationManualModification;
        public bool FoldoutAutoGenerationGenerate;

        public string EnumerationFolderGeneration;
        public string EnumerationName;
        public string EnumerationNamespace;
        public string EnumerationFileName;
        public List<string> EnumerationValues;
        #endregion
        
        #region Methods
        
        public SceneManagerEditorData()
        {
            FoldoutAutoGenerationSettings = true;
            FoldoutAutoGenerationManualModification = true;
            FoldoutAutoGenerationGenerate = true;
            
            EnumerationName = "SceneType";
            EnumerationFileName = "SceneTypeGenerated";
            EnumerationNamespace = System.IO.Directory.GetParent(Application.dataPath).Name;
            EnumerationValues = new List<string>();

            string scriptPath = SearchFileUtility.SearchScriptNamePath(EnumerationFileName);
            EnumerationFolderGeneration = System.IO.Path.GetDirectoryName(scriptPath);
        }
        
        #endregion
        

    }
}

