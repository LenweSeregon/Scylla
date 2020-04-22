namespace Scylla.SceneManagement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// SceneManager is a singleton used to access SceneLoaderProxy and more generally present if we need to
    /// extends the SceneManagement module and regroup features that need to be access in a global way in all
    /// games / applications.
    ///
    /// SceneManager allows to specify a scene to load at start, by setting the boolean to true, fields to enter
    /// scene's informations appears.
    /// SceneManager also allows to register an event on it to be noticed when the SceneManager is ready (ie: when
    /// Awake has been called). It's mainly used by ScyllaScene to override the scene to load at start. As explained
    /// in ScyllaScene, it allows to enter play mode from any scene and ensure that the main scene (at index 0 in build
    /// settings) is loaded first.
    /// So be aware that if you define a scene to load at start in SceneManager and press PlayMode in another scene that
    /// is containing a ScyllaScene's component which force the main scene to be loaded, it will override the one you defined.
    /// 
    /// SceneManager has a special editor.
    /// It allows to auto-generate a .cs file that contains an enumeration representing all scene that are in the BuildSettings.
    /// The auto-generate settings allows to choose the folder where you want to generate the enumeration and should be in your
    /// current project obviously.  You'll then be able to use this enumeration by casting values to string, to communicate with
    /// the SceneLoaderProxy and ask to load / unload scenes with those values.
    /// It allows to auto-generate a scene by configuring where you want the scene to be saved, and if you want to
    /// regenerate the enumeration SceneType and add the scene to buildSettings. The generated scene will by default
    /// has a ScyllaScene component.
    /// </summary>
    public class SceneManager : Singleton<SceneManager>
    {
        //=============================================================================//
        //============ Events & Delegates
        //=============================================================================//
        #region Events & Delegates
        public static event Action _onSceneManagerReady = null;

        #endregion
        
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        #region Serialized Fields
        [SerializeField] private SceneLoaderProxy _sceneLoaderProxy = null;
        [SerializeField] private bool _loadSceneAtStart = false;
        [SerializeField] private string _sceneToLoadAtStartName = null;
        [SerializeField] private bool _sceneToLoadAtStartIsMarked = false;
        [SerializeField] private bool _sceneToLoadAtStartIsSuppressible = false;
        [SerializeField] private string _sceneToLoadAtStartBundleIdentifier = null;
        #endregion

        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private SceneDataBundle _overrideSceneToLoad;
        #endregion
        
        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public SceneLoaderProxy SceneLoaderProxy => _sceneLoaderProxy;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//.
        #region Lifecycle Methods

        protected override void Awake()
        {
            base.Awake();
            _overrideSceneToLoad = null;
            _onSceneManagerReady?.Invoke();
        }

        private void Start()
        {
            SceneDataBundle bundleToLoad = null;
            
            if (_overrideSceneToLoad != null)
            {
                bundleToLoad = _overrideSceneToLoad;
            }
            else if (_loadSceneAtStart)
            {
                List<SceneData> scenesList = new List<SceneData>(){new SceneData(_sceneToLoadAtStartName, _sceneToLoadAtStartIsMarked, _sceneToLoadAtStartIsSuppressible)};
                bundleToLoad = new SceneDataBundle(scenesList, _sceneToLoadAtStartBundleIdentifier, new List<string>(){_sceneToLoadAtStartName});
            }

            if (bundleToLoad != null)
            {
                SceneLoaderRequest request = new SceneLoaderRequestLoad(bundleToLoad, new UnloadStrategyAll());
                _sceneLoaderProxy.PostRequest(request);
            }
        }

        #endregion
        
        //=============================================================================//
        //============ Public Methods
        //=============================================================================//.
        #region Public Methods

        public void ModifySceneToLoadAtStart(SceneDataBundle sceneBundle)
        {
            _overrideSceneToLoad = sceneBundle;
        }
        #endregion
    }
}

