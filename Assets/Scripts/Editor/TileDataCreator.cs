using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

public class TileDataCreator : MonoBehaviour
{
    [MenuItem("711Route/Create Tile Set")]
    public static void CreateTileSet()
    {
        string path = "Assets/TileData/";
        
        if (!AssetDatabase.IsValidFolder("Assets/TileData"))
        {
            AssetDatabase.CreateFolder("Assets", "TileData");
        }
        
        CreateTile("Straight", TileType.Straight, new bool[] { true, false, true, false }, path);
        CreateTile("Turn", TileType.Turn, new bool[] { true, true, false, false }, path);
        CreateTile("TJunction", TileType.TJunction, new bool[] { true, true, true, false }, path);
        CreateTile("Crossroad", TileType.Crossroad, new bool[] { true, true, true, true }, path);
        CreateTile("DeadEnd", TileType.DeadEnd, new bool[] { true, false, false, false }, path);
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log("Tile set created in Assets/TileData/");
    }
    
    private static void CreateTile(string name, TileType type, bool[] exits, string path)
    {
        TileData tile = ScriptableObject.CreateInstance<TileData>();
        tile.tileType = type;
        tile.roadExits = exits;
        
        AssetDatabase.CreateAsset(tile, path + name + ".asset");
    }
}
#endif
