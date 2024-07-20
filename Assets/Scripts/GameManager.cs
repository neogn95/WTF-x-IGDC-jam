using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Singleton instance
    public static GameManager Instance { get; private set; }

    // Level management
    public LevelLoader levelLoader;
    public List<string> levelNames = new List<string>();
    private int currentLevelIndex = -1;

    // Game state
    public enum GameState { MainMenu, Playing, Paused, GameOver }
    public GameState CurrentState { get; private set; }

    // Events
    public delegate void GameStateChangedHandler(GameState newState);
    public event GameStateChangedHandler OnGameStateChanged;

    public delegate void LevelLoadedHandler(string levelName);
    public event LevelLoadedHandler OnLevelLoaded;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize components
        if (levelLoader == null)
        {
            levelLoader = GetComponent<LevelLoader>();
            if (levelLoader == null)
            {
                levelLoader = gameObject.AddComponent<LevelLoader>();
            }
        }

        // Set initial game state
        SetGameState(GameState.MainMenu);
    }

    public void StartGame()
    {
        currentLevelIndex = -1;
        LoadNextLevel();
        SetGameState(GameState.Playing);
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex < levelNames.Count)
        {
            LoadLevel(levelNames[currentLevelIndex]);
        }
        else
        {
            Debug.Log("All levels completed!");
            SetGameState(GameState.GameOver);
        }
    }

    public void LoadLevel(string levelName)
    {
        levelLoader.LoadLevel(levelName);
        OnLevelLoaded?.Invoke(levelName);
    }

    public void RestartLevel()
    {
        LoadLevel(levelNames[currentLevelIndex]);
    }

    public void PauseGame()
    {
        SetGameState(GameState.Paused);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        SetGameState(GameState.Playing);
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        SetGameState(GameState.GameOver);
    }

    public void ReturnToMainMenu()
    {
        SetGameState(GameState.MainMenu);
        SceneManager.LoadScene("MainMenu"); // Assuming you have a MainMenu scene
    }

    private void SetGameState(GameState newState)
    {
        CurrentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    // Add more game-specific methods here as needed
}