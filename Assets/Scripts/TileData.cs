using UnityEngine;
using System;

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}

public enum TileType
{
    Straight,
    Turn,
    TJunction,
    Crossroad,
    DeadEnd,
    DoubleStraight,
    STurn,
    YJunction,
    DiagonalCross,
    TripleJunction,
    Roundabout
}

[CreateAssetMenu(fileName = "TileData", menuName = "711Route/TileData")]
public class TileData : ScriptableObject
{
    public TileType tileType;
    public Sprite sprite;
    public bool[] roadExits = new bool[4];
    
    public bool HasExit(Direction dir)
    {
        return roadExits[(int)dir];
    }
    
    public bool[] GetRotatedExits(int rotation)
    {
        bool[] rotated = new bool[4];
        for (int i = 0; i < 4; i++)
        {
            int originalIndex = (i - rotation + 4) % 4;
            rotated[i] = roadExits[originalIndex];
        }
        return rotated;
    }
}