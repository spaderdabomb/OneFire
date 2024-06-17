using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;

namespace OneFireUI
{
    public partial class PlayerInventoryUi : BaseInventoryUi
    {   
        public PlayerInventoryUi(VisualElement root, int inventoryRows, int inventoryCols) : base(root, inventoryRows, inventoryCols)
        {
            AssignQueryResults(root);
        }
    }
}
