using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneReset : MonoBehaviour
{
    public KeyCode RESET_SCENE = KeyCode.F5;
    void Update()
    {
        if (Input.GetKeyDown(RESET_SCENE)) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
