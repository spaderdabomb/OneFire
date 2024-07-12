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
    private Button allRightButton;
    private VisualElement allRightIcon;
    private Label allRightLabel;
    private Button allLeftButton;
    private VisualElement allLeftIcon;
    private Label allLeftLabel;
    private VisualElement playerContainerRoot;
    private Label playerHeaderLabel;
    private VisualElement playerContainer;
    
    protected void AssignQueryResults(VisualElement root)
    {
        chestMenuRoot = root.Q<VisualElement>("ChestMenuRoot");
        chestContainerRoot = root.Q<VisualElement>("ChestContainerRoot");
        chestHeaderLabel = root.Q<Label>("ChestHeaderLabel");
        chestContainer = root.Q<VisualElement>("ChestContainer");
        buttonContainer = root.Q<VisualElement>("ButtonContainer");
        allRightButton = root.Q<Button>("AllRightButton");
        allRightIcon = root.Q<VisualElement>("AllRightIcon");
        allRightLabel = root.Q<Label>("AllRightLabel");
        allLeftButton = root.Q<Button>("AllLeftButton");
        allLeftIcon = root.Q<VisualElement>("AllLeftIcon");
        allLeftLabel = root.Q<Label>("AllLeftLabel");
        playerContainerRoot = root.Q<VisualElement>("PlayerContainerRoot");
        playerHeaderLabel = root.Q<Label>("PlayerHeaderLabel");
        playerContainer = root.Q<VisualElement>("PlayerContainer");
    }
}
