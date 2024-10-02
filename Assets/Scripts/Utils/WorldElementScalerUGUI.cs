using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldElementScalerUGUI : MonoBehaviour
{
    public Camera mainCamera;
    public float targetSize = 1f;
    public float minDistance = 1f;
    public float maxDistance = 100f;

    private RectTransform rectTransform;
    private float initialScale;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        rectTransform = GetComponent<RectTransform>();
        initialScale = rectTransform.localScale.x;
    }

    void Update()
    {
        if (mainCamera == null || rectTransform == null) return;

        float distance = Vector3.Distance(mainCamera.transform.position, transform.position);
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        float scaleFactor = distance / targetSize;
        Vector3 newScale = Vector3.one * (scaleFactor * initialScale);

        rectTransform.localScale = newScale;
        transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                         mainCamera.transform.rotation * Vector3.up);
    }
}