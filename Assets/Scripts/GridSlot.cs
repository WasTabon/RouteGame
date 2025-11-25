using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GridSlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Vector2Int gridPosition;
    
    public static event Action<Vector2Int> OnSlotClicked;
    
    private Image image;
    private Color normalColor = new Color(1f, 1f, 1f, 0.3f);
    private Color hoverColor = new Color(0.5f, 1f, 0.5f, 0.5f);
    
    public void Initialize(Vector2Int pos)
    {
        gridPosition = pos;
        image = GetComponent<Image>();
        if (image != null)
        {
            image.color = normalColor;
        }
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotClicked?.Invoke(gridPosition);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (image != null)
        {
            image.color = hoverColor;
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (image != null)
        {
            image.color = normalColor;
        }
    }
    
    public void SetValid(bool valid)
    {
        if (image != null)
        {
            image.color = valid ? new Color(0.5f, 1f, 0.5f, 0.5f) : new Color(1f, 0.5f, 0.5f, 0.5f);
        }
    }
}
