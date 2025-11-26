using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject gridSlotPrefab;
    [SerializeField] private float tileSize = 100f;
    
    private Dictionary<Vector2Int, PlacedTile> placedTiles = new Dictionary<Vector2Int, PlacedTile>();
    private Dictionary<Vector2Int, GameObject> gridSlots = new Dictionary<Vector2Int, GameObject>();
    
    public Dictionary<Vector2Int, PlacedTile> PlacedTiles => placedTiles;
    
    public void Initialize()
    {
        placedTiles.Clear();
        ClearGridSlots();
    }
    
    public PlacedTile PlaceTile(TileData tileData, Vector2Int position, int rotation, Color playerColor = default)
    {
        if (placedTiles.ContainsKey(position)) return null;
        
        GameObject bgObj = new GameObject($"TileBackground_{position.x}_{position.y}");
        bgObj.transform.SetParent(gridContainer);
        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchoredPosition = GridToUIPosition(position);
        bgRect.sizeDelta = new Vector2(tileSize, tileSize);
        Image bgImage = bgObj.AddComponent<Image>();
        
        GameObject tileObj = Instantiate(tilePrefab, gridContainer);
        RectTransform rect = tileObj.GetComponent<RectTransform>();
        rect.anchoredPosition = GridToUIPosition(position);
        rect.sizeDelta = new Vector2(tileSize, tileSize);
        
        PlacedTile tile = tileObj.GetComponent<PlacedTile>();
        if (tile == null) tile = tileObj.AddComponent<PlacedTile>();
        tile.Initialize(tileData, position, rotation, playerColor);
        tile.SetBackgroundImage(bgImage);
        
        placedTiles[position] = tile;
        
        UpdateValidSlots();
        
        return tile;
    }
    
    public bool CanPlaceTile(TileData tileData, Vector2Int position, int rotation)
    {
        if (placedTiles.ContainsKey(position)) return false;
        
        bool hasNeighbor = false;
        bool[] exits = tileData.GetRotatedExits(rotation);
        
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        
        for (int i = 0; i < 4; i++)
        {
            Vector2Int neighborPos = position + directions[i];
            
            if (placedTiles.TryGetValue(neighborPos, out PlacedTile neighbor))
            {
                hasNeighbor = true;
                
                Direction toNeighbor = (Direction)i;
                Direction fromNeighbor = (Direction)((i + 2) % 4);
                
                bool thisHasExit = exits[i];
                bool neighborHasExit = neighbor.HasExit(fromNeighbor);
                
                if (thisHasExit != neighborHasExit)
                {
                    return false;
                }
            }
        }
        
        return hasNeighbor || placedTiles.Count == 0;
    }
    
    public List<Vector2Int> GetValidPositions()
    {
        List<Vector2Int> valid = new List<Vector2Int>();
        
        if (placedTiles.Count == 0)
        {
            valid.Add(Vector2Int.zero);
            return valid;
        }
        
        HashSet<Vector2Int> candidates = new HashSet<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        
        foreach (var pos in placedTiles.Keys)
        {
            foreach (var dir in directions)
            {
                Vector2Int neighbor = pos + dir;
                if (!placedTiles.ContainsKey(neighbor))
                {
                    candidates.Add(neighbor);
                }
            }
        }
        
        valid.AddRange(candidates);
        return valid;
    }
    
    public void UpdateValidSlots()
    {
        ClearGridSlots();
        
        List<Vector2Int> validPositions = GetValidPositions();
        
        foreach (var pos in validPositions)
        {
            GameObject slot = Instantiate(gridSlotPrefab, gridContainer);
            RectTransform rect = slot.GetComponent<RectTransform>();
            rect.anchoredPosition = GridToUIPosition(pos);
            rect.sizeDelta = new Vector2(tileSize, tileSize);
            
            GridSlot slotComponent = slot.GetComponent<GridSlot>();
            if (slotComponent != null)
            {
                slotComponent.Initialize(pos);
            }
            
            gridSlots[pos] = slot;
        }
    }
    
    private void ClearGridSlots()
    {
        foreach (var slot in gridSlots.Values)
        {
            if (slot != null) Destroy(slot);
        }
        gridSlots.Clear();
    }
    
    public Vector2 GridToUIPosition(Vector2Int gridPos)
    {
        return new Vector2(gridPos.x * tileSize, gridPos.y * tileSize);
    }
    
    public Vector2Int UIToGridPosition(Vector2 uiPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(uiPos.x / tileSize),
            Mathf.RoundToInt(uiPos.y / tileSize)
        );
    }
    
    public PlacedTile GetTileAt(Vector2Int position)
    {
        placedTiles.TryGetValue(position, out PlacedTile tile);
        return tile;
    }
}