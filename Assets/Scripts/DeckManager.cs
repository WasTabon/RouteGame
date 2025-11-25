using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    [SerializeField] private List<TileData> tileTypes;
    [SerializeField] private int straightCount = 20;
    [SerializeField] private int turnCount = 15;
    [SerializeField] private int tJunctionCount = 10;
    [SerializeField] private int crossroadCount = 5;
    [SerializeField] private int deadEndCount = 8;
    
    private List<TileData> deck = new List<TileData>();
    
    public int RemainingTiles => deck.Count;
    
    public void InitializeDeck()
    {
        deck.Clear();
        
        foreach (var tile in tileTypes)
        {
            int count = GetCountForType(tile.tileType);
            for (int i = 0; i < count; i++)
            {
                deck.Add(tile);
            }
        }
        
        ShuffleDeck();
    }
    
    private int GetCountForType(TileType type)
    {
        switch (type)
        {
            case TileType.Straight: return straightCount;
            case TileType.Turn: return turnCount;
            case TileType.TJunction: return tJunctionCount;
            case TileType.Crossroad: return crossroadCount;
            case TileType.DeadEnd: return deadEndCount;
            default: return 0;
        }
    }
    
    private void ShuffleDeck()
    {
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = deck[i];
            deck[i] = deck[j];
            deck[j] = temp;
        }
    }
    
    public TileData DrawTile()
    {
        if (deck.Count == 0) return null;
        
        TileData tile = deck[0];
        deck.RemoveAt(0);
        return tile;
    }
    
    public bool HasTiles()
    {
        return deck.Count > 0;
    }
}
