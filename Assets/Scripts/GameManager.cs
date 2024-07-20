using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public List<string> levelNames = new List<string>();
    private int currentLevelIndex = -1;
    private LevelLoader levelLoader;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadMainMenu();
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            levelLoader = FindObjectOfType<LevelLoader>();
            if (levelLoader == null)
            {
                Debug.LogError("LevelLoader not found in GameScene!");
            }
            else
            {
                StartGame();
            }
        }
    }

    public void StartGame()
    {
        currentLevelIndex = -1;
        LoadNextLevel();
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelNames.Count)
        {
            if (SceneManager.GetActiveScene().name != "GameScene")
            {
                SceneManager.LoadScene("GameScene");
            }
            else
            {
                LoadLevel(levelNames[currentLevelIndex]);
            }
        }
        else
        {
            Debug.Log("All levels completed!");
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void LoadLevel(string levelName)
    {
        if (levelLoader != null)
        {
            levelLoader.LoadLevel(levelName);
        }
        else
        {
            Debug.LogError("LevelLoader is not set!");
        }
    }
}