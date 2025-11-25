#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SceneSetup : MonoBehaviour
{
    [MenuItem("711Route/Setup Complete Scene")]
    public static void SetupScene()
    {
        CreateTileDataAssets();
        
        GameObject canvas = CreateCanvas();
        GameObject eventSystem = CreateEventSystem();
        
        Transform canvasTransform = canvas.transform;
        
        GameObject startPanel = CreateStartPanel(canvasTransform);
        GameObject gamePanel = CreateGamePanel(canvasTransform);
        GameObject endPanel = CreateEndPanel(canvasTransform);
        
        GameObject managers = CreateManagers(canvasTransform);
        
        GameObject tilePrefab = CreateTilePrefab();
        GameObject gridSlotPrefab = CreateGridSlotPrefab();
        GameObject playerScorePrefab = CreatePlayerScorePrefab();
        
        LinkAllReferences(managers, gamePanel, startPanel, endPanel, tilePrefab, gridSlotPrefab, playerScorePrefab, canvas);
        
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        endPanel.SetActive(false);
        
        Selection.activeGameObject = canvas;
        
        Debug.Log("Scene setup complete! Press Play to test.");
    }
    
    private static void CreateTileDataAssets()
    {
        string path = "Assets/TileData/";
        
        if (!AssetDatabase.IsValidFolder("Assets/TileData"))
        {
            AssetDatabase.CreateFolder("Assets", "TileData");
        }
        
        if (AssetDatabase.LoadAssetAtPath<TileData>(path + "Straight.asset") == null)
        {
            CreateTile("Straight", TileType.Straight, new bool[] { true, false, true, false }, path);
            CreateTile("Turn", TileType.Turn, new bool[] { true, true, false, false }, path);
            CreateTile("TJunction", TileType.TJunction, new bool[] { true, true, true, false }, path);
            CreateTile("Crossroad", TileType.Crossroad, new bool[] { true, true, true, true }, path);
            CreateTile("DeadEnd", TileType.DeadEnd, new bool[] { true, false, false, false }, path);
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
    
    private static void CreateTile(string name, TileType type, bool[] exits, string path)
    {
        TileData tile = ScriptableObject.CreateInstance<TileData>();
        tile.tileType = type;
        tile.roadExits = exits;
        AssetDatabase.CreateAsset(tile, path + name + ".asset");
    }
    
    private static GameObject CreateCanvas()
    {
        GameObject canvasObj = new GameObject("GameCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        return canvasObj;
    }
    
    private static GameObject CreateEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null)
            return FindObjectOfType<EventSystem>().gameObject;
            
        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();
        return eventSystem;
    }
    
    private static GameObject CreateStartPanel(Transform parent)
    {
        GameObject panel = CreatePanel("StartPanel", parent);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        Image bg = panel.GetComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.2f, 1f);
        
        GameObject title = CreateText("Title", panel.transform, "711 Route Builder", 72);
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(800, 100);
        title.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        
        GameObject inputLabel = CreateText("InputLabel", panel.transform, "Number of Players (2-4):", 36);
        RectTransform labelRect = inputLabel.GetComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 0.5f);
        labelRect.anchorMax = new Vector2(0.5f, 0.5f);
        labelRect.anchoredPosition = new Vector2(0, 50);
        labelRect.sizeDelta = new Vector2(500, 50);
        
        GameObject inputField = CreateInputField("PlayerCountInput", panel.transform);
        RectTransform inputRect = inputField.GetComponent<RectTransform>();
        inputRect.anchorMin = new Vector2(0.5f, 0.5f);
        inputRect.anchorMax = new Vector2(0.5f, 0.5f);
        inputRect.anchoredPosition = new Vector2(0, -20);
        inputRect.sizeDelta = new Vector2(200, 60);
        inputField.GetComponent<TMP_InputField>().text = "2";
        
        GameObject startButton = CreateButton("StartButton", panel.transform, "Start Game");
        RectTransform buttonRect = startButton.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.35f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.35f);
        buttonRect.sizeDelta = new Vector2(300, 80);
        
        return panel;
    }
    
    private static GameObject CreateGamePanel(Transform parent)
    {
        GameObject panel = CreatePanel("GamePanel", parent);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.15f, 1f);
        
        GameObject gridViewport = CreatePanel("GridViewport", panel.transform);
        RectTransform viewportRect = gridViewport.GetComponent<RectTransform>();
        viewportRect.anchorMin = new Vector2(0, 0.25f);
        viewportRect.anchorMax = new Vector2(1, 0.85f);
        viewportRect.offsetMin = new Vector2(20, 0);
        viewportRect.offsetMax = new Vector2(-20, 0);
        gridViewport.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.25f, 1f);
        gridViewport.AddComponent<Mask>().showMaskGraphic = true;
        gridViewport.AddComponent<GridPanZoom>();
        
        GameObject gridContainer = new GameObject("GridContainer");
        gridContainer.transform.SetParent(gridViewport.transform, false);
        RectTransform gridRect = gridContainer.AddComponent<RectTransform>();
        gridRect.anchorMin = new Vector2(0.5f, 0.5f);
        gridRect.anchorMax = new Vector2(0.5f, 0.5f);
        gridRect.sizeDelta = new Vector2(2000, 2000);
        gridRect.anchoredPosition = Vector2.zero;
        
        GameObject topBar = CreatePanel("TopBar", panel.transform);
        RectTransform topBarRect = topBar.GetComponent<RectTransform>();
        topBarRect.anchorMin = new Vector2(0, 0.85f);
        topBarRect.anchorMax = new Vector2(1, 1f);
        topBarRect.offsetMin = Vector2.zero;
        topBarRect.offsetMax = Vector2.zero;
        topBar.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.3f, 1f);
        
        GameObject currentPlayerText = CreateText("CurrentPlayerText", topBar.transform, "Current: Player 1", 32);
        RectTransform playerTextRect = currentPlayerText.GetComponent<RectTransform>();
        playerTextRect.anchorMin = new Vector2(0, 0.5f);
        playerTextRect.anchorMax = new Vector2(0, 0.5f);
        playerTextRect.anchoredPosition = new Vector2(150, 0);
        playerTextRect.sizeDelta = new Vector2(280, 50);
        
        GameObject remainingText = CreateText("RemainingTilesText", topBar.transform, "Tiles: 58", 28);
        RectTransform remainingRect = remainingText.GetComponent<RectTransform>();
        remainingRect.anchorMin = new Vector2(1, 0.5f);
        remainingRect.anchorMax = new Vector2(1, 0.5f);
        remainingRect.anchoredPosition = new Vector2(-100, 0);
        remainingRect.sizeDelta = new Vector2(150, 50);
        
        GameObject bottomPanel = CreatePanel("BottomPanel", panel.transform);
        RectTransform bottomRect = bottomPanel.GetComponent<RectTransform>();
        bottomRect.anchorMin = new Vector2(0, 0);
        bottomRect.anchorMax = new Vector2(1, 0.25f);
        bottomRect.offsetMin = Vector2.zero;
        bottomRect.offsetMax = Vector2.zero;
        bottomPanel.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 1f);
        
        GameObject tilePreviewPanel = CreatePanel("TilePreviewPanel", bottomPanel.transform);
        RectTransform previewPanelRect = tilePreviewPanel.GetComponent<RectTransform>();
        previewPanelRect.anchorMin = new Vector2(0, 0);
        previewPanelRect.anchorMax = new Vector2(0.4f, 1);
        previewPanelRect.offsetMin = new Vector2(20, 20);
        previewPanelRect.offsetMax = new Vector2(-10, -20);
        tilePreviewPanel.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.3f, 1f);
        
        GameObject tilePreview = new GameObject("CurrentTilePreview");
        tilePreview.transform.SetParent(tilePreviewPanel.transform, false);
        RectTransform tilePreviewRect = tilePreview.AddComponent<RectTransform>();
        tilePreviewRect.anchorMin = new Vector2(0.5f, 0.5f);
        tilePreviewRect.anchorMax = new Vector2(0.5f, 0.5f);
        tilePreviewRect.sizeDelta = new Vector2(120, 120);
        tilePreviewRect.anchoredPosition = new Vector2(-40, 0);
        Image tileImage = tilePreview.AddComponent<Image>();
        tileImage.color = Color.white;
        
        GameObject rotateLeft = CreateButton("RotateLeftButton", tilePreviewPanel.transform, "↺");
        RectTransform rotLeftRect = rotateLeft.GetComponent<RectTransform>();
        rotLeftRect.anchorMin = new Vector2(1, 0.5f);
        rotLeftRect.anchorMax = new Vector2(1, 0.5f);
        rotLeftRect.anchoredPosition = new Vector2(-80, -40);
        rotLeftRect.sizeDelta = new Vector2(60, 60);
        
        GameObject rotateRight = CreateButton("RotateRightButton", tilePreviewPanel.transform, "↻");
        RectTransform rotRightRect = rotateRight.GetComponent<RectTransform>();
        rotRightRect.anchorMin = new Vector2(1, 0.5f);
        rotRightRect.anchorMax = new Vector2(1, 0.5f);
        rotRightRect.anchoredPosition = new Vector2(-80, 40);
        rotRightRect.sizeDelta = new Vector2(60, 60);
        
        GameObject scoresPanel = CreatePanel("PlayerScoresPanel", bottomPanel.transform);
        RectTransform scoresPanelRect = scoresPanel.GetComponent<RectTransform>();
        scoresPanelRect.anchorMin = new Vector2(0.4f, 0);
        scoresPanelRect.anchorMax = new Vector2(1, 1);
        scoresPanelRect.offsetMin = new Vector2(10, 20);
        scoresPanelRect.offsetMax = new Vector2(-20, -20);
        scoresPanel.GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.3f, 1f);
        
        GameObject scoresContainer = new GameObject("PlayerScoresContainer");
        scoresContainer.transform.SetParent(scoresPanel.transform, false);
        RectTransform scoresContainerRect = scoresContainer.AddComponent<RectTransform>();
        scoresContainerRect.anchorMin = Vector2.zero;
        scoresContainerRect.anchorMax = Vector2.one;
        scoresContainerRect.offsetMin = new Vector2(10, 10);
        scoresContainerRect.offsetMax = new Vector2(-10, -10);
        VerticalLayoutGroup vlg = scoresContainer.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.spacing = 5;
        vlg.childControlHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = true;
        
        GameObject messageText = CreateText("MessageText", panel.transform, "", 28);
        RectTransform messageRect = messageText.GetComponent<RectTransform>();
        messageRect.anchorMin = new Vector2(0.5f, 0.25f);
        messageRect.anchorMax = new Vector2(0.5f, 0.25f);
        messageRect.anchoredPosition = new Vector2(0, 30);
        messageRect.sizeDelta = new Vector2(600, 50);
        messageText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        messageText.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        
        return panel;
    }
    
    private static GameObject CreateEndPanel(Transform parent)
    {
        GameObject panel = CreatePanel("EndPanel", parent);
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        panel.GetComponent<Image>().color = new Color(0.1f, 0.15f, 0.2f, 1f);
        
        GameObject winnerText = CreateText("WinnerText", panel.transform, "Player 1 Wins!\nScore: 5", 56);
        RectTransform winnerRect = winnerText.GetComponent<RectTransform>();
        winnerRect.anchorMin = new Vector2(0.5f, 0.6f);
        winnerRect.anchorMax = new Vector2(0.5f, 0.6f);
        winnerRect.sizeDelta = new Vector2(600, 200);
        winnerText.GetComponent<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        
        GameObject restartButton = CreateButton("RestartButton", panel.transform, "Play Again");
        RectTransform buttonRect = restartButton.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.35f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.35f);
        buttonRect.sizeDelta = new Vector2(300, 80);
        
        return panel;
    }
    
    private static GameObject CreateManagers(Transform parent)
    {
        GameObject managers = new GameObject("Managers");
        managers.transform.SetParent(parent, false);
        
        managers.AddComponent<GameManager>();
        managers.AddComponent<GridManager>();
        managers.AddComponent<DeckManager>();
        managers.AddComponent<RouteChecker>();
        managers.AddComponent<GameUIController>();
        
        return managers;
    }
    
    private static GameObject CreateTilePrefab()
    {
        string prefabPath = "Assets/Prefabs/";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "TilePrefab.prefab");
        if (existingPrefab != null) return existingPrefab;
        
        GameObject tile = new GameObject("TilePrefab");
        RectTransform rect = tile.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 100);
        
        Image image = tile.AddComponent<Image>();
        image.color = Color.white;
        
        tile.AddComponent<PlacedTile>();
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(tile, prefabPath + "TilePrefab.prefab");
        DestroyImmediate(tile);
        
        return prefab;
    }
    
    private static GameObject CreateGridSlotPrefab()
    {
        string prefabPath = "Assets/Prefabs/";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "GridSlotPrefab.prefab");
        if (existingPrefab != null) return existingPrefab;
        
        GameObject slot = new GameObject("GridSlotPrefab");
        RectTransform rect = slot.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 100);
        
        Image image = slot.AddComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.3f);
        
        slot.AddComponent<GridSlot>();
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(slot, prefabPath + "GridSlotPrefab.prefab");
        DestroyImmediate(slot);
        
        return prefab;
    }
    
    private static GameObject CreatePlayerScorePrefab()
    {
        string prefabPath = "Assets/Prefabs/";
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        GameObject existingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath + "PlayerScorePrefab.prefab");
        if (existingPrefab != null) return existingPrefab;
        
        GameObject scoreItem = new GameObject("PlayerScorePrefab");
        RectTransform rect = scoreItem.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 40);
        
        GameObject textObj = new GameObject("ScoreText");
        textObj.transform.SetParent(scoreItem.transform, false);
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "Player: 0";
        text.fontSize = 28;
        text.alignment = TextAlignmentOptions.Left;
        
        LayoutElement layout = scoreItem.AddComponent<LayoutElement>();
        layout.minHeight = 40;
        layout.preferredHeight = 40;
        
        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(scoreItem, prefabPath + "PlayerScorePrefab.prefab");
        DestroyImmediate(scoreItem);
        
        return prefab;
    }
    
    private static void LinkAllReferences(GameObject managers, GameObject gamePanel, GameObject startPanel, 
        GameObject endPanel, GameObject tilePrefab, GameObject gridSlotPrefab, GameObject playerScorePrefab, GameObject canvas)
    {
        GameManager gameManager = managers.GetComponent<GameManager>();
        GridManager gridManager = managers.GetComponent<GridManager>();
        DeckManager deckManager = managers.GetComponent<DeckManager>();
        RouteChecker routeChecker = managers.GetComponent<RouteChecker>();
        GameUIController uiController = managers.GetComponent<GameUIController>();
        
        SerializedObject gmSO = new SerializedObject(gameManager);
        gmSO.FindProperty("gridManager").objectReferenceValue = gridManager;
        gmSO.FindProperty("deckManager").objectReferenceValue = deckManager;
        gmSO.FindProperty("routeChecker").objectReferenceValue = routeChecker;
        
        TileData crossroad = AssetDatabase.LoadAssetAtPath<TileData>("Assets/TileData/Crossroad.asset");
        gmSO.FindProperty("startTile").objectReferenceValue = crossroad;
        gmSO.FindProperty("playerCount").intValue = 2;
        gmSO.ApplyModifiedProperties();
        
        Transform gridContainer = gamePanel.transform.Find("GridViewport/GridContainer");
        SerializedObject gridSO = new SerializedObject(gridManager);
        gridSO.FindProperty("gridContainer").objectReferenceValue = gridContainer;
        gridSO.FindProperty("tilePrefab").objectReferenceValue = tilePrefab;
        gridSO.FindProperty("gridSlotPrefab").objectReferenceValue = gridSlotPrefab;
        gridSO.FindProperty("tileSize").floatValue = 100f;
        gridSO.ApplyModifiedProperties();
        
        TileData[] allTiles = new TileData[5];
        allTiles[0] = AssetDatabase.LoadAssetAtPath<TileData>("Assets/TileData/Straight.asset");
        allTiles[1] = AssetDatabase.LoadAssetAtPath<TileData>("Assets/TileData/Turn.asset");
        allTiles[2] = AssetDatabase.LoadAssetAtPath<TileData>("Assets/TileData/TJunction.asset");
        allTiles[3] = AssetDatabase.LoadAssetAtPath<TileData>("Assets/TileData/Crossroad.asset");
        allTiles[4] = AssetDatabase.LoadAssetAtPath<TileData>("Assets/TileData/DeadEnd.asset");
        
        SerializedObject deckSO = new SerializedObject(deckManager);
        SerializedProperty tileTypesProp = deckSO.FindProperty("tileTypes");
        tileTypesProp.arraySize = 5;
        for (int i = 0; i < 5; i++)
        {
            tileTypesProp.GetArrayElementAtIndex(i).objectReferenceValue = allTiles[i];
        }
        deckSO.ApplyModifiedProperties();
        
        SerializedObject routeSO = new SerializedObject(routeChecker);
        routeSO.FindProperty("gridManager").objectReferenceValue = gridManager;
        routeSO.ApplyModifiedProperties();
        
        Transform bottomPanel = gamePanel.transform.Find("BottomPanel");
        Transform tilePreviewPanel = bottomPanel.Find("TilePreviewPanel");
        Transform topBar = gamePanel.transform.Find("TopBar");
        
        SerializedObject uiSO = new SerializedObject(uiController);
        uiSO.FindProperty("currentTileImage").objectReferenceValue = tilePreviewPanel.Find("CurrentTilePreview").GetComponent<Image>();
        uiSO.FindProperty("currentTileTransform").objectReferenceValue = tilePreviewPanel.Find("CurrentTilePreview").GetComponent<RectTransform>();
        uiSO.FindProperty("rotateLeftButton").objectReferenceValue = tilePreviewPanel.Find("RotateLeftButton").GetComponent<Button>();
        uiSO.FindProperty("rotateRightButton").objectReferenceValue = tilePreviewPanel.Find("RotateRightButton").GetComponent<Button>();
        uiSO.FindProperty("currentPlayerText").objectReferenceValue = topBar.Find("CurrentPlayerText").GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("playerScoresContainer").objectReferenceValue = bottomPanel.Find("PlayerScoresPanel/PlayerScoresContainer");
        uiSO.FindProperty("playerScorePrefab").objectReferenceValue = playerScorePrefab;
        uiSO.FindProperty("remainingTilesText").objectReferenceValue = topBar.Find("RemainingTilesText").GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("messageText").objectReferenceValue = gamePanel.transform.Find("MessageText").GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("startPanel").objectReferenceValue = startPanel;
        uiSO.FindProperty("gamePanel").objectReferenceValue = gamePanel;
        uiSO.FindProperty("endPanel").objectReferenceValue = endPanel;
        uiSO.FindProperty("winnerText").objectReferenceValue = endPanel.transform.Find("WinnerText").GetComponent<TextMeshProUGUI>();
        uiSO.FindProperty("playerCountInput").objectReferenceValue = startPanel.transform.Find("PlayerCountInput").GetComponent<TMP_InputField>();
        uiSO.FindProperty("startButton").objectReferenceValue = startPanel.transform.Find("StartButton").GetComponent<Button>();
        uiSO.FindProperty("restartButton").objectReferenceValue = endPanel.transform.Find("RestartButton").GetComponent<Button>();
        uiSO.ApplyModifiedProperties();
        
        GridPanZoom panZoom = gamePanel.transform.Find("GridViewport").GetComponent<GridPanZoom>();
        SerializedObject panZoomSO = new SerializedObject(panZoom);
        panZoomSO.FindProperty("gridContainer").objectReferenceValue = gridContainer;
        panZoomSO.ApplyModifiedProperties();
    }
    
    private static GameObject CreatePanel(string name, Transform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rect = panel.AddComponent<RectTransform>();
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        return panel;
    }
    
    private static GameObject CreateText(string name, Transform parent, string text, int fontSize)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        RectTransform rect = textObj.AddComponent<RectTransform>();
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        return textObj;
    }
    
    private static GameObject CreateButton(string name, Transform parent, string text)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        RectTransform rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 60);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.3f, 0.5f, 0.8f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.4f, 0.6f, 0.9f, 1f);
        colors.pressedColor = new Color(0.2f, 0.4f, 0.7f, 1f);
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
    
    private static GameObject CreateInputField(string name, Transform parent)
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
        phText.text = "2";
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
        
        return inputObj;
    }
}
#endif
