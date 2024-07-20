using UnityEngine;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [System.Serializable]
    public class MenuEntry
    {
        public string menuName;
        public GameObject menuObject;
    }

    public List<MenuEntry> menus;
    private Stack<string> menuHistory = new Stack<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenMenu(string menuName)
    {
        if (menuHistory.Count > 0)
        {
            string currentMenu = menuHistory.Peek();
            GetMenu(currentMenu).SetActive(false);
        }

        GetMenu(menuName).SetActive(true);
        menuHistory.Push(menuName);
    }

    public void CloseMenu()
    {
        if (menuHistory.Count > 0)
        {
            string currentMenu = menuHistory.Pop();
            GetMenu(currentMenu).SetActive(false);

            if (menuHistory.Count > 0)
            {
                string previousMenu = menuHistory.Peek();
                GetMenu(previousMenu).SetActive(true);
            }
        }
    }

    public void CloseAllMenus()
    {
        while (menuHistory.Count > 0)
        {
            string currentMenu = menuHistory.Pop();
            GetMenu(currentMenu).SetActive(false);
        }
    }

    private GameObject GetMenu(string menuName)
    {
        MenuEntry entry = menus.Find(m => m.menuName == menuName);
        if (entry != null)
        {
            return entry.menuObject;
        }
        else
        {
            Debug.LogWarning($"Menu '{menuName}' not found!");
            return null;
        }
    }
}