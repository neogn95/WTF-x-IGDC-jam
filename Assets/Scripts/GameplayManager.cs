using UnityEngine;
using System.Collections.Generic;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public GameObject playerPrefab;
    public GameObject logPrefab;
    public GameObject stumpPrefab;

    private GameObject[,] grid;
    private Vector2Int gridSize;
    private Vector2Int playerPosition;

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
        }
    }

    public void InitializeGrid(int width, int height)
    {
        gridSize = new Vector2Int(width, height);
        grid = new GameObject[width, height];
        // Initialize grid with null values
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = null;
            }
        }
    }

    public void SetGridObject(int x, int y, GameObject obj)
    {
        if (IsValidGridPosition(x, y))
        {
            grid[x, y] = obj;
        }
    }

    public GameObject GetGridObject(int x, int y)
    {
        if (IsValidGridPosition(x, y))
        {
            return grid[x, y];
        }
        return null;
    }

    public void SetPlayerPosition(int x, int y)
    {
        playerPosition = new Vector2Int(x, y);
        SetGridObject(x, y, playerPrefab);
    }

    public bool TryMovePlayer(Vector2Int currentPosition, Vector2Int direction)
    {
        Vector2Int newPosition = currentPosition + direction;

        if (!IsValidGridPosition(newPosition.x, newPosition.y))
        {
            return false;
        }

        GameObject targetObject = GetGridObject(newPosition.x, newPosition.y);

        if (targetObject == null)
        {
            // Move player to empty space
            SetGridObject(currentPosition.x, currentPosition.y, null);
            SetGridObject(newPosition.x, newPosition.y, playerPrefab);
            playerPosition = newPosition;
            return true;
        }
        else if (targetObject.CompareTag("LevelObject"))
        {
            // Check for specific components
            TreeBehavior treeBehavior = targetObject.GetComponent<TreeBehavior>();
            if (treeBehavior != null)
            {
                return HandleTreeInteraction(newPosition, direction);
            }

            LogBehavior logBehavior = targetObject.GetComponent<LogBehavior>();
            if (logBehavior != null)
            {
                return HandleLogInteraction(newPosition, direction);
            }
        }

        return false;
    }

    private bool HandleTreeInteraction(Vector2Int treePosition, Vector2Int direction)
    {
        GameObject treeObject = GetGridObject(treePosition.x, treePosition.y);
        TreeBehavior treeBehavior = treeObject.GetComponent<TreeBehavior>();

        if (treeBehavior != null)
        {
            treeBehavior.Interact(direction);

            // Move player
            SetGridObject(playerPosition.x, playerPosition.y, null);
            SetPlayerPosition(treePosition.x, treePosition.y);

            return true;
        }

        return false;
    }

    private bool HandleLogInteraction(Vector2Int logPosition, Vector2Int direction)
    {
        GameObject logObject = GetGridObject(logPosition.x, logPosition.y);
        LogBehavior logBehavior = logObject.GetComponent<LogBehavior>();

        if (logBehavior != null)
        {
            if (direction.x != 0) // Horizontal push
            {
                logBehavior.Roll(direction);
            }
            else // Vertical push
            {
                logBehavior.Rotate(direction);
            }

            // Move player
            SetGridObject(playerPosition.x, playerPosition.y, null);
            SetPlayerPosition(logPosition.x, logPosition.y);

            return true;
        }

        return false;
    }

    private bool RollLog(Vector2Int logPosition, Vector2Int direction)
    {
        Vector2Int nextPosition = logPosition + direction;

        while (IsValidGridPosition(nextPosition.x, nextPosition.y) && GetGridObject(nextPosition.x, nextPosition.y) == null)
        {
            nextPosition += direction;
        }

        nextPosition -= direction; // Move back one step

        if (nextPosition == logPosition)
        {
            return false; // Log couldn't move
        }

        // Move log
        GameObject log = GetGridObject(logPosition.x, logPosition.y);
        SetGridObject(logPosition.x, logPosition.y, null);
        SetGridObject(nextPosition.x, nextPosition.y, log);
        log.transform.position = GridToWorldPosition(nextPosition);

        // Move player
        SetGridObject(playerPosition.x, playerPosition.y, null);
        SetPlayerPosition(logPosition.x, logPosition.y);

        return true;
    }

    private bool RotateLog(Vector2Int logPosition, Vector2Int direction)
    {
        Vector2Int nextPosition = logPosition + direction;

        if (!IsValidGridPosition(nextPosition.x, nextPosition.y) || GetGridObject(nextPosition.x, nextPosition.y) != null)
        {
            return false;
        }

        // Rotate log
        GameObject log = GetGridObject(logPosition.x, logPosition.y);
        SetGridObject(logPosition.x, logPosition.y, null);
        SetGridObject(nextPosition.x, nextPosition.y, log);
        log.transform.position = GridToWorldPosition(nextPosition);
        log.transform.rotation = Quaternion.Euler(0, 0, 90); // Rotate 90 degrees

        // Move player
        SetGridObject(playerPosition.x, playerPosition.y, null);
        SetPlayerPosition(logPosition.x, logPosition.y);

        return true;
    }

    private bool IsValidGridPosition(int x, int y)
    {
        return x >= 0 && x < gridSize.x && y >= 0 && y < gridSize.y;
    }

    private Vector3 GridToWorldPosition(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.y);
    }
}