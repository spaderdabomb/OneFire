// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class UiGameManager
{
    private TemplateContainer optionsMenuUi;
    private TemplateContainer gameSceneUi;
    private TemplateContainer playerInteractionMenu;
    private TemplateContainer playerHotbarInventory;
    
    protected void AssignQueryResults(VisualElement root)
    {
        optionsMenuUi = root.Q<TemplateContainer>("OptionsMenuUi");
        gameSceneUi = root.Q<TemplateContainer>("GameSceneUi");
        playerInteractionMenu = root.Q<TemplateContainer>("PlayerInteractionMenu");
        playerHotbarInventory = root.Q<TemplateContainer>("PlayerHotbarInventory");
    }
}
