// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace OneFireUI
{
    partial class PlayerInventory
    {
        private VisualElement inventoryUiRoot;
        private VisualElement inventoryContainer;
        private VisualElement inventoryActionsContainer;
        private VisualElement leftButtonGroup;
        private Button sortButton;
        private VisualElement sortButtonIcon;
        private Label sortButtonLabel;
        private VisualElement rightButtonGroup;
        private Button dropButton;
        private VisualElement dropButtonIcon;
        private Label dropButtonLabel;
        private Button discardButton;
        private VisualElement discardButtonIcon;
        private Label discardButtonLabel;
    
        protected void AssignQueryResults(VisualElement root)
        {
            inventoryUiRoot = root.Q<VisualElement>("InventoryUiRoot");
            inventoryContainer = root.Q<VisualElement>("InventoryContainer");
            inventoryActionsContainer = root.Q<VisualElement>("InventoryActionsContainer");
            leftButtonGroup = root.Q<VisualElement>("LeftButtonGroup");
            sortButton = root.Q<Button>("SortButton");
            sortButtonIcon = root.Q<VisualElement>("SortButtonIcon");
            sortButtonLabel = root.Q<Label>("SortButtonLabel");
            rightButtonGroup = root.Q<VisualElement>("RightButtonGroup");
            dropButton = root.Q<Button>("DropButton");
            dropButtonIcon = root.Q<VisualElement>("DropButtonIcon");
            dropButtonLabel = root.Q<Label>("DropButtonLabel");
            discardButton = root.Q<Button>("DiscardButton");
            discardButtonIcon = root.Q<VisualElement>("DiscardButtonIcon");
            discardButtonLabel = root.Q<Label>("DiscardButtonLabel");
        }
    }
}
