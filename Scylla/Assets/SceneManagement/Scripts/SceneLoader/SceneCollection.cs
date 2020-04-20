using System.Text.RegularExpressions;

namespace Scylla.SceneManagement
{
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class SceneCollection
    {
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private List<InternalSceneData> _scenes;
        private List<int> _freedBundleIdentifiersInteger;
        private int _maxBundleIdentifierInteger;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public SceneCollection()
        {
            _maxBundleIdentifierInteger = 0;
            _scenes = new List<InternalSceneData>();
            _freedBundleIdentifiersInteger = new List<int>();
        }
        #endregion

        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods
        
        internal string GetAvailableBundleIdentifier()
        {
            if (_freedBundleIdentifiersInteger.Count == 0)
            {
                string bundleIdentifier = SceneLoaderConstants.BUNDLE_IDENTIFIER_BASIS + _maxBundleIdentifierInteger;
                _maxBundleIdentifierInteger++;
                return bundleIdentifier;
            }
            else
            {
                int freedInteger = _freedBundleIdentifiersInteger[0];
                _freedBundleIdentifiersInteger.RemoveAt(0);
                string bundleIdentifier = SceneLoaderConstants.BUNDLE_IDENTIFIER_BASIS + freedInteger;
                return bundleIdentifier;
            }
        }

        #endregion
        
        //=============================================================================//
        //============ Private / Protected Methods
        //=============================================================================//
        #region Private / Protected Methods

        private void FreeBundleIdentifierNumericIfExists(string bundleIdentifier)
        {
            Match regexMatch = Regex.Match(bundleIdentifier, SceneLoaderConstants.BUNDLE_IDENTIFIER_BASIS+"[0-9]+$");
            if (regexMatch.Success)
            {
                string extractNumericAsString = bundleIdentifier.Replace(SceneLoaderConstants.BUNDLE_IDENTIFIER_BASIS, "");
                int numeric = int.Parse(extractNumericAsString);

                _freedBundleIdentifiersInteger.Add(numeric);
            }
        }
        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//
        #region Public Methods

        public InternalSceneData GetScene(string sceneName)
        {
            return _scenes.Find(scene => scene.SceneName == sceneName);
        }

        public InternalSceneData GetLastScene()
        {
            if (_scenes.Count <= 1)
                return null;

            return _scenes.Last();
        }

        public List<string> GetAllBundleIdentifiers()
        {
            HashSet<string> bundleIdentifiers = new HashSet<string>();
            _scenes.ForEach(scene => bundleIdentifiers.Add(scene.BundleIdentifier));

            return bundleIdentifiers.ToList();
        }

        public List<InternalSceneData> GetAllScenesAtBundle(string bundleIdentifier, bool includeMainBundleScenes)
        {
            return _scenes.FindAll(scene => scene.BundleIdentifier == bundleIdentifier && (includeMainBundleScenes || scene.IsBundleMain == false));
        }

        public List<InternalSceneData> GetAllScenes(bool includeMainScene)
        {
            return (includeMainScene) ? (_scenes) : (_scenes.FindAll(scene => scene.IsMainScene == false));
        }

        public List<InternalSceneData> GetScenesUntilMarked()
        {
            List<InternalSceneData> scenes = new List<InternalSceneData>();
            for (int i = _scenes.Count - 1; i >= 0; i--)
            {
                InternalSceneData scene = _scenes[i];
                if (scene.IsMarked || scene.IsMainScene)
                    break;

                scenes.Add(scene);
            }

            return scenes;
        }
        
        public bool ScenesExists(string sceneName)
        {
            return _scenes.Find(scene => scene.SceneName == sceneName) != null;
        }

        public bool ScenesExists(IEnumerable<string> scenesName, bool all = true)
        {
            List<string> loadedScenesName = _scenes.ConvertAll(scene => scene.SceneName);
            
            return 
                (all) ? 
                (scenesName.All(sceneName => loadedScenesName.Contains(sceneName))) :
                (loadedScenesName.Find(sceneName => scenesName.Contains(sceneName)) != null);
        }

        public bool BundleExists(string bundleIdentifier)
        {
            return _scenes.Find(scene => scene.BundleIdentifier == bundleIdentifier) != null;
        }

        public void AddScene(InternalSceneData sceneData)
        {
            _scenes.Add(sceneData);
        }
        
        public void RemoveSceneByName(string sceneName)
        {
            InternalSceneData scene = GetScene(sceneName);
            if (scene != null)
            {
                FreeBundleIdentifierNumericIfExists(scene.BundleIdentifier);
                _scenes.Remove(scene);
                
            }
        }

        public void RemoveSceneByBundle(string bundleIdentifier)
        {
            int nbRemoved = _scenes.RemoveAll(scene => scene.BundleIdentifier == bundleIdentifier);
            if (nbRemoved > 0)
            {
                FreeBundleIdentifierNumericIfExists(bundleIdentifier);
            }
        }
        #endregion
    }
}

