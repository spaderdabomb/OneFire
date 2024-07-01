using UnityEngine.UIElements;
using UnityEngine;
using ItemType = ItemData.ItemType;

namespace OneFireUi
{
    public partial class EquipmentInventorySlot : BaseInventorySlot
    {
        public EquipmentSlotData equipmentSlotData;
        public ItemType itemType;
        public EquipmentInventorySlot
        (
            VisualElement root, 
            int slotIndex, 
            EquipmentInventory equipmentInventory, 
            EquipmentSlotData equipmentSlotData, 
            ItemType itemType
        ) 
            : base(root, slotIndex, equipmentInventory)
        {
            this.equipmentSlotData = equipmentSlotData;
            this.itemType = itemType;
            AssignQueryResults(root);
            InitEquipmentSlot();
        }

        private void InitEquipmentSlot()
        {
            equipmentInventorySlotUi.style.backgroundImage = equipmentSlotData.backingIcon;
        }
    }
}
