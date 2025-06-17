using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UiToolkitUtils
{
    public static VisualElement GetHighestHierarchyElement(VisualElement element)
    {
        if (element == null) return null;

        while (element.hierarchy.parent != null)
        {
            element = element.hierarchy.parent;
        }

        return element;
    }
    
    public static void AddElementRelativeTo(VisualElement activeElement, VisualElement rootElement, VisualElement targetElement)
    {
        // Create your new overlay element
        activeElement.style.position = Position.Absolute;
    
        // Get the world bounds of the target element
        Rect worldBounds = targetElement.worldBound;
        
        // Convert world position to local coordinates relative to root
        Vector2 localPosition = rootElement.WorldToLocal(worldBounds.position);
        
        // Position the overlay element
        activeElement.style.left = localPosition.x;
        activeElement.style.top = localPosition.y;
    
        // Add to root so it appears above everything
        rootElement.Add(activeElement);
    }
    
    public static void SetPositionToMouse(VisualElement rootElement, Vector2 offset = default(Vector2))
    {
        if (rootElement.panel == null)
        {
            Debug.LogError("No root panel assigned");
            return;
        }
            
        Vector3 mousePos = Input.mousePosition;
        Vector2 panelPosition = RuntimePanelUtils.ScreenToPanel(
            rootElement.panel, 
            new Vector2(mousePos.x, Screen.height - mousePos.y)
        );
    
        rootElement.style.left = panelPosition.x + offset.x;
        rootElement.style.top = panelPosition.y + offset.y;
    }
    
    public static void SetPickingModeIgnoreRecursive(VisualElement root)
    {
        root.pickingMode = PickingMode.Ignore;

        foreach (var child in root.Children())
        {
            SetPickingModeIgnoreRecursive(child);
        }
    }
}
