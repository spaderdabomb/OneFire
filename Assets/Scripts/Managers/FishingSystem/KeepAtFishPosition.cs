using System;
using UnityEngine;

public class KeepAtScreenPoint : MonoBehaviour
{
    public RectTransform uiElement;
    public float yOffset = 0f;
    
    private Camera _mainCamera;
    private Canvas _canvas;
    private Transform _worldObject;

    private float _baseScreenHeight = 1080f;

    private void Start()
    {
        _mainCamera = GameObjectManager.Instance.playerCamera;
        _canvas = GetComponentInParent<Canvas>();
        _worldObject = FishingManager.Instance.currentFishingPositionTransform;
    }

    void LateUpdate()
    {
        if (_worldObject == null || !_worldObject.gameObject.activeSelf)
            return;

        Vector3 screenPosition = _mainCamera.WorldToScreenPoint(_worldObject.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, screenPosition, _canvas.worldCamera, out Vector2 localPosition);
        float scaledYOffset = yOffset * (Screen.height / _baseScreenHeight);
        localPosition.y += scaledYOffset;
        uiElement.localPosition = localPosition;
    }
}