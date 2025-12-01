using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private Button menuButton;
    
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
    [SerializeField] private GameObject modeSelectPanel;
    [SerializeField] private GameObject playerSetupPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject endPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    
    [Header("Mode Select Panel")]
    [SerializeField] private Button vsBotButton;
    [SerializeField] private Button vsPlayersButton;
    [SerializeField] private TextMeshProUGUI vsPlayersButtonText;
    [SerializeField] private Button restorePurchaseButton;
    [SerializeField] private TextMeshProUGUI purchaseStatusText;
    
    [Header("Player Setup Panel")]
    [SerializeField] private TMP_InputField playerCountInput;
    [SerializeField] private TMP_InputField playerNameInput;
    [SerializeField] private Button startButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI setupTitleText;
    
    [Header("End Panel")]
    [SerializeField] private Button restartButton;
    
    [Header("Bot Indicator")]
    [SerializeField] private GameObject botThinkingIndicator;
    [SerializeField] private TextMeshProUGUI botThinkingText;
    
    private GameManager gameManager;
    private DeckManager deckManager;
    private IAPManager iapManager;
    private Dictionary<Player, TextMeshProUGUI> playerScoreTexts = new Dictionary<Player, TextMeshProUGUI>();
    private bool isVsBotMode;
    
    private void Awake()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
    }
    
    private void Start()
    {
        if (gameManager == null)
        {
            gameManager = GameManager.Instance;
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }
        }
        
        deckManager = FindObjectOfType<DeckManager>();
        iapManager = IAPManager.Instance;
        
        SetupButtons();
        SubscribeToEvents();
        SubscribeToIAPEvents();
        
        ShowModeSelectPanel();
        UpdatePurchaseUI();
    }
    
    private void SetupButtons()
    {
        if (rotateLeftButton != null)
            rotateLeftButton.onClick.AddListener(OnRotateLeft);
        
        if (rotateRightButton != null)
            rotateRightButton.onClick.AddListener(OnRotateRight);
        
        if (vsBotButton != null)
            vsBotButton.onClick.AddListener(OnVsBotSelected);
        
        if (vsPlayersButton != null)
            vsPlayersButton.onClick.AddListener(OnVsPlayersSelected);
        
        if (restorePurchaseButton != null)
            restorePurchaseButton.onClick.AddListener(OnRestorePurchase);
        
        if (startButton != null)
            startButton.onClick.AddListener(OnStartGame);
        
        if (backButton != null)
            backButton.onClick.AddListener(OnBackToModeSelect);
        
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartGame);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(OnRestartGame);
    }
    
    private void SubscribeToEvents()
    {
        if (gameManager == null) return;
        
        gameManager.OnPlayerChanged += HandlePlayerChanged;
        gameManager.OnTileDrawn += HandleTileDrawn;
        gameManager.OnScoreChanged += HandleScoreChanged;
        gameManager.OnGameEnded += HandleGameEnded;
        gameManager.OnRouteCompleted += HandleRouteCompleted;
        gameManager.OnBotThinkingStarted += HandleBotThinkingStarted;
        gameManager.OnBotThinkingEnded += HandleBotThinkingEnded;
    }
    
    private void SubscribeToIAPEvents()
    {
        if (iapManager == null) return;
        
        iapManager.OnPurchaseCompleted += HandlePurchaseComplete;
        iapManager.OnPurchaseError += HandlePurchaseFailed;
        iapManager.OnRestoreCompleted += HandleRestoreComplete;
    }
    
    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnPlayerChanged -= HandlePlayerChanged;
            gameManager.OnTileDrawn -= HandleTileDrawn;
            gameManager.OnScoreChanged -= HandleScoreChanged;
            gameManager.OnGameEnded -= HandleGameEnded;
            gameManager.OnRouteCompleted -= HandleRouteCompleted;
            gameManager.OnBotThinkingStarted -= HandleBotThinkingStarted;
            gameManager.OnBotThinkingEnded -= HandleBotThinkingEnded;
        }
        
        if (iapManager != null)
        {
            iapManager.OnPurchaseCompleted -= HandlePurchaseComplete;
            iapManager.OnPurchaseError -= HandlePurchaseFailed;
            iapManager.OnRestoreCompleted -= HandleRestoreComplete;
        }
    }
    
    private void OnVsBotSelected()
    {
        isVsBotMode = true;
        ShowPlayerSetupPanel();
        
        if (setupTitleText != null)
            setupTitleText.text = "VS Bot";
        
        if (playerCountInput != null)
            playerCountInput.gameObject.SetActive(false);
        
        if (playerNameInput != null)
        {
            playerNameInput.gameObject.SetActive(true);
            playerNameInput.text = "Player";
        }
    }
    
    private void OnVsPlayersSelected()
    {
        if (iapManager != null && !iapManager.IsMultiplayerUnlocked)
        {
            iapManager.PurchaseMultiplayer();
            return;
        }
        
        isVsBotMode = false;
        ShowPlayerSetupPanel();
        
        if (setupTitleText != null)
            setupTitleText.text = "VS Players";
        
        if (playerCountInput != null)
        {
            playerCountInput.gameObject.SetActive(true);
            playerCountInput.text = "2";
        }
        
        if (playerNameInput != null)
            playerNameInput.gameObject.SetActive(false);
    }
    
    private void OnRestorePurchase()
    {
        if (iapManager != null)
        {
            if (purchaseStatusText != null)
                purchaseStatusText.text = "Restoring...";
            
            iapManager.RestorePurchases();
        }
    }
    
    private void OnBackToModeSelect()
    {
        ShowModeSelectPanel();
    }
    
    private void OnRotateLeft()
    {
        if (gameManager == null || gameManager.IsBotTurn || gameManager.IsBotThinking) return;
        gameManager.RotateTileCounterClockwise();
        UpdateTilePreview();
    }
    
    private void OnRotateRight()
    {
        if (gameManager == null || gameManager.IsBotTurn || gameManager.IsBotThinking) return;
        gameManager.RotateTile();
        UpdateTilePreview();
    }
    
    private void OnStartGame()
    {
        if (isVsBotMode)
        {
            string playerName = "Player";
            if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
            {
                playerName = playerNameInput.text;
            }
            
            gameManager.StartGameVsBot(playerName);
        }
        else
        {
            int playerCount = 2;
            if (playerCountInput != null && int.TryParse(playerCountInput.text, out int count))
            {
                playerCount = Mathf.Clamp(count, 2, 4);
            }
            
            gameManager.StartGame(playerCount);
        }
        
        ShowGamePanel();
        InitializePlayerScores();
    }
    
    private void OnRestartGame()
    {
        ShowModeSelectPanel();
    }
    
    private void ShowModeSelectPanel()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (modeSelectPanel != null) modeSelectPanel.SetActive(true);
        if (playerSetupPanel != null) playerSetupPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);
        
        UpdatePurchaseUI();
    }
    
    private void ShowPlayerSetupPanel()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (modeSelectPanel != null) modeSelectPanel.SetActive(false);
        if (playerSetupPanel != null) playerSetupPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(false);
    }
    
    private void ShowGamePanel()
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (modeSelectPanel != null) modeSelectPanel.SetActive(false);
        if (playerSetupPanel != null) playerSetupPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
        if (endPanel != null) endPanel.SetActive(false);
        
        if (botThinkingIndicator != null)
            botThinkingIndicator.SetActive(false);
        
        if (rotateLeftButton != null)
            rotateLeftButton.interactable = true;
        if (rotateRightButton != null)
            rotateRightButton.interactable = true;
    }
    
    private void ShowEndPanel(Player winner)
    {
        if (startPanel != null) startPanel.SetActive(false);
        if (modeSelectPanel != null) modeSelectPanel.SetActive(false);
        if (playerSetupPanel != null) playerSetupPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (endPanel != null) endPanel.SetActive(true);
        
        if (winnerText != null)
        {
            winnerText.text = $"{winner.playerName} Wins!\nScore: {winner.score}";
        }
    }
    
    private void UpdatePurchaseUI()
    {
        bool isUnlocked = iapManager != null && iapManager.IsMultiplayerUnlocked;
        
        if (vsPlayersButtonText != null)
        {
            if (isUnlocked)
            {
                vsPlayersButtonText.text = "VS Players";
            }
            else
            {
                string price = iapManager != null ? iapManager.GetMultiplayerPrice() : "";
                vsPlayersButtonText.text = string.IsNullOrEmpty(price) ? "VS Players (Premium)" : $"VS Players ({price})";
            }
        }
        
        if (restorePurchaseButton != null)
        {
            restorePurchaseButton.gameObject.SetActive(!isUnlocked);
        }
        
        if (purchaseStatusText != null)
        {
            purchaseStatusText.text = isUnlocked ? "Multiplayer Unlocked!" : "";
        }
    }
    
    private void HandlePurchaseComplete(bool success)
    {
        UpdatePurchaseUI();
        
        if (success)
        {
            ShowMessage("Purchase successful!");
            OnVsPlayersSelected();
        }
    }
    
    private void HandlePurchaseFailed(string reason)
    {
        if (purchaseStatusText != null)
            purchaseStatusText.text = $"Purchase failed: {reason}";
        
        ShowMessage($"Purchase failed: {reason}");
    }
    
    private void HandleRestoreComplete()
    {
        UpdatePurchaseUI();
        
        if (purchaseStatusText != null)
        {
            bool isUnlocked = iapManager != null && iapManager.IsMultiplayerUnlocked;
            purchaseStatusText.text = isUnlocked ? "Purchases restored!" : "No purchases to restore";
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
        UpdateControlsInteractability();
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
    
    private void HandleBotThinkingStarted()
    {
        if (botThinkingIndicator != null)
            botThinkingIndicator.SetActive(true);
        
        if (botThinkingText != null)
            botThinkingText.text = "Bot is thinking...";
        
        UpdateControlsInteractability();
    }
    
    private void HandleBotThinkingEnded()
    {
        if (botThinkingIndicator != null)
            botThinkingIndicator.SetActive(false);
        
        UpdateControlsInteractability();
    }
    
    private void UpdateControlsInteractability()
    {
        bool canInteract = gameManager != null && !gameManager.IsBotTurn && !gameManager.IsBotThinking;
        
        if (rotateLeftButton != null)
            rotateLeftButton.interactable = canInteract;
        
        if (rotateRightButton != null)
            rotateRightButton.interactable = canInteract;
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