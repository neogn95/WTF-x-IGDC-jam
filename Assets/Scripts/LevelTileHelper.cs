using UnityEngine;

public static class LevelTileHelper
{
    public const char Wall = 'W';
    public const char Floor = 'F';
    public const char Object = 'O';
    public const char Goal = 'G';
    public const char Player = 'P';
    public const char Bush = 'B';
    public const char Log = 'L';
    public const char Pillar = '5';
    public const char Rock = 'R';
    public const char Stump = 'S';
    public const char Tree = 'T';
    public const char Water = '6';

    public static char GetTileChar(GameObject tile, GameObject wallPrefab, GameObject waterPrefab, GameObject floorPrefab, GameObject objectPrefab, GameObject goalPrefab, GameObject playerPrefab,
        GameObject bushPrefab, GameObject logPrefab, GameObject pillarPrefab, GameObject rockPrefab, GameObject stumpPrefab, GameObject treeBPrefab, GameObject treeGPrefab)
    {
        if (tile == wallPrefab) return Wall;
        if (tile == waterPrefab) return Water;
        if (tile == floorPrefab) return Floor;
        if (tile == objectPrefab) return Object;
        if (tile == goalPrefab) return Goal;
        if (tile == playerPrefab) return Player;
        if (tile == bushPrefab) return Bush;
        if (tile == logPrefab) return Log;
        if (tile == pillarPrefab) return Pillar;
        if (tile == rockPrefab) return Rock;
        if (tile == stumpPrefab) return Stump;
        if (tile == treeBPrefab) return Tree;
        return Floor; // Default to floor
    }

    public static GameObject GetTilePrefab(char tileChar, GameObject wallPrefab, GameObject waterPrefab, GameObject floorPrefab, GameObject objectPrefab, GameObject goalPrefab, GameObject playerPrefab,
        GameObject bushPrefab, GameObject logPrefab, GameObject pillarPrefab, GameObject rockPrefab, GameObject stumpPrefab, GameObject treeBPrefab, GameObject treeGPrefab)
    {
        switch (tileChar)
        {
            case Wall: return wallPrefab;
            case Water: return waterPrefab;
            case Floor: return floorPrefab;
            case Object: return objectPrefab;
            case Goal: return goalPrefab;
            case Player: return playerPrefab;
            case Bush: return bushPrefab;
            case Log: return logPrefab;
            case Pillar: return pillarPrefab;
            case Rock: return rockPrefab;
            case Stump: return stumpPrefab;
            case Tree: return treeBPrefab;
            default: return floorPrefab;
        }
    }
}