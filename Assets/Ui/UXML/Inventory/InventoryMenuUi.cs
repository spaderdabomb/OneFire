using UnityEngine.UIElements;
using UnityEngine;
using OneFireUI;

namespace OneFireUi
{
    public partial class InventoryMenuUi: ITabMenu
    {
        public VisualElement root;
        public int TabIndex { get; }
        public InventoryMenuUi(VisualElement root, int tabIndex)
        {
            this.root = root;
            TabIndex = tabIndex;

            AssignQueryResults(root);
        }

        public VisualElement GetPlayerInventoryRoot()
        {
            return playerInventory;
        }

        public VisualElement GetEquipmentInventoryRoot()
        {
            return playerEquipmentInventory;
        }

        public void ShowMenu()
        {
            root.style.display = DisplayStyle.Flex;
            InventoryManager.Instance.PlayerInventory.ShowPlayerInventory();
        }
    
        public void HideMenu()
        {
            root.style.display = DisplayStyle.None;
        }

        public void OnTabChanged(int newIndex)
        {
            if (TabIndex == newIndex)
                ShowMenu();
            else
                HideMenu();
        }
    }
}
