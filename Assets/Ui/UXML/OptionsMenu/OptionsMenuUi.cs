using UnityEngine.UIElements;
using UnityEngine;

namespace OneFireUi
{
    public partial class OptionsMenuUi
    {
        public VisualElement root;
        public InventoryMenuUi inventoryMenu;
        public OptionsMenuUi(VisualElement root)
        {
            this.root = root;

            AssignQueryResults(root);
            InitInventoryMenu();
        }

        private void InitInventoryMenu()
        {
            inventoryMenu = new InventoryMenuUi(inventoryMenuUi);
        }
    }
}
