using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ThemeInitializer : MonoBehaviour
{
    [SerializeField] private ThemeStyleSheet customTheme;
    [SerializeField] private UIDocument uiDocument;
    
    void Start()
    {
        ApplyTheme();
    }
    
    void ApplyTheme()
    {
        if (customTheme != null && uiDocument != null)
        {
            uiDocument.rootVisualElement.styleSheets.Clear();
            uiDocument.rootVisualElement.styleSheets.Add(customTheme);
        }
    }
    
    // This ensures the theme is reapplied after domain reloads
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoad()
    {
        var themeManager = FindObjectOfType<ThemeInitializer>();
        if (themeManager != null)
        {
            themeManager.ApplyTheme(); //
        }
    }
}