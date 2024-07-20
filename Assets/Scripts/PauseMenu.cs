using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void ResumeGame()
    {
        // Resume game logic here
        MenuManager.Instance.CloseMenu();
    }

    public void OpenOptions()
    {
        MenuManager.Instance.OpenMenu("OptionsMenu");
    }

    public void QuitToMainMenu()
    {
        // Logic to return to main menu
        MenuManager.Instance.CloseAllMenus();
        MenuManager.Instance.OpenMenu("MainMenu");
    }
}
