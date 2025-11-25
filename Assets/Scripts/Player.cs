using UnityEngine;
using System;

[Serializable]
public class Player
{
    public string playerName;
    public int score;
    public Color playerColor;
    
    public Player(string name, Color color)
    {
        playerName = name;
        playerColor = color;
        score = 0;
    }
    
    public void AddScore(int points)
    {
        score += points;
    }
}
