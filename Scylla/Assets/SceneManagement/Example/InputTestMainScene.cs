using System.Collections;
using System.Collections.Generic;
using Scylla.SceneManagement;
using UnityEngine;

public class InputTestMainScene : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneLoaderRequest request = new SceneLoaderRequestLoad("SceneTestA", new UnloadStrategyNone());
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.PostRequest(request);
        }
    }
}
