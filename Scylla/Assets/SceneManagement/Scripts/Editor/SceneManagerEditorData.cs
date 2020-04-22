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

        public bool FoldoutBuildSettingsVisualizer;
        public bool FoldoutBuildSettingsManagement;
        
        public bool FoldoutAutoGenerationSettings;
        public bool FoldoutAutoGenerationManualModification;
        public bool FoldoutAutoGenerationGenerate;

        public bool FoldoutAutoGenerationSceneGlobalSettings;
        public bool FoldoutAutoGenerationSceneSettings;
        public bool FoldoutAutoGenerationSceneGenerate;
        
        public string EnumerationFolderGeneration;
        public string EnumerationName;
        public string EnumerationNamespace;
        public string EnumerationFileName;
        public List<string> EnumerationValues;

        public string SceneFolderGeneration;
        public string SceneName;
        public bool SceneAddToBuild;
        public bool RegenerateEnum;
        #endregion
        
        #region Methods
        
        public SceneManagerEditorData()
        {
            FoldoutBuildSettingsVisualizer = true;
            FoldoutBuildSettingsManagement = true;
            
            FoldoutAutoGenerationSettings = true;
            FoldoutAutoGenerationManualModification = true;
            FoldoutAutoGenerationGenerate = true;

            FoldoutAutoGenerationSceneGlobalSettings = true;
            FoldoutAutoGenerationSceneSettings = true;
            FoldoutAutoGenerationSceneGenerate = true;
            
            EnumerationName = "SceneType";
            EnumerationFileName = "SceneTypeGenerated";
            EnumerationNamespace = System.IO.Directory.GetParent(Application.dataPath).Name;
            EnumerationValues = new List<string>();

            string scriptPath = SearchFileUtility.SearchScriptNamePath(EnumerationFileName);
            EnumerationFolderGeneration = System.IO.Path.GetDirectoryName(scriptPath);

            SceneFolderGeneration = "";
            SceneName = "";
            SceneAddToBuild = true;
            RegenerateEnum = true;
        }
        
        #endregion
        

    }
}

