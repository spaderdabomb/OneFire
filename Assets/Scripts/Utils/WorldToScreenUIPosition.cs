using ES3Types;
using UnityEngine;

public class WorldToScreenUIPosition : MonoBehaviour
{
    [SerializeField] private float offsetX;
    [SerializeField] private float offsetY;
    [SerializeField] private Transform positionTransform;
    private RectTransform _rectTransform;
    public Camera perspectiveCamera;   // The perspective camera in world space
    public Camera uiCamera;            // The orthographic camera rendering the Canvas

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
        

    private void LateUpdate()
    {
        if (positionTransform != null)
            return;

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(positionTransform.position);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform.parent as RectTransform, screenPoint, uiCamera, out Vector2 canvasPosition);

        _rectTransform.anchoredPosition = new Vector2(canvasPosition.x + offsetX, canvasPosition.y + offsetY);

    }
}