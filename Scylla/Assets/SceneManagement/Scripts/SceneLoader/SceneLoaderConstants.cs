﻿namespace Scylla.SceneManagement
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public enum SceneLoaderReturnType
    {
        Accepted,
        LoadRejectedSceneExists,
        LoadRejectedBundleExists,
        LoadRejectedUnloadLastSceneNotSuppressible,
        LoadRejectedUnloadBundleNotSuppressible,
        LoadRejectedUnloadUntilMarkedNotSuppressible,
        LoadRejectedUnloadAllNotSuppressible,
        UnloadRejectedSceneNotExists,
        UnloadRejectedMultipleSameScenes
    }
    
    public class SceneLoaderRequestResult
    {
        //=============================================================================//
        //============ Non-Serialized Fields
        //=============================================================================//
        #region Non-Serialized Fields
        private int _returnCode;
        private string _returnMessaage;
        private bool _isSuccess;
        #endregion
        
        //=============================================================================//
        //============ Properties
        //=============================================================================//
        #region Properties
        public int ReturnCode => _returnCode;
        public string ReturnMessaage => _returnMessaage;
        public bool IsSuccess =>_isSuccess;
        #endregion

        //=============================================================================//
        //============ Lifecycle Methods
        //=============================================================================//
        #region Lifecycle Methods
        public SceneLoaderRequestResult(int returnCode, string returnMessage, bool isSuccess)
        {
            _returnCode = returnCode;
            _returnMessaage = returnMessage;
            _isSuccess = isSuccess;
        }
        #endregion
    }
    
    public static class SceneLoaderConstants
    {
        //=============================================================================//
        //============ Constantes
        //=============================================================================//
        #region Constantes
        public static readonly string BUNDLE_IDENTIFIER_BASIS = "Bundle";
        public static readonly Dictionary<SceneLoaderReturnType, SceneLoaderRequestResult> REQUEST_RESULTS = new Dictionary<SceneLoaderReturnType, SceneLoaderRequestResult>()
        {
            [SceneLoaderReturnType.Accepted] = new SceneLoaderRequestResult(0, "Request accepted", true),
            
            [SceneLoaderReturnType.LoadRejectedSceneExists] = new SceneLoaderRequestResult(10, "Load request rejected - Scene exists", false),
            [SceneLoaderReturnType.LoadRejectedBundleExists] = new SceneLoaderRequestResult(11, "Load request rejected - Bundle exists", false),
            [SceneLoaderReturnType.LoadRejectedUnloadLastSceneNotSuppressible] = new SceneLoaderRequestResult(12, "Load request rejected - Ask to unload last scene which is not suppressible", false), 
            [SceneLoaderReturnType.LoadRejectedUnloadBundleNotSuppressible] = new SceneLoaderRequestResult(13, "Load request rejected - Ask to unload last bundle which contains scenes not suppressible", false), 
            [SceneLoaderReturnType.LoadRejectedUnloadUntilMarkedNotSuppressible] = new SceneLoaderRequestResult(14, "Load request rejected - Ask to unload until marked which contains scenes not suppressible", false), 
            [SceneLoaderReturnType.LoadRejectedUnloadAllNotSuppressible] = new SceneLoaderRequestResult(15, "Load request rejected - Ask to unload all scenes which contains scenes not suppressible", false), 
            
            [SceneLoaderReturnType.UnloadRejectedSceneNotExists] = new SceneLoaderRequestResult(30, "Unload request rejected - Scene doesn't exists", false),
            [SceneLoaderReturnType.UnloadRejectedMultipleSameScenes] = new SceneLoaderRequestResult(31, "Unload request rejected - List to unload contains several time the same scene", false),
        };
        
        #endregion
    }
}

