using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveToScreenPointUGUI : MonoBehaviour
{
    [SerializeField] private RectTransform uiElement;
    [SerializeField] private Transform positionTransform;
    [SerializeField] private float yoffset;

    void LateUpdate()
    {
        if (positionTransform == null)
            return;

        if (uiElement != null)
        {
            Vector3 worldPosition = positionTransform.position;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(
                new Vector3(worldPosition.x, worldPosition.y + yoffset, worldPosition.z)
            );
            uiElement.position = screenPosition;
        }
    }
}
