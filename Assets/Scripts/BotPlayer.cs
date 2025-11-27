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
        
        List<(Vector2Int position, int rotation)> validMoves = GetAllValidMoves(currentTile);
        
        if (validMoves.Count > 0)
        {
            int randomIndex = Random.Range(0, validMoves.Count);
            var move = validMoves[randomIndex];
            onMoveDecided?.Invoke(move.position, move.rotation);
        }
        
        isThinking = false;
    }
    
    private List<(Vector2Int, int)> GetAllValidMoves(TileData tile)
    {
        List<(Vector2Int, int)> validMoves = new List<(Vector2Int, int)>();
        List<Vector2Int> positions = gridManager.GetValidPositions();
        
        foreach (var pos in positions)
        {
            for (int rot = 0; rot < 4; rot++)
            {
                if (gridManager.CanPlaceTile(tile, pos, rot))
                {
                    validMoves.Add((pos, rot));
                }
            }
        }
        
        return validMoves;
    }
}
