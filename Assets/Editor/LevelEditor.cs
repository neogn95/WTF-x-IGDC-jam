using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class LevelEditor : EditorWindow
{
    private int gridWidth = 5;
    private int gridHeight = 5;
    private float cellSize = 50f;
    private GameObject[,] grid;
    private Vector2 scrollPosition;

    // Prefabs for different tile types
    private GameObject wallPrefab;
    private GameObject floorPrefab;
    private GameObject objectPrefab;
    private GameObject goalPrefab;
    private GameObject playerPrefab;

    private GameObject currentTile;
    private string levelName = "New Level";

    [MenuItem("Window/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditor>("Level Editor");
    }

    private void OnEnable()
    {
        // Load prefabs
        wallPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Wall.prefab");
        floorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Floor.prefab");
        objectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Object.prefab");
        goalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Goal.prefab");
        playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Player.prefab");

        // Initialize the grid
        InitializeGrid();
    }

    private void OnGUI()
    {
        GUILayout.Label("Level Editor", EditorStyles.boldLabel);

        // Grid size settings
        EditorGUILayout.BeginHorizontal();
        gridWidth = EditorGUILayout.IntField("Grid Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Grid Height", gridHeight);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Resize Grid"))
        {
            InitializeGrid();
            Repaint();
        }

        // Tile selection
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Wall")) SetCurrentTile(wallPrefab);
        if (GUILayout.Button("Floor")) SetCurrentTile(floorPrefab);
        if (GUILayout.Button("Object")) SetCurrentTile(objectPrefab);
        if (GUILayout.Button("Goal")) SetCurrentTile(goalPrefab);
        if (GUILayout.Button("Player")) SetCurrentTile(playerPrefab);
        EditorGUILayout.EndHorizontal();

        // Level name input
        levelName = EditorGUILayout.TextField("Level Name", levelName);

        // Save and load buttons
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Level")) SaveLevel();
        if (GUILayout.Button("Load Level")) LoadLevel();
        EditorGUILayout.EndHorizontal();

        // Grid view
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        DrawGrid();
        EditorGUILayout.EndScrollView();

        // Handle mouse input for painting tiles
        HandleMouseInput();

        // Ensure the window is redrawn continuously
        Repaint();
    }

    private void SetCurrentTile(GameObject tile)
    {
        currentTile = tile;
        Repaint();
    }

    private void InitializeGrid()
    {
        grid = new GameObject[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                grid[x, y] = floorPrefab; // Initialize with floor tiles
            }
        }
    }

    private void DrawGrid()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < gridWidth; x++)
            {
                if (grid[x, y] != null)
                {
                    Texture2D preview = AssetPreview.GetAssetPreview(grid[x, y]);
                    GUILayout.Box(preview, GUILayout.Width(cellSize), GUILayout.Height(cellSize));
                }
                else
                {
                    GUILayout.Box("", GUILayout.Width(cellSize), GUILayout.Height(cellSize));
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void HandleMouseInput()
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Vector2 mousePos = e.mousePosition;
            mousePos.y -= EditorGUIUtility.singleLineHeight * 7; // Adjust for UI elements above
            Vector2 gridPos = new Vector2(
                Mathf.Floor(mousePos.x / cellSize),
                Mathf.Floor(mousePos.y / cellSize)
            );

            if (gridPos.x >= 0 && gridPos.x < gridWidth && gridPos.y >= 0 && gridPos.y < gridHeight)
            {
                PlaceTile((int)gridPos.x, (int)gridPos.y);
                e.Use(); // Consume the event
            }
        }
    }

    private void PlaceTile(int x, int y)
    {
        if (currentTile != null)
        {
            grid[x, y] = currentTile;
            Repaint();
        }
    }

    private void SaveLevel()
    {
        string path = EditorUtility.SaveFilePanel("Save Level", "Assets/Levels", levelName, "json");
        if (!string.IsNullOrEmpty(path))
        {
            LevelData levelData = new LevelData
            {
                width = gridWidth,
                height = gridHeight,
                tiles = new string[gridWidth * gridHeight]
            };

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int index = y * gridWidth + x;
                    levelData.tiles[index] = GetTileChar(grid[x, y]);
                }
            }

            string json = JsonUtility.ToJson(levelData, true);
            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }
    }

    private void LoadLevel()
    {
        string path = EditorUtility.OpenFilePanel("Load Level", "Assets/Levels", "json");
        if (!string.IsNullOrEmpty(path))
        {
            string json = File.ReadAllText(path);
            LevelData levelData = JsonUtility.FromJson<LevelData>(json);

            gridWidth = levelData.width;
            gridHeight = levelData.height;
            InitializeGrid();

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int index = y * gridWidth + x;
                    grid[x, y] = GetTilePrefab(levelData.tiles[index]);
                }
            }

            levelName = Path.GetFileNameWithoutExtension(path);
            Repaint();
        }
    }

    private string GetTileChar(GameObject tile)
    {
        if (tile == wallPrefab) return "W";
        if (tile == floorPrefab) return "F";
        if (tile == objectPrefab) return "O";
        if (tile == goalPrefab) return "G";
        if (tile == playerPrefab) return "P";
        return "F"; // Default to floor
    }

    private GameObject GetTilePrefab(string tileChar)
    {
        switch (tileChar)
        {
            case "W": return wallPrefab;
            case "F": return floorPrefab;
            case "O": return objectPrefab;
            case "G": return goalPrefab;
            case "P": return playerPrefab;
            default: return floorPrefab;
        }
    }
}