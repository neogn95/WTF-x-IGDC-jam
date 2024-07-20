using UnityEngine;
using System.IO;


public class LevelLoader : MonoBehaviour
{
    public GameObject wallPrefab;
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

    public void LoadLevel(string levelName)
    {
        string path = Path.Combine(Application.dataPath, "Levels", levelName + ".json");
        string json = File.ReadAllText(path);
        // Parse the JSON manually
        JSONData jsonData = JsonUtility.FromJson<JSONData>(json);

        // Create a LevelData object and populate it
        LevelData levelData = new LevelData
        {
            width = jsonData.width,
            height = jsonData.height,
            tiles = string.Join("", jsonData.tiles).ToCharArray() // Convert string array to single string
        };

        // Clear existing level
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new level
        for (int y = 0; y < levelData.height; y++)
        {
            for (int x = 0; x < levelData.width; x++)
            {
                int index = y * levelData.width + x;
                Vector3 position = new Vector3(x, 0, y);

                GameObject tilePrefab = GetTilePrefab(levelData.tiles[index]);
                Instantiate(tilePrefab, position, Quaternion.identity, transform);

                // Always place a floor tile
                if (tilePrefab != floorPrefab)
                {
                    Instantiate(floorPrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }

    private GameObject GetTilePrefab(char tileChar)
    {
        return LevelTileHelper.GetTilePrefab(tileChar, wallPrefab, floorPrefab, objectPrefab, goalPrefab, playerPrefab, 
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