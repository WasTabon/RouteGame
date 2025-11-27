using UnityEngine;
using System;
using System.Collections.Generic;

public enum GameMode
{
    VsPlayers,
    VsBot
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private GridManager gridManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private RouteChecker routeChecker;
    [SerializeField] private BotPlayer botPlayer;
    [SerializeField] private TileData startTile;
    [SerializeField] private int playerCount = 2;
    
    private List<Player> players = new List<Player>();
    private int currentPlayerIndex;
    private TileData currentTile;
    private int currentRotation;
    private GameMode currentGameMode;
    private int botPlayerIndex = -1;
    
    public Player CurrentPlayer => players[currentPlayerIndex];
    public TileData CurrentTile => currentTile;
    public int CurrentRotation => currentRotation;
    public List<Player> Players => players;
    public bool IsGameOver { get; private set; }
    public GameMode CurrentGameMode => currentGameMode;
    public bool IsBotTurn => currentGameMode == GameMode.VsBot && currentPlayerIndex == botPlayerIndex;
    public bool IsBotThinking => botPlayer != null && botPlayer.IsThinking;
    
    public event Action<Player> OnPlayerChanged;
    public event Action<TileData> OnTileDrawn;
    public event Action<Player, int, int> OnScoreChanged;
    public event Action<Player> OnGameEnded;
    public event Action<RouteResult> OnRouteCompleted;
    public event Action OnBotThinkingStarted;
    public event Action OnBotThinkingEnded;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void OnEnable()
    {
        GridSlot.OnSlotClicked += HandleSlotClicked;
    }
    
    private void OnDisable()
    {
        GridSlot.OnSlotClicked -= HandleSlotClicked;
    }
    
    public void StartGameVsBot(string playerName)
    {
        currentGameMode = GameMode.VsBot;
        
        players.Clear();
        players.Add(new Player(playerName, Color.blue));
        players.Add(new Player("Bot", Color.red));
        
        botPlayerIndex = 1;
        
        InitializeGame();
    }
    
    public void StartGame(List<string> playerNames)
    {
        currentGameMode = GameMode.VsPlayers;
        botPlayerIndex = -1;
        
        players.Clear();
        Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
        
        for (int i = 0; i < playerNames.Count; i++)
        {
            players.Add(new Player(playerNames[i], colors[i % colors.Length]));
        }
        
        InitializeGame();
    }
    
    public void StartGame(int numPlayers)
    {
        List<string> names = new List<string>();
        for (int i = 0; i < numPlayers; i++)
        {
            names.Add($"Player {i + 1}");
        }
        StartGame(names);
    }
    
    private void InitializeGame()
    {
        IsGameOver = false;
        currentPlayerIndex = 0;
        currentRotation = 0;
        
        gridManager.Initialize();
        deckManager.InitializeDeck();
        
        if (botPlayer != null)
        {
            botPlayer.Initialize(gridManager);
        }
        
        if (startTile != null)
        {
            gridManager.PlaceTile(startTile, Vector2Int.zero, 0, Color.white);
        }
        
        DrawTile();
        OnPlayerChanged?.Invoke(CurrentPlayer);
        
        CheckBotTurn();
    }
    
    private void DrawTile()
    {
        currentTile = deckManager.DrawTile();
        currentRotation = 0;
        
        if (currentTile == null)
        {
            EndGame();
            return;
        }
        
        OnTileDrawn?.Invoke(currentTile);
    }
    
    public void RotateTile()
    {
        if (IsBotTurn) return;
        currentRotation = (currentRotation + 1) % 4;
    }
    
    public void RotateTileCounterClockwise()
    {
        if (IsBotTurn) return;
        currentRotation = (currentRotation + 3) % 4;
    }
    
    private void HandleSlotClicked(Vector2Int position)
    {
        if (IsGameOver || currentTile == null) return;
        if (IsBotTurn || IsBotThinking) return;
        
        TryPlaceTile(position);
    }
    
    public bool TryPlaceTile(Vector2Int position)
    {
        return TryPlaceTileWithRotation(position, currentRotation);
    }
    
    public bool TryPlaceTileWithRotation(Vector2Int position, int rotation)
    {
        if (currentTile == null || IsGameOver) return false;
        
        if (!gridManager.CanPlaceTile(currentTile, position, rotation))
        {
            return false;
        }
        
        currentRotation = rotation;
        
        PlacedTile placed = gridManager.PlaceTile(currentTile, position, currentRotation, CurrentPlayer.playerColor);
        
        if (placed != null)
        {
            CheckRoutes(position);
            NextTurn();
            return true;
        }
        
        return false;
    }
    
    private void CheckRoutes(Vector2Int position)
    {
        List<RouteResult> completedRoutes = routeChecker.CheckCompletedRoutes(position);
        
        foreach (var route in completedRoutes)
        {
            int score = route.GetScore();
            
            if (score > 0)
            {
                CurrentPlayer.AddScore(score);
                OnScoreChanged?.Invoke(CurrentPlayer, score, CurrentPlayer.score);
            }
            
            route.MarkTilesComplete();
            OnRouteCompleted?.Invoke(route);
        }
    }
    
    private void NextTurn()
    {
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        DrawTile();
        
        if (!IsGameOver)
        {
            OnPlayerChanged?.Invoke(CurrentPlayer);
            CheckBotTurn();
        }
    }
    
    private void CheckBotTurn()
    {
        if (IsBotTurn && currentTile != null && botPlayer != null)
        {
            OnBotThinkingStarted?.Invoke();
            botPlayer.MakeMove(currentTile, OnBotMoveDecided);
        }
    }
    
    private void OnBotMoveDecided(Vector2Int position, int rotation)
    {
        OnBotThinkingEnded?.Invoke();
        TryPlaceTileWithRotation(position, rotation);
    }
    
    private void EndGame()
    {
        IsGameOver = true;
        
        Player winner = players[0];
        foreach (var player in players)
        {
            if (player.score > winner.score)
            {
                winner = player;
            }
        }
        
        OnGameEnded?.Invoke(winner);
    }
    
    public bool CanPlaceCurrentTile(Vector2Int position)
    {
        if (currentTile == null) return false;
        return gridManager.CanPlaceTile(currentTile, position, currentRotation);
    }
    
    public List<Vector2Int> GetValidPositionsForCurrentTile()
    {
        if (currentTile == null) return new List<Vector2Int>();
        
        List<Vector2Int> allPositions = gridManager.GetValidPositions();
        List<Vector2Int> validPositions = new List<Vector2Int>();
        
        foreach (var pos in allPositions)
        {
            for (int rot = 0; rot < 4; rot++)
            {
                if (gridManager.CanPlaceTile(currentTile, pos, rot))
                {
                    validPositions.Add(pos);
                    break;
                }
            }
        }
        
        return validPositions;
    }
}
