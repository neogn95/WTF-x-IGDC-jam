using UnityEngine;
using System.IO;
using UnityEditor;


public class LevelLoader : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject waterPrefab;
    public GameObject floorPrefab;
    public GameObject objectPrefab;
    public GameObject goalPrefab;
    public GameObject playerPrefab;
    public GameObject bushPrefab;
    public GameObject logPrefab;
    public GameObject pillarPrefab;
    public GameObject rockPrefab;
    public GameObject stumpPrefab;
    public GameObject treeBPrefab;
    public GameObject treeGPrefab;


    private void OnEnable()
    {
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
    }

    public void LoadLevel(string levelName)
    {
        string path = Path.Combine(Application.dataPath, "Levels", levelName + ".json");
        string json = File.ReadAllText(path);
        JSONData jsonData = JsonUtility.FromJson<JSONData>(json);

        LevelData levelData = new LevelData
        {
            width = jsonData.width,
            height = jsonData.height,
            tiles = string.Join("", jsonData.tiles).ToCharArray()
        };

        // Clear existing level
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Calculate the offset to center the grid
        float offsetX = -levelData.width / 2f + 0.5f;
        float offsetY = -levelData.height / 2f + 0.5f;

        // Instantiate new level
        for (int y = 0; y < levelData.height; y++)
        {
            for (int x = 0; x < levelData.width; x++)
            {
                int index = y * levelData.width + x;
                GameObject tilePrefab = GetTilePrefab(levelData.tiles[index]);

                // Determine the Z position (height) based on the tile type
                float z = (tilePrefab.name == "Floor" || tilePrefab.name == "Water") ? 0f : 1f;

                // Apply the offset to center the grid
                Vector3 position = new Vector3(x + offsetX, z, y + offsetY);

                Instantiate(tilePrefab, position, Quaternion.identity, transform);

                // Always place a floor tile if the current tile is not Water or Floor
                if (tilePrefab.name != "Water" && tilePrefab != floorPrefab)
                {
                    Vector3 floorPosition = new Vector3(x + offsetX, 0, y + offsetY);
                    Instantiate(floorPrefab, floorPosition, Quaternion.identity, transform);
                }
            }
        }
    }

    private GameObject GetTilePrefab(char tileChar)
    {
        return LevelTileHelper.GetTilePrefab(tileChar, wallPrefab, waterPrefab, floorPrefab, objectPrefab, goalPrefab, playerPrefab, 
            bushPrefab, logPrefab, pillarPrefab, rockPrefab, stumpPrefab, treeBPrefab, treeGPrefab);
    }                                                                                                                   
}                                                                                                                       
                                                                                                                        
[System.Serializable]                                                                                                   
public class JSONData                                                                                                 
{
    public int width;
    public int height;
    public string[] tiles;
}