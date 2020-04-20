namespace Scylla
{
    using Scylla.SceneManagement;
    using UnityEngine.UI;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class StandardSceneLoaderGUI : SceneLoaderGUI
    {
        [SerializeField] private Slider _slider = null;
        [SerializeField] private TextMeshProUGUI _textChunk = null;
        
        protected override void OnLoaderProcessUpdate(string description, float progress)
        {
            _slider.value = progress;
            _textChunk.text = description;
        }
    }
}

