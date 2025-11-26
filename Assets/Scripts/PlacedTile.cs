using UnityEngine;
using UnityEngine.UI;

public class PlacedTile : MonoBehaviour
{
    public TileData tileData;
    public int rotation;
    public Vector2Int gridPosition;
    public bool isMarkedComplete;
    public Color playerColor;
    
    private Image image;
    private Image backgroundImage;
    private RectTransform rectTransform;
    
    public void Initialize(TileData data, Vector2Int pos, int rot = 0, Color playerCol = default)
    {
        tileData = data;
        gridPosition = pos;
        rotation = rot;
        playerColor = playerCol == default ? Color.white : playerCol;
        
        rectTransform = GetComponent<RectTransform>();
        
        image = GetComponent<Image>();
        if (image == null)
        {
            image = gameObject.AddComponent<Image>();
        }
        
        if (image != null && data.sprite != null)
        {
            image.sprite = data.sprite;
        }
        
        rectTransform.localRotation = Quaternion.Euler(0, 0, -rotation * 90);
    }
    
    public void SetBackgroundImage(Image bgImage)
    {
        backgroundImage = bgImage;
        if (backgroundImage != null)
        {
            Color bgColor = playerColor;
            bgColor.a = 0.3f;
            backgroundImage.color = bgColor;
        }
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
        
        if (backgroundImage != null)
        {
            Color bgColor = playerColor;
            bgColor.r *= 0.7f;
            bgColor.g *= 0.7f;
            bgColor.b *= 0.7f;
            bgColor.a = 0.3f;
            backgroundImage.color = bgColor;
        }
    }
}