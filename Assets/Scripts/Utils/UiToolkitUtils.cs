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
}
