#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class GameModeSetupEditor : EditorWindow
{
    [MenuItem("711Route/Setup Game Mode UI")]
    public static void SetupGameModeUI()
    {
        GameObject canvas = GameObject.Find("GameCanvas");
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("Error", "GameCanvas not found! Run '711Route/Setup Complete Scene' first.", "OK");
            return;
        }
        
        Transform startPanel = canvas.transform.Find("StartPanel");
        if (startPanel == null)
        {
            EditorUtility.DisplayDialog("Error", "StartPanel not found!", "OK");
            return;
        }
        
        startPanel.gameObject.SetActive(false);
        
        GameObject modeSelectPanel = CreateModeSelectPanel(canvas.transform);
        GameObject playerSetupPanel = CreatePlayerSetupPanel(canvas.transform);
        GameObject botThinkingIndicator = CreateBotThinkingIndicator(canvas.transform.Find("GamePanel"));
        GameObject iapManagerObj = CreateIAPManager();
        GameObject botPlayerObj = CreateBotPlayer();
        
        LinkNewReferences(canvas, modeSelectPanel, playerSetupPanel, botThinkingIndicator, botPlayerObj);
        
        modeSelectPanel.SetActive(true);
        playerSetupPanel.SetActive(false);
        
        AssetDatabase.SaveAssets();
        EditorUtility.SetDirty(canvas);
        
        Debug.Log("Game Mode UI setup complete!");
        EditorUtility.DisplayDialog("Success", "Game Mode UI created!\n\nNew elements:\n- ModeSelectPanel\n- PlayerSetupPanel\n- BotThinkingIndicator\n- IAPManager\n- BotPlayer", "OK");
    }
    
    private static GameObject CreateModeSelectPanel(Transform parent)
    {
        GameObject panel = new GameObject("ModeSelectPanel");
        panel.transform.SetParent(parent, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.2f, 1f);
        
        GameObject title = CreateText("Title", panel.transform, "711 Route Builder", 72);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.8f);
        titleRect.anchorMax = new Vector2(0.5f, 0.8f);
        titleRect.sizeDelta = new Vector2(800, 100);
        title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        
        GameObject subtitle = CreateText("Subtitle", panel.transform, "Select Game Mode", 42);
        RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0.5f, 0.65f);
        subtitleRect.anchorMax = new Vector2(0.5f, 0.65f);
        subtitleRect.sizeDelta = new Vector2(600, 60);
        subtitle.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        
        GameObject vsBotButton = CreateButton("VsBotButton", panel.transform, "VS Bot", new Color(0.2f, 0.6f, 0.3f, 1f));
        RectTransform vsBotRect = vsBotButton.GetComponent<RectTransform>();
        vsBotRect.anchorMin = new Vector2(0.5f, 0.5f);
        vsBotRect.anchorMax = new Vector2(0.5f, 0.5f);
        vsBotRect.sizeDelta = new Vector2(400, 100);
        
        GameObject vsPlayersButton = CreateButton("VsPlayersButton", panel.transform, "VS Players (Premium)", new Color(0.6f, 0.4f, 0.2f, 1f));
        RectTransform vsPlayersRect = vsPlayersButton.GetComponent<RectTransform>();
        vsPlayersRect.anchorMin = new Vector2(0.5f, 0.35f);
        vsPlayersRect.anchorMax = new Vector2(0.5f, 0.35f);
        vsPlayersRect.sizeDelta = new Vector2(400, 100);
        
        GameObject restoreButton = CreateButton("RestorePurchaseButton", panel.transform, "Restore Purchase", new Color(0.3f, 0.3f, 0.4f, 1f));
        RectTransform restoreRect = restoreButton.GetComponent<RectTransform>();
        restoreRect.anchorMin = new Vector2(0.5f, 0.2f);
        restoreRect.anchorMax = new Vector2(0.5f, 0.2f);
        restoreRect.sizeDelta = new Vector2(300, 60);
        restoreButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 24;
        
        GameObject statusText = CreateText("PurchaseStatusText", panel.transform, "", 24);
        RectTransform statusRect = statusText.GetComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0.5f, 0.1f);
        statusRect.anchorMax = new Vector2(0.5f, 0.1f);
        statusRect.sizeDelta = new Vector2(600, 40);
        statusText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        statusText.GetComponent<TextMeshProUGUI>().color = new Color(0.7f, 0.7f, 0.7f, 1f);
        
        return panel;
    }
    
    private static GameObject CreatePlayerSetupPanel(Transform parent)
    {
        GameObject panel = new GameObject("PlayerSetupPanel");
        panel.transform.SetParent(parent, false);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.2f, 1f);
        
        GameObject title = CreateText("SetupTitleText", panel.transform, "Game Setup", 56);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.75f);
        titleRect.anchorMax = new Vector2(0.5f, 0.75f);
        titleRect.sizeDelta = new Vector2(600, 80);
        title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        
        GameObject playerCountLabel = CreateText("PlayerCountLabel", panel.transform, "Number of Players (2-4):", 32);
        RectTransform countLabelRect = playerCountLabel.GetComponent<RectTransform>();
        countLabelRect.anchorMin = new Vector2(0.5f, 0.55f);
        countLabelRect.anchorMax = new Vector2(0.5f, 0.55f);
        countLabelRect.sizeDelta = new Vector2(400, 40);
        playerCountLabel.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        
        GameObject playerCountInput = CreateInputField("PlayerCountInput", panel.transform, "2");
        RectTransform countInputRect = playerCountInput.GetComponent<RectTransform>();
        countInputRect.anchorMin = new Vector2(0.5f, 0.48f);
        countInputRect.anchorMax = new Vector2(0.5f, 0.48f);
        countInputRect.sizeDelta = new Vector2(150, 60);
        
        GameObject playerNameLabel = CreateText("PlayerNameLabel", panel.transform, "Your Name:", 32);
        RectTransform nameLabelRect = playerNameLabel.GetComponent<RectTransform>();
        nameLabelRect.anchorMin = new Vector2(0.5f, 0.55f);
        nameLabelRect.anchorMax = new Vector2(0.5f, 0.55f);
        nameLabelRect.sizeDelta = new Vector2(400, 40);
        playerNameLabel.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        playerNameLabel.SetActive(false);
        
        GameObject playerNameInput = CreateInputField("PlayerNameInput", panel.transform, "Player");
        RectTransform nameInputRect = playerNameInput.GetComponent<RectTransform>();
        nameInputRect.anchorMin = new Vector2(0.5f, 0.48f);
        nameInputRect.anchorMax = new Vector2(0.5f, 0.48f);
        nameInputRect.sizeDelta = new Vector2(300, 60);
        playerNameInput.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.Standard;
        playerNameInput.GetComponent<TMP_InputField>().characterLimit = 20;
        playerNameInput.SetActive(false);
        
        GameObject startButton = CreateButton("StartButton", panel.transform, "Start Game", new Color(0.2f, 0.6f, 0.3f, 1f));
        RectTransform startRect = startButton.GetComponent<RectTransform>();
        startRect.anchorMin = new Vector2(0.5f, 0.3f);
        startRect.anchorMax = new Vector2(0.5f, 0.3f);
        startRect.sizeDelta = new Vector2(300, 80);
        
        GameObject backButton = CreateButton("BackButton", panel.transform, "Back", new Color(0.4f, 0.4f, 0.5f, 1f));
        RectTransform backRect = backButton.GetComponent<RectTransform>();
        backRect.anchorMin = new Vector2(0.5f, 0.15f);
        backRect.anchorMax = new Vector2(0.5f, 0.15f);
        backRect.sizeDelta = new Vector2(200, 60);
        backButton.GetComponentInChildren<TextMeshProUGUI>().fontSize = 28;
        
        return panel;
    }
    
    private static GameObject CreateBotThinkingIndicator(Transform gamePanel)
    {
        if (gamePanel == null)
        {
            Debug.LogWarning("GamePanel not found for bot thinking indicator");
            return null;
        }
        
        GameObject indicator = new GameObject("BotThinkingIndicator");
        indicator.transform.SetParent(gamePanel, false);
        
        RectTransform rect = indicator.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(400, 100);
        
        Image bg = indicator.AddComponent<Image>();
        bg.color = new Color(0, 0, 0, 0.8f);
        
        GameObject text = CreateText("BotThinkingText", indicator.transform, "Bot is thinking...", 36);
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        text.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        text.GetComponent<TextMeshProUGUI>().color = Color.white;
        
        indicator.SetActive(false);
        
        return indicator;
    }
    
    private static GameObject CreateIAPManager()
    {
        GameObject existing = GameObject.Find("IAPManager");
        if (existing != null)
        {
            Debug.Log("IAPManager already exists");
            return existing;
        }
        
        GameObject iapManager = new GameObject("IAPManager");
        iapManager.AddComponent<IAPManager>();
        
        return iapManager;
    }
    
    private static GameObject CreateBotPlayer()
    {
        GameObject managers = GameObject.Find("Managers");
        if (managers == null)
        {
            managers = new GameObject("Managers");
        }
        
        BotPlayer existingBot = managers.GetComponentInChildren<BotPlayer>();
        if (existingBot != null)
        {
            Debug.Log("BotPlayer already exists");
            return existingBot.gameObject;
        }
        
        GameObject botPlayer = new GameObject("BotPlayer");
        botPlayer.transform.SetParent(managers.transform);
        botPlayer.AddComponent<BotPlayer>();
        
        return botPlayer;
    }
    
    private static void LinkNewReferences(GameObject canvas, GameObject modeSelectPanel, GameObject playerSetupPanel, 
        GameObject botThinkingIndicator, GameObject botPlayerObj)
    {
        Transform managers = canvas.transform.Find("Managers");
        if (managers == null) return;
        
        GameUIController uiController = managers.GetComponentInChildren<GameUIController>();
        GameManager gameManager = managers.GetComponentInChildren<GameManager>();
        GridManager gridManager = managers.GetComponentInChildren<GridManager>();
        
        if (uiController != null)
        {
            SerializedObject uiSO = new SerializedObject(uiController);
            
            uiSO.FindProperty("modeSelectPanel").objectReferenceValue = modeSelectPanel;
            uiSO.FindProperty("playerSetupPanel").objectReferenceValue = playerSetupPanel;
            
            Transform modePanel = modeSelectPanel.transform;
            uiSO.FindProperty("vsBotButton").objectReferenceValue = modePanel.Find("VsBotButton")?.GetComponent<Button>();
            uiSO.FindProperty("vsPlayersButton").objectReferenceValue = modePanel.Find("VsPlayersButton")?.GetComponent<Button>();
            uiSO.FindProperty("vsPlayersButtonText").objectReferenceValue = modePanel.Find("VsPlayersButton/Text")?.GetComponent<TextMeshProUGUI>();
            uiSO.FindProperty("restorePurchaseButton").objectReferenceValue = modePanel.Find("RestorePurchaseButton")?.GetComponent<Button>();
            uiSO.FindProperty("purchaseStatusText").objectReferenceValue = modePanel.Find("PurchaseStatusText")?.GetComponent<TextMeshProUGUI>();
            
            Transform setupPanel = playerSetupPanel.transform;
            uiSO.FindProperty("playerCountInput").objectReferenceValue = setupPanel.Find("PlayerCountInput")?.GetComponent<TMP_InputField>();
            uiSO.FindProperty("playerNameInput").objectReferenceValue = setupPanel.Find("PlayerNameInput")?.GetComponent<TMP_InputField>();
            uiSO.FindProperty("startButton").objectReferenceValue = setupPanel.Find("StartButton")?.GetComponent<Button>();
            uiSO.FindProperty("backButton").objectReferenceValue = setupPanel.Find("BackButton")?.GetComponent<Button>();
            uiSO.FindProperty("setupTitleText").objectReferenceValue = setupPanel.Find("SetupTitleText")?.GetComponent<TextMeshProUGUI>();
            
            if (botThinkingIndicator != null)
            {
                uiSO.FindProperty("botThinkingIndicator").objectReferenceValue = botThinkingIndicator;
                uiSO.FindProperty("botThinkingText").objectReferenceValue = botThinkingIndicator.transform.Find("BotThinkingText")?.GetComponent<TextMeshProUGUI>();
            }
            
            uiSO.ApplyModifiedProperties();
        }
        
        if (gameManager != null && botPlayerObj != null)
        {
            SerializedObject gmSO = new SerializedObject(gameManager);
            gmSO.FindProperty("botPlayer").objectReferenceValue = botPlayerObj.GetComponent<BotPlayer>();
            gmSO.ApplyModifiedProperties();
        }
        
        BotPlayer botPlayer = botPlayerObj?.GetComponent<BotPlayer>();
        if (botPlayer != null && gridManager != null)
        {
            SerializedObject botSO = new SerializedObject(botPlayer);
            botSO.FindProperty("gridManager").objectReferenceValue = gridManager;
            botSO.ApplyModifiedProperties();
        }
    }
    
    private static GameObject CreateText(string name, Transform parent, string text, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        return textObj;
    }
    
    private static GameObject CreateButton(string name, Transform parent, string text, Color color)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 60);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = color;
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(color.r + 0.1f, color.g + 0.1f, color.b + 0.1f, 1f);
        colors.pressedColor = new Color(color.r - 0.1f, color.g - 0.1f, color.b - 0.1f, 1f);
        button.colors = colors;
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 32;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        
        return buttonObj;
    }
    
    private static GameObject CreateInputField(string name, Transform parent, string defaultText)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent, false);
        RectTransform rect = inputObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 50);
        
        Image image = inputObj.AddComponent<Image>();
        image.color = new Color(0.25f, 0.25f, 0.3f, 1f);
        
        GameObject textArea = new GameObject("Text Area");
        textArea.transform.SetParent(inputObj.transform, false);
        RectTransform textAreaRect = textArea.AddComponent<RectTransform>();
        textAreaRect.anchorMin = Vector2.zero;
        textAreaRect.anchorMax = Vector2.one;
        textAreaRect.offsetMin = new Vector2(10, 5);
        textAreaRect.offsetMax = new Vector2(-10, -5);
        textArea.AddComponent<RectMask2D>();
        
        GameObject placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(textArea.transform, false);
        RectTransform phRect = placeholder.AddComponent<RectTransform>();
        phRect.anchorMin = Vector2.zero;
        phRect.anchorMax = Vector2.one;
        phRect.offsetMin = Vector2.zero;
        phRect.offsetMax = Vector2.zero;
        TextMeshProUGUI phText = placeholder.AddComponent<TextMeshProUGUI>();
        phText.text = defaultText;
        phText.fontSize = 28;
        phText.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        phText.alignment = TextAlignmentOptions.Center;
        
        GameObject textComponent = new GameObject("Text");
        textComponent.transform.SetParent(textArea.transform, false);
        RectTransform tcRect = textComponent.AddComponent<RectTransform>();
        tcRect.anchorMin = Vector2.zero;
        tcRect.anchorMax = Vector2.one;
        tcRect.offsetMin = Vector2.zero;
        tcRect.offsetMax = Vector2.zero;
        TextMeshProUGUI tcText = textComponent.AddComponent<TextMeshProUGUI>();
        tcText.fontSize = 28;
        tcText.color = Color.white;
        tcText.alignment = TextAlignmentOptions.Center;
        
        TMP_InputField inputField = inputObj.AddComponent<TMP_InputField>();
        inputField.textViewport = textAreaRect;
        inputField.textComponent = tcText;
        inputField.placeholder = phText;
        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        inputField.characterLimit = 1;
        inputField.text = defaultText;
        
        return inputObj;
    }
}
#endif
