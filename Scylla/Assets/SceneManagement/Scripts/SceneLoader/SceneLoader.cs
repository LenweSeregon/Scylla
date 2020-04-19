namespace Scylla.SceneManagement
{
    using System.Linq;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// SceneLoader component is responsible for loading and holding consistency of scenes in Scylla engine.
    /// It is base on additive scene, means that you'll always load your scenes in additive mode but based on parameters given in methods
    /// this loader will ensure to unload or not the scenes.
    /// The additive mode is used to avoid using DontDestroyOnLoad as recommended in Unity's documentation and so, it permit to use
    /// "ManagerScene" where singleton and long life objects can stay available during all the game's lifecycle.
    /// It is also based on 2 principles which are deletable and marked.
    ///     - Deletable : when loading process is asked, we give a boolean parameter to determine if the scene is deletable.
    ///     If the scene is deletable, you'll be able to delete the scene through SceneLaoder methods.
    ///     - Marked : when laoding process is asked, we give a boolean parameter to determine if the scene is marked.
    ///     If the scene is marked, you'll be able for example through SceneLoader unload's methods to unload all scenes that has been
    ///     loaded since a marked scene.
    ///
    /// To handle requests, SceneLoader use a queue. When load or unload of scene(s) is requested from the user, those request
    /// are wrapped as Process which are enqueue and handle one after one in the Update method when the SceneLoader is free.
    /// 
    /// To handle loaded scene, SceneLoader use a list which is mainly use as a stack. Indeed, we are using a list because we also need to be able
    /// to remove a scene in the middle of the list if the class's user request it by removing a specific case. but in mostly case, we use the last as a stack because the
    /// first element will remains unchanged during all the game lifecycle, and user will mostly unload the last loaded scene to load a new one.
    /// 
    /// The SceneLoader exclusively use C# events to communicate progress of requested process (process lifecycle, scene loaded and unloaded).
    /// NOTA BENE : SceneLoader will automatically add the scene which contains this script as the first loaded scene not deletable.
    /// This class will ensure in any circumstance that this very first scene is never remove and unload.
    /// </summary>
    internal class SceneLoader : MonoBehaviour
    {
        //=============================================================================//
        //============ Serialized Fields
        //=============================================================================//
        #region Serialized Fields
        [SerializeField] private float _minimumLoadingTime = 0.5f;
        #endregion
        
        //=============================================================================//
        //============ Internal Fields
        //=============================================================================//
        #region Internal Fields
        private SceneLoaderProcess _currentProcess;
        private SceneCollection _collection;
        #endregion
        
        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public bool IsBusy => _currentProcess != null; 
        internal SceneCollection Collection => _collection;
        #endregion
        
        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        
        /// <summary>
        /// Unity's Awake lifecycle
        /// Will initialize all member's variables and always add the scene containing SceneLoader as the first scene (this one will not be delete in any case)
        /// </summary>
        private void Awake()
        {
            _currentProcess = null;
            _collection = new SceneCollection();

            InternalSceneData loaderScene =  new InternalSceneData(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name, true, false, true, "MainSceneBundle", true);
            _collection.AddScene(loaderScene);
        }

        /// <summary>
        /// Unity's Update lifecycle
        /// Will manage the process queue by dequeuing when the scene loader is free and not handling a process.
        /// When a scene loader is handling a process, it will manage event of Start and Finish of the process.
        /// It will internally call update on the current process. 
        /// </summary>
        private void Update()
        {
            // If there is a process currently handle call the update on it
            // If the current process has ended, call the event to specify that the process ended
            // and set the process currently handed to null (allowing the scene loaded to handle a new one)
            if (_currentProcess != null)
            {
                _currentProcess.UpdateProcess();

                if (_currentProcess.ProcessFinish)
                {
                    SceneLoaderEvents.TriggerOnLoaderProcessFinish();
                    _currentProcess = null;
                }
            }
        }
        #endregion

        //=============================================================================//
        //============ Internal Methods
        //=============================================================================//
        #region Internal Methods
        
        internal void AddSceneToLoadedScene(InternalSceneData scene)
        {
            _collection.AddScene(scene);
        }
        
        internal void RemoveSceneFromLoadedScene(string sceneName)
        {
            _collection.RemoveSceneByName(sceneName);
        }
        
        internal void SendRequest(Queue<InternalSceneRequest> requests)
        {
            _currentProcess = new SceneLoaderProcess(_minimumLoadingTime, this, requests);
            SceneLoaderEvents.TriggerOnLoaderProcessStart();
        }
        
        #endregion
    }
}