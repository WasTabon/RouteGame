using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TilePreviewDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Canvas canvas;
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image image;
    private Vector2 originalPosition;
    private Transform originalParent;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        image = GetComponent<Image>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    
    public void UpdatePreview(TileData tile, int rotation)
    {
        if (tile != null && tile.sprite != null)
        {
            image.sprite = tile.sprite;
            image.enabled = true;
            rectTransform.localRotation = Quaternion.Euler(0, 0, -rotation * 90);
        }
        else
        {
            image.enabled = false;
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
        
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.7f;
        
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPosition;
        
        Vector2 dropPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridManager.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out dropPosition
        );
        
        Vector2Int gridPos = gridManager.UIToGridPosition(dropPosition);
        GameManager.Instance.TryPlaceTile(gridPos);
    }
}
