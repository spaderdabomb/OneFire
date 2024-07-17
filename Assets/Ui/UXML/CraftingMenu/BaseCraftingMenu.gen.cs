// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class BaseCraftingMenu
{
    private VisualElement craftingMenuRoot;
    private VisualElement craftingMenuContainer;
    private VisualElement mainContent;
    private VisualElement leftContainer;
    private TemplateContainer craftingSlotContainer;
    private VisualElement rightContainer;
    private VisualElement slotPreviewContainer;
    private VisualElement slotPreviewIcon;
    private Label slotPreviewLabel;
    private VisualElement requiredMateialsContainer;
    private Label requiredMaterialsHeader;
    private VisualElement craftingButtonsContainer;
    private VisualElement selectAmountContainer;
    private Button minusButton;
    private Label craftAmountLabel;
    private Button plusButton;
    private Button maxButton;
    private Button craftButton;
    
    protected void AssignQueryResults(VisualElement root)
    {
        craftingMenuRoot = root.Q<VisualElement>("CraftingMenuRoot");
        craftingMenuContainer = root.Q<VisualElement>("CraftingMenuContainer");
        mainContent = root.Q<VisualElement>("MainContent");
        leftContainer = root.Q<VisualElement>("LeftContainer");
        craftingSlotContainer = root.Q<TemplateContainer>("CraftingSlotContainer");
        rightContainer = root.Q<VisualElement>("RightContainer");
        slotPreviewContainer = root.Q<VisualElement>("SlotPreviewContainer");
        slotPreviewIcon = root.Q<VisualElement>("SlotPreviewIcon");
        slotPreviewLabel = root.Q<Label>("SlotPreviewLabel");
        requiredMateialsContainer = root.Q<VisualElement>("RequiredMateialsContainer");
        requiredMaterialsHeader = root.Q<Label>("RequiredMaterialsHeader");
        craftingButtonsContainer = root.Q<VisualElement>("CraftingButtonsContainer");
        selectAmountContainer = root.Q<VisualElement>("SelectAmountContainer");
        minusButton = root.Q<Button>("MinusButton");
        craftAmountLabel = root.Q<Label>("CraftAmountLabel");
        plusButton = root.Q<Button>("PlusButton");
        maxButton = root.Q<Button>("MaxButton");
        craftButton = root.Q<Button>("CraftButton");
    }
}
