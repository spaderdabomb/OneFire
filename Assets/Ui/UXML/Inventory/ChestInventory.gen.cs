// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class ChestInventory
{
    private VisualElement chestMenuRoot;
    private VisualElement chestContainerRoot;
    private Label chestHeaderLabel;
    private VisualElement chestContainer;
    private VisualElement buttonContainer;
    private Button allToInventoryButton;
    private VisualElement allRightIcon;
    private Label allRightLabel;
    private Button allToChestButton;
    private VisualElement allLeftIcon;
    private Label allLeftLabel;
    private Button sameToChestButton;
    private VisualElement sameToChestIcon;
    private Label sameToChestLabel;
    private Button sameToInventoryButton;
    private VisualElement sameToInventoryIcon;
    private Label sameToInventoryLabel;
    private VisualElement playerContainerRoot;
    private Label playerHeaderLabel;
    private VisualElement playerContainer;
    private TemplateContainer exitButton;
    
    protected void AssignQueryResults(VisualElement root)
    {
        chestMenuRoot = root.Q<VisualElement>("ChestMenuRoot");
        chestContainerRoot = root.Q<VisualElement>("ChestContainerRoot");
        chestHeaderLabel = root.Q<Label>("ChestHeaderLabel");
        chestContainer = root.Q<VisualElement>("ChestContainer");
        buttonContainer = root.Q<VisualElement>("ButtonContainer");
        allToInventoryButton = root.Q<Button>("AllToInventoryButton");
        allRightIcon = root.Q<VisualElement>("AllRightIcon");
        allRightLabel = root.Q<Label>("AllRightLabel");
        allToChestButton = root.Q<Button>("AllToChestButton");
        allLeftIcon = root.Q<VisualElement>("AllLeftIcon");
        allLeftLabel = root.Q<Label>("AllLeftLabel");
        sameToChestButton = root.Q<Button>("SameToChestButton");
        sameToChestIcon = root.Q<VisualElement>("SameToChestIcon");
        sameToChestLabel = root.Q<Label>("SameToChestLabel");
        sameToInventoryButton = root.Q<Button>("SameToInventoryButton");
        sameToInventoryIcon = root.Q<VisualElement>("SameToInventoryIcon");
        sameToInventoryLabel = root.Q<Label>("SameToInventoryLabel");
        playerContainerRoot = root.Q<VisualElement>("PlayerContainerRoot");
        playerHeaderLabel = root.Q<Label>("PlayerHeaderLabel");
        playerContainer = root.Q<VisualElement>("PlayerContainer");
        exitButton = root.Q<TemplateContainer>("ExitButton");
    }
}
