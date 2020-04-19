using System.Collections;
using System.Collections.Generic;
using Scylla;
using Scylla.SceneManagement;
using UnityEngine;

public class InputTestSceneA : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneLoaderRequest request = new SceneLoaderRequestLoad("SceneTestB", new UnloadStrategyNone());
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.PostRequest(request);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneLoaderRequest request = new SceneLoaderRequestLoad("SceneTestB", new UnloadStrategyLastScene());
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.PostRequest(request);
        }
    }
}
