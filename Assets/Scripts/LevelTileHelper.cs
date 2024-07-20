using UnityEngine;

public static class LevelTileHelper
{
    public const char Wall = 'W';
    public const char Floor = 'F';
    public const char Object = 'O';
    public const char Goal = 'G';
    public const char Player = 'P';

    public static char GetTileChar(GameObject tile, GameObject wallPrefab, GameObject floorPrefab, GameObject objectPrefab, GameObject goalPrefab, GameObject playerPrefab)
    {
        if (tile == wallPrefab) return Wall;
        if (tile == floorPrefab) return Floor;
        if (tile == objectPrefab) return Object;
        if (tile == goalPrefab) return Goal;
        if (tile == playerPrefab) return Player;
        return Floor; // Default to floor
    }

    public static GameObject GetTilePrefab(char tileChar, GameObject wallPrefab, GameObject floorPrefab, GameObject objectPrefab, GameObject goalPrefab, GameObject playerPrefab)
    {
        switch (tileChar)
        {
            case Wall: return wallPrefab;
            case Floor: return floorPrefab;
            case Object: return objectPrefab;
            case Goal: return goalPrefab;
            case Player: return playerPrefab;
            default: return floorPrefab;
        }
    }
}