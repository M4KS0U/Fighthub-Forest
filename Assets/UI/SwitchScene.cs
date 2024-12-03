using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScene : MonoBehaviour
{
    public string sceneName;

    public void Switch()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
