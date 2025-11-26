using UnityEngine;
using System.Collections.Generic;

public class RouteChecker : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    
    private static readonly Vector2Int[] DirectionVectors = 
    {
        Vector2Int.up,
        Vector2Int.right,
        Vector2Int.down,
        Vector2Int.left
    };
    
    public List<RouteResult> CheckCompletedRoutes(Vector2Int placedPosition)
    {
        List<RouteResult> results = new List<RouteResult>();
        PlacedTile placedTile = gridManager.GetTileAt(placedPosition);
        
        if (placedTile == null) return results;
        
        bool[] exits = placedTile.GetCurrentExits();
        
        for (int i = 0; i < 4; i++)
        {
            if (!exits[i]) continue;
            
            RouteResult route = TraceRoute(placedPosition, (Direction)i);
            
            if (route != null && route.isComplete && !IsRouteAlreadyCounted(route))
            {
                results.Add(route);
            }
        }
        
        return RemoveDuplicateRoutes(results);
    }
    
    private RouteResult TraceRoute(Vector2Int startPos, Direction startDir)
    {
        HashSet<(Vector2Int, Direction)> visited = new HashSet<(Vector2Int, Direction)>();
        List<PlacedTile> routeTiles = new List<PlacedTile>();
        
        Vector2Int currentPos = startPos;
        Direction currentDir = startDir;
        
        PlacedTile startTile = gridManager.GetTileAt(startPos);
        if (startTile != null && !startTile.isMarkedComplete)
        {
            routeTiles.Add(startTile);
        }
        
        while (true)
        {
            Vector2Int nextPos = currentPos + DirectionVectors[(int)currentDir];
            Direction enterDir = GetOppositeDirection(currentDir);
            
            PlacedTile nextTile = gridManager.GetTileAt(nextPos);
            
            if (nextTile == null)
            {
                return new RouteResult
                {
                    tiles = routeTiles,
                    length = routeTiles.Count,
                    isComplete = false
                };
            }
            
            if (!nextTile.HasExit(enterDir))
            {
                return new RouteResult
                {
                    tiles = routeTiles,
                    length = routeTiles.Count,
                    isComplete = true
                };
            }
            
            if (visited.Contains((nextPos, enterDir)))
            {
                return new RouteResult
                {
                    tiles = routeTiles,
                    length = routeTiles.Count,
                    isComplete = true
                };
            }
            
            visited.Add((nextPos, enterDir));
            
            if (!nextTile.isMarkedComplete && !routeTiles.Contains(nextTile))
            {
                routeTiles.Add(nextTile);
            }
            
            Direction? exitDir = FindExitDirection(nextTile, enterDir);
            
            if (exitDir == null)
            {
                return new RouteResult
                {
                    tiles = routeTiles,
                    length = routeTiles.Count,
                    isComplete = true
                };
            }
            
            currentPos = nextPos;
            currentDir = exitDir.Value;
        }
    }
    
    private Direction? FindExitDirection(PlacedTile tile, Direction enterDir)
    {
        bool[] exits = tile.GetCurrentExits();
        
        for (int i = 0; i < 4; i++)
        {
            if ((Direction)i != enterDir && exits[i])
            {
                return (Direction)i;
            }
        }
        
        return null;
    }
    
    private Direction GetOppositeDirection(Direction dir)
    {
        return (Direction)(((int)dir + 2) % 4);
    }
    
    private bool IsRouteAlreadyCounted(RouteResult route)
    {
        foreach (var tile in route.tiles)
        {
            if (tile.isMarkedComplete) return true;
        }
        return false;
    }
    
    private List<RouteResult> RemoveDuplicateRoutes(List<RouteResult> routes)
    {
        List<RouteResult> unique = new List<RouteResult>();
        
        foreach (var route in routes)
        {
            bool isDuplicate = false;
            
            foreach (var existing in unique)
            {
                if (AreRoutesSame(route, existing))
                {
                    isDuplicate = true;
                    break;
                }
            }
            
            if (!isDuplicate)
            {
                unique.Add(route);
            }
        }
        
        return unique;
    }
    
    private bool AreRoutesSame(RouteResult a, RouteResult b)
    {
        if (a.tiles.Count != b.tiles.Count) return false;
        
        HashSet<PlacedTile> setA = new HashSet<PlacedTile>(a.tiles);
        
        foreach (var tile in b.tiles)
        {
            if (!setA.Contains(tile)) return false;
        }
        
        return true;
    }
    
    public int CalculateScore(int routeLength)
    {
        if (routeLength == 7) return 1;
        if (routeLength == 11) return 2;
        return 0;
    }
}

public class RouteResult
{
    public List<PlacedTile> tiles;
    public int length;
    public bool isComplete;
    
    public int GetScore()
    {
        if (length == 7) return 1;
        if (length == 11) return 2;
        return 0;
    }
    
    public void MarkTilesComplete()
    {
        foreach (var tile in tiles)
        {
            tile.MarkAsComplete();
        }
    }
}