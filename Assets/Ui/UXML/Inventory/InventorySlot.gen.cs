// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace OneFireUi
{
    partial class InventorySlot
    {
        private VisualElement inventorySlotRoot;
        private VisualElement slotIcon;
        private Label slotLabel;
        private VisualElement slotBackingIcon;
    
        protected void AssignQueryResults(VisualElement root)
        {
            inventorySlotRoot = root.Q<VisualElement>("InventorySlotRoot");
            slotIcon = root.Q<VisualElement>("SlotIcon");
            slotLabel = root.Q<Label>("SlotLabel");
            slotBackingIcon = root.Q<VisualElement>("SlotBackingIcon");
        }
    }
}
