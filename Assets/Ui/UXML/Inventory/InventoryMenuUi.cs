using UnityEngine.UIElements;
using UnityEngine;
using OneFireUI;

namespace OneFireUi
{
    public partial class InventoryMenuUi
    {
        public VisualElement root;
        public InventoryMenuUi(VisualElement root)
        {
            this.root = root;

            AssignQueryResults(root);
        }

        public VisualElement GetPlayerInventoryRoot()
        {
            return playerInventory;
        }
    }
}
