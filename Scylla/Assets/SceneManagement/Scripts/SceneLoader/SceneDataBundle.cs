namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SceneDataBundle
    {
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private List<SceneData> _scenes;
        private string _bundleIdentifier;
        private List<string> _bundleMainSceneNames;
        #endregion

        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public List<SceneData> Scenes => _scenes;
        public string BundleIdentifier => _bundleIdentifier;
        public List<string> BundleMainSceneNames => _bundleMainSceneNames;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods

        public SceneDataBundle(SceneData scene)
        {
            _scenes = new List<SceneData>() {scene};
            _bundleIdentifier = null;
            _bundleMainSceneNames = new List<string>() {scene.SceneName};
        }
        
        public SceneDataBundle(List<SceneData> scenes)
        {
            _scenes = scenes;
            _bundleIdentifier = null;
            _bundleMainSceneNames = new List<string>();
        }
        
        public SceneDataBundle(List<SceneData> scenes, string bundleIdentifier, List<string> bundleMainSceneNames)
        {
            _scenes = scenes;
            _bundleIdentifier = bundleIdentifier;
            _bundleMainSceneNames = bundleMainSceneNames;
            ShiftMainBundleScenesToLeft();
        }
        #endregion
        
        //=============================================================================//
        //============ Private / Protected Methods
        //=============================================================================//
        #region Private / Protected Methods

        private void ShiftMainBundleScenesToLeft()
        {
            if (_bundleMainSceneNames == null || _bundleMainSceneNames.Count == 0)
                return;

            for (int i = 0; i < _scenes.Count; i++)
            {
                SceneData scene = _scenes[i];
                if (_bundleMainSceneNames.Contains(scene.SceneName))
                {                
                    for (int j = i - 1; j >= 0; j--)
                    {
                        SceneData previousScene = _scenes[j];
                        if (_bundleMainSceneNames.Contains(previousScene.SceneName))
                        {
                            break;
                        }
                        
                        SceneData tmpPrevious = _scenes[j];
                        _scenes[j] = _scenes[i];
                        _scenes[i] = tmpPrevious;
                    }
                }
            }
        }
        
        #endregion
    }
}