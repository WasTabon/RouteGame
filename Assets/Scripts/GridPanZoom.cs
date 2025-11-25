using UnityEngine;
using UnityEngine.EventSystems;

public class GridPanZoom : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IScrollHandler
{
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private float minZoom = 0.5f;
    [SerializeField] private float maxZoom = 2f;
    [SerializeField] private float zoomSpeed = 0.1f;
    [SerializeField] private float panSpeed = 1f;
    
    private float currentZoom = 1f;
    private Vector2 lastPointerPosition;
    private bool isPanning;
    private int activeTouchCount;
    private float initialPinchDistance;
    private float initialZoom;
    
    private void Update()
    {
        HandleTouchInput();
    }
    
    private void HandleTouchInput()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);
            
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                initialPinchDistance = Vector2.Distance(touch0.position, touch1.position);
                initialZoom = currentZoom;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touch0.position, touch1.position);
                
                if (initialPinchDistance > 0)
                {
                    float zoomDelta = (currentDistance - initialPinchDistance) * zoomSpeed * 0.01f;
                    SetZoom(initialZoom + zoomDelta);
                }
                
                Vector2 centerDelta = (touch0.deltaPosition + touch1.deltaPosition) * 0.5f;
                Pan(centerDelta * panSpeed);
            }
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Input.touchCount <= 1)
        {
            isPanning = true;
            lastPointerPosition = eventData.position;
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isPanning = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (isPanning && Input.touchCount <= 1)
        {
            Vector2 delta = eventData.position - lastPointerPosition;
            Pan(delta * panSpeed);
            lastPointerPosition = eventData.position;
        }
    }
    
    public void OnScroll(PointerEventData eventData)
    {
        float zoomDelta = eventData.scrollDelta.y * zoomSpeed;
        SetZoom(currentZoom + zoomDelta);
    }
    
    private void SetZoom(float zoom)
    {
        currentZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        gridContainer.localScale = Vector3.one * currentZoom;
    }
    
    private void Pan(Vector2 delta)
    {
        gridContainer.anchoredPosition += delta;
    }
    
    public void ResetView()
    {
        currentZoom = 1f;
        gridContainer.localScale = Vector3.one;
        gridContainer.anchoredPosition = Vector2.zero;
    }
    
    public void CenterOnPosition(Vector2 worldPosition)
    {
        gridContainer.anchoredPosition = -worldPosition * currentZoom;
    }
}
