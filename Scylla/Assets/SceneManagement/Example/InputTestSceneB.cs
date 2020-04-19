using System.Collections;
using System.Collections.Generic;
using Scylla;
using Scylla.SceneManagement;
using UnityEngine;

public class InputTestSceneB : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneLoaderRequest request = new SceneLoaderRequestLoad("SceneTestA", new UnloadStrategyAll());
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.PostRequest(request);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneLoaderRequest request = new SceneLoaderRequestLoad("SceneTestC", new UnloadStrategySimple("SceneTestA"));
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.PostRequest(request);
        }
    }
}
