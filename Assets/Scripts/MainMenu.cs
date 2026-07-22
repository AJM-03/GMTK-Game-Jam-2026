using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

public void PlayButton()
    {
        SceneManager.LoadScene("MainScene");
    }

public void OptionsButton()
    {
        SceneManager.LoadScene("Options");
    }

public void QuitButton()
    {
        Application.Quit();
    }

    public void MenuButton()
    {
        Debug.Log("a");
        SceneManager.LoadScene("Menu");
    }
}
