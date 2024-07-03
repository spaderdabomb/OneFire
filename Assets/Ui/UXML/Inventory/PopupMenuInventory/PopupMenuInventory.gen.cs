// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class PopupMenuInventory
{
    private VisualElement popupContainer;
    private VisualElement popupHeader;
    private Label itemNameLabel;
    private VisualElement itemTypeIcon;
    private VisualElement dividerHeader;
    private Label itemDescriptionLabel;
    private VisualElement dividerStats;
    private VisualElement statsListContainer;
    private VisualElement dividerBottom;
    private VisualElement popupBottomContainer;
    private VisualElement shopSellContainer;
    private VisualElement shopSellIcon;
    private Label shopSellLabel;
    private Label itemTypeLabel;
    
    protected void AssignQueryResults(VisualElement root)
    {
        popupContainer = root.Q<VisualElement>("PopupContainer");
        popupHeader = root.Q<VisualElement>("PopupHeader");
        itemNameLabel = root.Q<Label>("ItemNameLabel");
        itemTypeIcon = root.Q<VisualElement>("ItemTypeIcon");
        dividerHeader = root.Q<VisualElement>("DividerHeader");
        itemDescriptionLabel = root.Q<Label>("ItemDescriptionLabel");
        dividerStats = root.Q<VisualElement>("DividerStats");
        statsListContainer = root.Q<VisualElement>("StatsListContainer");
        dividerBottom = root.Q<VisualElement>("DividerBottom");
        popupBottomContainer = root.Q<VisualElement>("PopupBottomContainer");
        shopSellContainer = root.Q<VisualElement>("ShopSellContainer");
        shopSellIcon = root.Q<VisualElement>("ShopSellIcon");
        shopSellLabel = root.Q<Label>("ShopSellLabel");
        itemTypeLabel = root.Q<Label>("ItemTypeLabel");
    }
}
