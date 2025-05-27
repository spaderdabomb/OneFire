using UnityEngine;

public class WorldToScreenUI : MonoBehaviour
{
    public Camera mainCamera;      
    public RectTransform uiElement;
    public Transform worldObject;  
    public float yOffset = 0f;

    private float baseScreenHeight = 1080f;

    void LateUpdate()
    {
        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldObject.position);
        float scaledYOffset = yOffset * (Screen.height / baseScreenHeight);
        screenPosition.y += scaledYOffset;
        uiElement.position = screenPosition;
    }
}