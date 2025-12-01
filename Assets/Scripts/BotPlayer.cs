using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotPlayer : MonoBehaviour
{
    [SerializeField] private float thinkingDelay = 1.5f;
    [SerializeField] private GridManager gridManager;
    
    private bool isThinking;
    
    public bool IsThinking => isThinking;
    
    public void Initialize(GridManager grid)
    {
        gridManager = grid;
    }
    
    public void MakeMove(TileData currentTile, System.Action<Vector2Int, int> onMoveDecided)
    {
        if (isThinking) return;
        StartCoroutine(ThinkAndMove(currentTile, onMoveDecided));
    }
    
    private IEnumerator ThinkAndMove(TileData currentTile, System.Action<Vector2Int, int> onMoveDecided)
    {
        isThinking = true;
        
        yield return new WaitForSeconds(thinkingDelay);
        
        var bestMove = FindBestMove(currentTile);
        
        isThinking = false;
        
        if (bestMove.HasValue)
        {
            onMoveDecided?.Invoke(bestMove.Value.position, bestMove.Value.rotation);
        }
    }
    
    private (Vector2Int position, int rotation)? FindBestMove(TileData tile)
    {
        List<(Vector2Int position, int rotation, int score)> scoredMoves = new List<(Vector2Int, int, int)>();
        List<Vector2Int> positions = gridManager.GetValidPositions();
        
        foreach (var pos in positions)
        {
            for (int rot = 0; rot < 4; rot++)
            {
                if (gridManager.CanPlaceTile(tile, pos, rot))
                {
                    int score = CalculateMoveScore(tile, pos, rot);
                    scoredMoves.Add((pos, rot, score));
                }
            }
        }
        
        if (scoredMoves.Count == 0) return null;
        
        int maxScore = -1000;
        List<(Vector2Int position, int rotation, int score)> bestMoves = new List<(Vector2Int, int, int)>();
        
        foreach (var move in scoredMoves)
        {
            if (move.score > maxScore)
            {
                maxScore = move.score;
                bestMoves.Clear();
                bestMoves.Add(move);
            }
            else if (move.score == maxScore)
            {
                bestMoves.Add(move);
            }
        }
        
        int randomIndex = Random.Range(0, bestMoves.Count);
        return (bestMoves[randomIndex].position, bestMoves[randomIndex].rotation);
    }
    
    private int CalculateMoveScore(TileData tile, Vector2Int position, int rotation)
    {
        int score = 0;
        bool[] exits = tile.GetRotatedExits(rotation);
        
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        
        for (int i = 0; i < 4; i++)
        {
            Vector2Int neighborPos = position + directions[i];
            PlacedTile neighbor = gridManager.GetTileAt(neighborPos);
            
            if (neighbor != null)
            {
                Direction fromNeighbor = (Direction)((i + 2) % 4);
                bool thisHasExit = exits[i];
                bool neighborHasExit = neighbor.HasExit(fromNeighbor);
                
                if (thisHasExit && neighborHasExit)
                {
                    score += 10;
                    
                    bool[] neighborExits = neighbor.GetCurrentExits();
                    int neighborExitDir = (i + 2) % 4;
                    
                    if (DoRoadsConnect(tile, rotation, i, neighbor.tileData, neighbor.rotation, neighborExitDir))
                    {
                        score += 5;
                    }
                }
                else if (!thisHasExit && !neighborHasExit)
                {
                    score += 1;
                }
            }
        }
        
        return score;
    }
    
    private bool DoRoadsConnect(TileData tile1, int rot1, int exitDir1, TileData tile2, int rot2, int exitDir2)
    {
        bool[] exits1 = tile1.GetRotatedExits(rot1);
        bool[] exits2 = tile2.GetRotatedExits(rot2);
        
        int oppositeExitCount1 = 0;
        int oppositeExitCount2 = 0;
        int connectedDir1 = -1;
        int connectedDir2 = -1;
        
        for (int i = 0; i < 4; i++)
        {
            if (i != exitDir1 && exits1[i])
            {
                oppositeExitCount1++;
                connectedDir1 = i;
            }
            if (i != exitDir2 && exits2[i])
            {
                oppositeExitCount2++;
                connectedDir2 = i;
            }
        }
        
        if (oppositeExitCount1 == 1 && oppositeExitCount2 == 1)
        {
            int opposite1 = (exitDir1 + 2) % 4;
            int opposite2 = (exitDir2 + 2) % 4;
            
            if (connectedDir1 == opposite1 && connectedDir2 == opposite2)
            {
                return true;
            }
            
            if ((connectedDir1 == (exitDir1 + 1) % 4 && connectedDir2 == (exitDir2 + 1) % 4) ||
                (connectedDir1 == (exitDir1 + 3) % 4 && connectedDir2 == (exitDir2 + 3) % 4))
            {
                return true;
            }
        }
        
        return oppositeExitCount1 > 1 || oppositeExitCount2 > 1;
    }
}