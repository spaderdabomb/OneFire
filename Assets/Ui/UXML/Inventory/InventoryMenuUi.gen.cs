// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace OneFireUi
{
    partial class InventoryMenuUi
    {
        private VisualElement inventoryMenuUiRoot;
        private Label headerLabel;
        private VisualElement horizontalLayout;
        private VisualElement leftGroup;
        private VisualElement playerSummary;
        private VisualElement playerHLayout;
        private VisualElement playerIconLayout;
        private Label playerNameLabel;
        private VisualElement playerIcon;
        private VisualElement playerEquipment;
        private Label equipmentLabel;
        private VisualElement playerEquipmentInventory;
        private VisualElement statsLayout;
        private Label statsHeaderLabel;
        private VisualElement statsVerticalLayout;
        private TemplateContainer equipmentUi;
        private VisualElement rightGroup;
        private VisualElement inventoryContainer;
        private TemplateContainer playerInventory;
    
        protected void AssignQueryResults(VisualElement root)
        {
            inventoryMenuUiRoot = root.Q<VisualElement>("InventoryMenuUiRoot");
            headerLabel = root.Q<Label>("HeaderLabel");
            horizontalLayout = root.Q<VisualElement>("HorizontalLayout");
            leftGroup = root.Q<VisualElement>("LeftGroup");
            playerSummary = root.Q<VisualElement>("PlayerSummary");
            playerHLayout = root.Q<VisualElement>("PlayerHLayout");
            playerIconLayout = root.Q<VisualElement>("PlayerIconLayout");
            playerNameLabel = root.Q<Label>("PlayerNameLabel");
            playerIcon = root.Q<VisualElement>("PlayerIcon");
            playerEquipment = root.Q<VisualElement>("PlayerEquipment");
            equipmentLabel = root.Q<Label>("EquipmentLabel");
            playerEquipmentInventory = root.Q<VisualElement>("PlayerEquipmentInventory");
            statsLayout = root.Q<VisualElement>("StatsLayout");
            statsHeaderLabel = root.Q<Label>("StatsHeaderLabel");
            statsVerticalLayout = root.Q<VisualElement>("StatsVerticalLayout");
            equipmentUi = root.Q<TemplateContainer>("EquipmentUi");
            rightGroup = root.Q<VisualElement>("RightGroup");
            inventoryContainer = root.Q<VisualElement>("InventoryContainer");
            playerInventory = root.Q<TemplateContainer>("PlayerInventory");
        }
    }
}
