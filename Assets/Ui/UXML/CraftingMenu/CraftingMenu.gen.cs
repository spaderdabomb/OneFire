// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class CraftingMenu
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
    private Label requiredMaterialsHeader;
    private VisualElement requiredMaterialsContainer;
    private VisualElement craftingButtonsContainer;
    private VisualElement selectAmountContainer;
    private VisualElement ownedContainer;
    private Label ownedLabel;
    private Label ownedQuantityLabel;
    private VisualElement craftAmountContainer;
    private Button minusButton;
    private Label craftAmountLabel;
    private Button plusButton;
    private VisualElement timeContainer;
    private VisualElement timeIcon;
    private Label timeQuantityLabel;
    private Button minButton;
    private Button maxButton;
    private Button craftButton;
    private VisualElement craftProgressContainer;
    private ProgressBar progressBar;
    private VisualElement timeLeftContainer;
    private VisualElement singleTimeIcon;
    private Label singleTimeQuantityLabel;
    private Button cancelCraftButton;
    
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
        requiredMaterialsHeader = root.Q<Label>("RequiredMaterialsHeader");
        requiredMaterialsContainer = root.Q<VisualElement>("RequiredMaterialsContainer");
        craftingButtonsContainer = root.Q<VisualElement>("CraftingButtonsContainer");
        selectAmountContainer = root.Q<VisualElement>("SelectAmountContainer");
        ownedContainer = root.Q<VisualElement>("OwnedContainer");
        ownedLabel = root.Q<Label>("OwnedLabel");
        ownedQuantityLabel = root.Q<Label>("OwnedQuantityLabel");
        craftAmountContainer = root.Q<VisualElement>("CraftAmountContainer");
        minusButton = root.Q<Button>("MinusButton");
        craftAmountLabel = root.Q<Label>("CraftAmountLabel");
        plusButton = root.Q<Button>("PlusButton");
        timeContainer = root.Q<VisualElement>("TimeContainer");
        timeIcon = root.Q<VisualElement>("TimeIcon");
        timeQuantityLabel = root.Q<Label>("TimeQuantityLabel");
        minButton = root.Q<Button>("MinButton");
        maxButton = root.Q<Button>("MaxButton");
        craftButton = root.Q<Button>("CraftButton");
        craftProgressContainer = root.Q<VisualElement>("CraftProgressContainer");
        progressBar = root.Q<ProgressBar>("ProgressBar");
        timeLeftContainer = root.Q<VisualElement>("TimeLeftContainer");
        singleTimeIcon = root.Q<VisualElement>("SingleTimeIcon");
        singleTimeQuantityLabel = root.Q<Label>("SingleTimeQuantityLabel");
        cancelCraftButton = root.Q<Button>("CancelCraftButton");
    }
}
