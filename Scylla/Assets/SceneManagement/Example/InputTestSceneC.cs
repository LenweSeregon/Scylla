using System.Collections;
using System.Collections.Generic;
using Scylla.SceneManagement;
using UnityEngine;

public class InputTestSceneC : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SceneLoaderRequest request = new SceneLoaderRequestUnload(new UnloadStrategyAll());
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.PostRequest(request);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SceneLoaderRequest request = new SceneLoaderRequestUnload(new UnloadStrategyLastScene());
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.PostRequest(request);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SceneLoaderRequest request = new SceneLoaderRequestUnload(new UnloadStrategySimple("SceneTestC"));
            Scylla.SceneManagement.SceneManager.Instance.SceneLoaderProxy.PostRequest(request);
        }
    }
}
