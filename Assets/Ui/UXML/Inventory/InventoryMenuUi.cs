using UnityEngine.UIElements;
using UnityEngine;
using OneFireUI;

namespace OneFireUi
{
    public partial class InventoryMenuUi
    {
        public VisualElement root;
        public PlayerInventoryUi playerInventory;
        public InventoryMenuUi(VisualElement root)
        {
            this.root = root;

            AssignQueryResults(root);
            InitInventory();
        }

        public void InitInventory()
        {
            playerInventory = new PlayerInventoryUi(playerInventoryUi, 5, 8);
        }
    }
}
