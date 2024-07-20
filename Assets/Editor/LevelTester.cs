using UnityEngine;
using UnityEditor;
using System.IO;

public class LevelTester : EditorWindow
{
    private string levelName = "";
    private LevelLoader levelLoader;

    [MenuItem("Window/Level Tester")]
    public static void ShowWindow()
    {
        GetWindow<LevelTester>("Level Tester");
    }

    void OnGUI()
    {
        GUILayout.Label("Level Tester", EditorStyles.boldLabel);

        levelName = EditorGUILayout.TextField("Level Name", levelName);

        if (GUILayout.Button("Load Level"))
        {
            LoadLevelInEditor();
        }

        if (levelLoader == null)
        {
            EditorGUILayout.HelpBox("Please assign a LevelLoader in the scene.", MessageType.Warning);
        }
    }

    private void LoadLevelInEditor()
    {
        if (string.IsNullOrEmpty(levelName))
        {
            EditorUtility.DisplayDialog("Error", "Please enter a level name.", "OK");
            return;
        }

        if (levelLoader == null)
        {
            levelLoader = FindObjectOfType<LevelLoader>();
            if (levelLoader == null)
            {
                EditorUtility.DisplayDialog("Error", "LevelLoader not found in the scene.", "OK");
                return;
            }
        }

        string path = Path.Combine(Application.dataPath, "Levels", levelName + ".json");

        if (!File.Exists(path))
        {
            EditorUtility.DisplayDialog("Error", $"Level file not found: {path}", "OK");
            return;
        }

        string json = File.ReadAllText(path);
        JSONData jsonData = JsonUtility.FromJson<JSONData>(json);

        LevelData levelData = new LevelData
        {
            width = jsonData.width,
            height = jsonData.height,
            tiles = string.Join("", jsonData.tiles).ToCharArray()
        };

        ClearExistingLevel();
        LoadLevel(levelData);

        SceneView.RepaintAll();
    }

    private void ClearExistingLevel()
    {
        GameObject[] levelObjects = GameObject.FindGameObjectsWithTag("LevelObject");
        foreach (GameObject obj in levelObjects)
        {
            Undo.DestroyObjectImmediate(obj);
        }
    }

    private void LoadLevel(LevelData levelData)
    {
        for (int y = 0; y < levelData.height; y++)
        {
            for (int x = 0; x < levelData.width; x++)
            {
                int index = y * levelData.width + x;
                Vector3 position = new Vector3(x, 0, y);

                GameObject tilePrefab = GetTilePrefab(levelData.tiles[index]);
                GameObject tileObject = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
                tileObject.transform.position = position;   
                tileObject.tag = "LevelObject";
                Undo.RegisterCreatedObjectUndo(tileObject, "Create Level Object");

                if (tilePrefab != levelLoader.floorPrefab)
                {
                    GameObject floorObject = PrefabUtility.InstantiatePrefab(levelLoader.floorPrefab) as GameObject;
                    floorObject.transform.position = position;
                    floorObject.tag = "LevelObject";
                    Undo.RegisterCreatedObjectUndo(floorObject, "Create Floor Object");
                }
            }
        }
    }

    private GameObject GetTilePrefab(char tileChar)
    {
        return LevelTileHelper.GetTilePrefab(tileChar, levelLoader.wallPrefab, levelLoader.waterPrefab, levelLoader.floorPrefab,
            levelLoader.objectPrefab, levelLoader.goalPrefab, levelLoader.playerPrefab,
            levelLoader.bushPrefab, levelLoader.logPrefab, levelLoader.pillarPrefab,
            levelLoader.rockPrefab, levelLoader.stumpPrefab, levelLoader.treeBPrefab, levelLoader.treeGPrefab);
    }
}