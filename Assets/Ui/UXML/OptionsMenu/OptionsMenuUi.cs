using UnityEngine.UIElements;
using UnityEngine;

namespace OneFireUi
{
    public partial class OptionsMenuUi
    {
        public VisualElement root;
        public VisualElement rootElement;
        public InventoryMenuUi inventoryMenu;
        public OptionsMenuUi(VisualElement root)
        {
            AssignQueryResults(root);

            this.root = root;
            rootElement = optionsUiRoot;

            InitInventoryMenu();
        }

        private void InitInventoryMenu()
        {
            inventoryMenu = new InventoryMenuUi(inventoryMenuUi);
        }

        public VisualElement GetRootElement()
        {
            return optionsUiRoot;
        }
    }
}
