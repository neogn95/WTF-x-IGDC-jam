using UnityEngine;
using UnityEditor;
using System.IO;
using Unity.Plastic.Newtonsoft.Json;

public class LevelEditor : EditorWindow
{
    private int gridWidth = 5;
    private int gridHeight = 5;
    private float cellSize = 50f;
    private GameObject[,] grid;
    private Vector2 scrollPosition;

    // Prefabs for different tile types
    private GameObject waterPrefab;
    private GameObject wallPrefab;
    private GameObject floorPrefab;
    private GameObject objectPrefab;
    private GameObject goalPrefab;
    private GameObject playerPrefab;
    private GameObject bushPrefab;
    private GameObject logPrefab;
    private GameObject pillarPrefab;
    private GameObject rockPrefab;
    private GameObject stumpPrefab;
    private GameObject treeBPrefab;
    private GameObject treeGPrefab;

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
        waterPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Water.prefab");
        floorPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Floor.prefab");
        objectPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Object.prefab");
        goalPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Goal.prefab");
        bushPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Bush.prefab");
        logPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Log.prefab");
        pillarPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Pillar.prefab");
        rockPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Rock.prefab");
        stumpPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Stump.prefab");
        treeBPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tree_Brown.prefab");
        treeGPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tree_Green.prefab");

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
        if (GUILayout.Button("Water")) SetCurrentTile(waterPrefab);
        if (GUILayout.Button("Floor")) SetCurrentTile(floorPrefab);
        if (GUILayout.Button("Object")) SetCurrentTile(objectPrefab);
        if (GUILayout.Button("Goal")) SetCurrentTile(goalPrefab);
        if (GUILayout.Button("Player")) SetCurrentTile(playerPrefab);
        if (GUILayout.Button("Bush")) SetCurrentTile(bushPrefab);
        if (GUILayout.Button("Log")) SetCurrentTile(logPrefab);
        if (GUILayout.Button("Pillar")) SetCurrentTile(pillarPrefab);
        if (GUILayout.Button("Rock")) SetCurrentTile(rockPrefab);
        if (GUILayout.Button("Stump")) SetCurrentTile(stumpPrefab);
        if (GUILayout.Button("Tree Brown")) SetCurrentTile(treeBPrefab);
        if (GUILayout.Button("Tree Green")) SetCurrentTile(treeGPrefab);
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
                grid[x, y] = waterPrefab; // Initialize with water tiles
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
            // Check if the current tile is an object that should be placed on top of the floor
            if (IsObjectTile(currentTile))
            {
                // If the current tile at this position is not a floor, place a floor first
                if (grid[x, y] != waterPrefab)
                {
                    grid[x, y] = waterPrefab;
                }
                // Place the object on top of the floor
                grid[x, y] = currentTile;
            }
            else
            {
                // For other tiles (like walls), just place them directly
                grid[x, y] = currentTile;
            }
            Repaint();
        }
    }

    // Helper method to check if a tile is an object that should be placed on top of the floor
    private bool IsObjectTile(GameObject tile)
    {
        return tile == bushPrefab || tile == logPrefab || tile == pillarPrefab ||
               tile == rockPrefab || tile == stumpPrefab || tile == treeBPrefab ||
               tile == treeGPrefab || tile == objectPrefab || tile == goalPrefab ||
               tile == playerPrefab;
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
                tiles = new char[gridWidth * gridHeight]
            };

            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    int index = y * gridWidth + x;
                    levelData.tiles[index] = GetTileChar(grid[x, y]);
                }
            }

            string json = JsonConvert.SerializeObject(levelData, Formatting.Indented);
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
            JSONData jsonData = JsonUtility.FromJson<JSONData>(json);

            LevelData levelData = new()
            {
                width = jsonData.width,
                height = jsonData.height,
                tiles = string.Join("", jsonData.tiles).ToCharArray()
            };

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

    private char GetTileChar(GameObject tile)
    {
        return LevelTileHelper.GetTileChar(tile, wallPrefab, waterPrefab, floorPrefab, objectPrefab, goalPrefab, playerPrefab,
            bushPrefab, logPrefab, pillarPrefab, rockPrefab, stumpPrefab, treeBPrefab, treeGPrefab);
    }

    private GameObject GetTilePrefab(char tileChar)
    {
        return LevelTileHelper.GetTilePrefab(tileChar, wallPrefab, waterPrefab, floorPrefab, objectPrefab, goalPrefab, playerPrefab, 
            bushPrefab, logPrefab, pillarPrefab, rockPrefab, stumpPrefab, treeBPrefab, treeGPrefab);
    }
}