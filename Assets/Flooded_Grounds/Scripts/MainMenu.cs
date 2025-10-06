using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Debug.Log("MainMenu Start");
        Cursor.visible = true;
    	Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        Debug.Log("StartGame");
        SceneManager.LoadScene("Scene_A");
    }

    public void ExitGame()
    {
        Debug.Log("ExitGame");
        Application.Quit();
    }
}
