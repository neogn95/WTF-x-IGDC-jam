using UnityEngine;
using System.IO;


public class LevelLoader : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    public GameObject objectPrefab;
    public GameObject goalPrefab;
    public GameObject playerPrefab;

    public void LoadLevel(string levelName)
    {
        string path = Path.Combine(Application.dataPath, "Levels", levelName + ".json");
        string json = File.ReadAllText(path);
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

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
        return LevelTileHelper.GetTilePrefab(tileChar, wallPrefab, floorPrefab, objectPrefab, goalPrefab, playerPrefab);
    }
}