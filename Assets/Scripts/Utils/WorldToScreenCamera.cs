using UnityEngine;

public class WorldToScreenCamera : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform uiElement;
    public Transform worldObject;
    public Canvas canvas;
    public float yOffset = 0f;

    private float baseScreenHeight = 1080f;

    void LateUpdate()
    {
        if (worldObject == null || !worldObject.gameObject.activeSelf)
            return;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldObject.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPosition, canvas.worldCamera, out Vector2 localPosition);
        float scaledYOffset = yOffset * (Screen.height / baseScreenHeight);
        localPosition.y += scaledYOffset;
        uiElement.localPosition = localPosition;
    }
}