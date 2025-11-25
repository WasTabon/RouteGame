# 711 Route Builder - Инструкция по настройке сцены

## 1. Создание TileData

1. В Unity: `711Route → Create Tile Set` в верхнем меню
2. Откроются созданные ассеты в `Assets/TileData/`
3. Для каждого TileData назначь соответствующий спрайт

### Выходы дорог (roadExits):
- Индекс 0 = Up (вверх)
- Индекс 1 = Right (вправо)  
- Индекс 2 = Down (вниз)
- Индекс 3 = Left (влево)

## 2. Префабы

### TilePrefab (размещённый тайл):
- GameObject с RectTransform
- Image (для отображения спрайта)
- PlacedTile компонент

### GridSlotPrefab (ячейка для размещения):
- GameObject с RectTransform
- Image (полупрозрачный фон)
- GridSlot компонент

### PlayerScorePrefab:
- GameObject с TextMeshProUGUI

## 3. Иерархия сцены

```
Canvas (Screen Space - Overlay)
├── StartPanel
│   ├── PlayerCountInput (TMP_InputField)
│   └── StartButton (Button)
│
├── GamePanel
│   ├── GridViewport (с Mask)
│   │   └── GridContainer (RectTransform) ← сюда кладутся тайлы
│   │
│   ├── CurrentTilePanel
│   │   ├── TilePreview (Image) ← для превью текущего тайла
│   │   ├── RotateLeftButton
│   │   └── RotateRightButton
│   │
│   ├── InfoPanel
│   │   ├── CurrentPlayerText
│   │   ├── RemainingTilesText
│   │   └── MessageText
│   │
│   └── PlayerScoresContainer (Vertical Layout Group)
│
├── EndPanel
│   ├── WinnerText
│   └── RestartButton
│
└── Managers
    ├── GameManager
    ├── GridManager
    ├── DeckManager
    ├── RouteChecker
    └── GameUIController
```

## 4. Настройка компонентов

### GameManager:
- gridManager → GridManager
- deckManager → DeckManager  
- routeChecker → RouteChecker
- startTile → любой TileData (например Crossroad)
- playerCount → 2-4

### GridManager:
- gridContainer → GridContainer
- tilePrefab → TilePrefab
- gridSlotPrefab → GridSlotPrefab
- tileSize → 100 (или другой размер)

### DeckManager:
- tileTypes → все созданные TileData
- Настрой количество каждого типа

### GameUIController:
- Привяжи все UI элементы

### GridPanZoom (на GridViewport):
- gridContainer → GridContainer

## 5. Спрайты тайлов

Создай спрайты размером 128x128 или 256x256:

- **Straight**: дорога сверху вниз (|)
- **Turn**: дорога сверху направо (⌐)
- **TJunction**: дорога сверху, справа, снизу (├)
- **Crossroad**: дорога во все стороны (+)
- **DeadEnd**: дорога только сверху (╵)

## 6. Canvas Scaler

Для адаптации под телефон:
- UI Scale Mode: Scale With Screen Size
- Reference Resolution: 1080 x 1920
- Match Width Or Height: 0.5
