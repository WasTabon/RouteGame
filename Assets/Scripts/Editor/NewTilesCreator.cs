using UnityEngine;
using UnityEditor;
using System.IO;

public class NewTilesCreator : EditorWindow
{
    private string spritesPath = "Assets/Sprites/Tiles";
    private string tilesDataPath = "Assets/ScriptableObjects/TileData";
    
    [MenuItem("Tools/711Route/Create New Tiles")]
    public static void ShowWindow()
    {
        GetWindow<NewTilesCreator>("New Tiles Creator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("New Tiles Creator", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "This tool will create TileData assets for the 6 new tile types:\n" +
            "• DoubleStraight\n" +
            "• STurn\n" +
            "• YJunction\n" +
            "• DiagonalCross\n" +
            "• TripleJunction\n" +
            "• Roundabout",
            MessageType.Info
        );
        
        GUILayout.Space(10);
        
        spritesPath = EditorGUILayout.TextField("Sprites Folder:", spritesPath);
        tilesDataPath = EditorGUILayout.TextField("TileData Output Folder:", tilesDataPath);
        
        GUILayout.Space(20);
        
        if (GUILayout.Button("Create New Tiles", GUILayout.Height(40)))
        {
            CreateNewTiles();
        }
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("Create All Tiles (Including Originals)", GUILayout.Height(30)))
        {
            CreateAllTiles();
        }
    }
    
    private void CreateNewTiles()
    {
        if (!Directory.Exists(tilesDataPath))
        {
            Directory.CreateDirectory(tilesDataPath);
        }
        
        CreateDoubleStraight();
        CreateSTurn();
        CreateYJunction();
        CreateDiagonalCross();
        CreateTripleJunction();
        CreateRoundabout();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Success", "New tiles created successfully!", "OK");
    }
    
    private void CreateAllTiles()
    {
        if (!Directory.Exists(tilesDataPath))
        {
            Directory.CreateDirectory(tilesDataPath);
        }
        
        CreateStraight();
        CreateTurn();
        CreateTJunction();
        CreateCrossroad();
        CreateDeadEnd();
        
        CreateDoubleStraight();
        CreateSTurn();
        CreateYJunction();
        CreateDiagonalCross();
        CreateTripleJunction();
        CreateRoundabout();
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Success", "All tiles created successfully!", "OK");
    }
    
    private void CreateStraight()
    {
        TileData tile = CreateTileAsset("Straight", TileType.Straight);
        tile.roadExits = new bool[] { true, false, true, false };
        EditorUtility.SetDirty(tile);
    }
    
    private void CreateTurn()
    {
        TileData tile = CreateTileAsset("Turn", TileType.Turn);
        tile.roadExits = new bool[] { true, true, false, false };
        EditorUtility.SetDirty(tile);
    }
    
    private void CreateTJunction()
    {
        TileData tile = CreateTileAsset("TJunction", TileType.TJunction);
        tile.roadExits = new bool[] { false, true, true, true };
        EditorUtility.SetDirty(tile);
    }
    
    private void CreateCrossroad()
    {
        TileData tile = CreateTileAsset("Crossroad", TileType.Crossroad);
        tile.roadExits = new bool[] { true, true, true, true };
        EditorUtility.SetDirty(tile);
    }
    
    private void CreateDeadEnd()
    {
        TileData tile = CreateTileAsset("DeadEnd", TileType.DeadEnd);
        tile.roadExits = new bool[] { true, false, false, false };
        EditorUtility.SetDirty(tile);
    }
    
    private void CreateDoubleStraight()
    {
        TileData tile = CreateTileAsset("DoubleStraight", TileType.DoubleStraight);
        tile.roadExits = new bool[] { true, false, true, false };
        EditorUtility.SetDirty(tile);
        Debug.Log("Created DoubleStraight: Vertical double road (Up + Down)");
    }
    
    private void CreateSTurn()
    {
        TileData tile = CreateTileAsset("STurn", TileType.STurn);
        tile.roadExits = new bool[] { true, false, true, false };
        EditorUtility.SetDirty(tile);
        Debug.Log("Created STurn: S-shaped curve connecting opposite sides");
    }
    
    private void CreateYJunction()
    {
        TileData tile = CreateTileAsset("YJunction", TileType.YJunction);
        tile.roadExits = new bool[] { true, true, true, false };
        EditorUtility.SetDirty(tile);
        Debug.Log("Created YJunction: Y-fork with 3 exits (Up, Right, Down)");
    }
    
    private void CreateDiagonalCross()
    {
        TileData tile = CreateTileAsset("DiagonalCross", TileType.DiagonalCross);
        tile.roadExits = new bool[] { true, true, true, true };
        EditorUtility.SetDirty(tile);
        Debug.Log("Created DiagonalCross: Diagonal intersection with all 4 exits");
    }
    
    private void CreateTripleJunction()
    {
        TileData tile = CreateTileAsset("TripleJunction", TileType.TripleJunction);
        tile.roadExits = new bool[] { true, true, false, true };
        EditorUtility.SetDirty(tile);
        Debug.Log("Created TripleJunction: Central point with 3 exits (Up, Right, Left)");
    }
    
    private void CreateRoundabout()
    {
        TileData tile = CreateTileAsset("Roundabout", TileType.Roundabout);
        tile.roadExits = new bool[] { true, true, true, true };
        EditorUtility.SetDirty(tile);
        Debug.Log("Created Roundabout: Circular intersection with all 4 exits");
    }
    
    private TileData CreateTileAsset(string name, TileType type)
    {
        string assetPath = $"{tilesDataPath}/{name}.asset";
        
        TileData existingTile = AssetDatabase.LoadAssetAtPath<TileData>(assetPath);
        if (existingTile != null)
        {
            Debug.LogWarning($"TileData already exists: {name}. Updating it.");
            existingTile.tileType = type;
            return existingTile;
        }
        
        TileData tile = ScriptableObject.CreateInstance<TileData>();
        tile.tileType = type;
        
        Sprite sprite = LoadSprite(name);
        if (sprite != null)
        {
            tile.sprite = sprite;
            Debug.Log($"Assigned sprite to {name}");
        }
        else
        {
            Debug.LogWarning($"Sprite not found for {name} at path: {spritesPath}/{name}.png");
        }
        
        AssetDatabase.CreateAsset(tile, assetPath);
        Debug.Log($"Created TileData: {name}");
        
        return tile;
    }
    
    private Sprite LoadSprite(string spriteName)
    {
        string[] possibleNames = new string[]
        {
            spriteName,
            $"{spriteName}_",
            spriteName.Replace("_", "")
        };
        
        foreach (string name in possibleNames)
        {
            string path = $"{spritesPath}/{name}.png";
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
            {
                return sprite;
            }
        }
        
        return null;
    }
}