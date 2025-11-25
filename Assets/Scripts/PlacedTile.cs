using UnityEngine;
using UnityEngine.UI;

public class PlacedTile : MonoBehaviour
{
    public TileData tileData;
    public int rotation;
    public Vector2Int gridPosition;
    public bool isMarkedComplete;
    
    private Image image;
    private RectTransform rectTransform;
    
    public void Initialize(TileData data, Vector2Int pos, int rot = 0)
    {
        tileData = data;
        gridPosition = pos;
        rotation = rot;
        
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        
        if (image != null && data.sprite != null)
        {
            image.sprite = data.sprite;
        }
        
        rectTransform.localRotation = Quaternion.Euler(0, 0, -rotation * 90);
    }
    
    public bool HasExit(Direction dir)
    {
        int rotatedDir = ((int)dir - rotation + 4) % 4;
        return tileData.roadExits[rotatedDir];
    }
    
    public bool[] GetCurrentExits()
    {
        return tileData.GetRotatedExits(rotation);
    }
    
    public void MarkAsComplete()
    {
        isMarkedComplete = true;
        if (image != null)
        {
            image.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        }
    }
}
