using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameUIController : MonoBehaviour
{
    [Header("Current Tile Preview")]
    [SerializeField] private Image currentTileImage;
    [SerializeField] private RectTransform currentTileTransform;
    [SerializeField] private Button rotateLeftButton;
    [SerializeField] private Button rotateRightButton;
    
    [Header("Player Info")]
    [SerializeField] private TextMeshProUGUI currentPlayerText;
    [SerializeField] private Transform playerScoresContainer;
    [SerializeField] private GameObject playerScorePrefab;
    
    [Header("Game Info")]
    [SerializeField] private TextMeshProUGUI remainingTilesText;
    [SerializeField] private TextMeshProUGUI messageText;
    
    [Header("Panels")]
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    
    [Header("Start Panel")]
    [SerializeField] private TMP_InputField playerCountInput;
    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    
    private GameManager gameManager;
    private DeckManager deckManager;
    private Dictionary<Player, TextMeshProUGUI> playerScoreTexts = new Dictionary<Player, TextMeshProUGUI>();
    
    private void Start()
    {
        gameManager = GameManager.Instance;
        deckManager = FindObjectOfType<DeckManager>();
        
        SetupButtons();
        SubscribeToEvents();
        
        ShowStartPanel();
    }
    
    private void SetupButtons()
    {
        if (rotateLeftButton != null)
            rotateLeftButton.onClick.AddListener(OnRotateLeft);
        
        if (rotateRightButton != null)
            rotateRightButton.onClick.AddListener(OnRotateRight);
        
        if (startButton != null)
            startButton.onClick.AddListener(OnStartGame);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartGame);
    }
    
    private void SubscribeToEvents()
    {
        if (gameManager == null) return;
        
        gameManager.OnPlayerChanged += HandlePlayerChanged;
        gameManager.OnTileDrawn += HandleTileDrawn;
        gameManager.OnScoreChanged += HandleScoreChanged;
        gameManager.OnGameEnded += HandleGameEnded;
        gameManager.OnRouteCompleted += HandleRouteCompleted;
    }
    
    private void OnDestroy()
    {
        if (gameManager == null) return;
        
        gameManager.OnPlayerChanged -= HandlePlayerChanged;
        gameManager.OnTileDrawn -= HandleTileDrawn;
        gameManager.OnScoreChanged -= HandleScoreChanged;
        gameManager.OnGameEnded -= HandleGameEnded;
        gameManager.OnRouteCompleted -= HandleRouteCompleted;
    }
    
    private void OnRotateLeft()
    {
        gameManager.RotateTileCounterClockwise();
        UpdateTilePreview();
    }
    
    private void OnRotateRight()
    {
        gameManager.RotateTile();
        UpdateTilePreview();
    }
    
    private void OnStartGame()
    {
        int playerCount = 2;
        if (playerCountInput != null && int.TryParse(playerCountInput.text, out int count))
        {
            playerCount = Mathf.Clamp(count, 2, 4);
        }
        
        gameManager.StartGame(playerCount);
        ShowGamePanel();
        InitializePlayerScores();
    }
    
    private void OnRestartGame()
    {
        ShowStartPanel();
    }
    
    private void ShowStartPanel()
    {
        if (startPanel != null) startPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);
    }
    
    private void ShowGamePanel()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
        if (endPanel != null) endPanel.SetActive(false);
    }
    
    private void ShowEndPanel(Player winner)
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(true);
        
        if (winnerText != null)
        {
            winnerText.text = $"{winner.playerName} Wins!\nScore: {winner.score}";
        }
    }
    
    private void InitializePlayerScores()
    {
        foreach (Transform child in playerScoresContainer)
        {
            Destroy(child.gameObject);
        }
        playerScoreTexts.Clear();
        
        foreach (var player in gameManager.Players)
        {
            GameObject scoreObj = Instantiate(playerScorePrefab, playerScoresContainer);
            TextMeshProUGUI scoreText = scoreObj.GetComponentInChildren<TextMeshProUGUI>();
            
            if (scoreText != null)
            {
                scoreText.text = $"{player.playerName}: 0";
                scoreText.color = player.playerColor;
                playerScoreTexts[player] = scoreText;
            }
        }
    }
    
    private void HandlePlayerChanged(Player player)
    {
        if (currentPlayerText != null)
        {
            currentPlayerText.text = $"Current: {player.playerName}";
            currentPlayerText.color = player.playerColor;
        }
        
        UpdateRemainingTiles();
    }
    
    private void HandleTileDrawn(TileData tile)
    {
        UpdateTilePreview();
        UpdateRemainingTiles();
    }
    
    private void HandleScoreChanged(Player player, int points, int total)
    {
        if (playerScoreTexts.TryGetValue(player, out TextMeshProUGUI text))
        {
            text.text = $"{player.playerName}: {total}";
        }
        
        ShowMessage($"{player.playerName} +{points}!");
    }
    
    private void HandleGameEnded(Player winner)
    {
        ShowEndPanel(winner);
    }
    
    private void HandleRouteCompleted(RouteResult route)
    {
        string msg = route.GetScore() > 0 
            ? $"Route completed! Length: {route.length}" 
            : $"Route completed (no points). Length: {route.length}";
        
        ShowMessage(msg);
    }
    
    private void UpdateTilePreview()
    {
        if (currentTileImage == null || gameManager.CurrentTile == null) return;
        
        currentTileImage.sprite = gameManager.CurrentTile.sprite;
        currentTileTransform.localRotation = Quaternion.Euler(0, 0, -gameManager.CurrentRotation * 90);
    }
    
    private void UpdateRemainingTiles()
    {
        if (remainingTilesText != null && deckManager != null)
        {
            remainingTilesText.text = $"Tiles: {deckManager.RemainingTiles}";
        }
    }
    
    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
            CancelInvoke(nameof(ClearMessage));
            Invoke(nameof(ClearMessage), 3f);
        }
    }
    
    private void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }
}
