using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        // Start game logic here
        MenuManager.Instance.CloseAllMenus();
    }

    public void OpenOptions()
    {
        MenuManager.Instance.OpenMenu("OptionsMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
