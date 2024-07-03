using UnityEngine.UIElements;
using UnityEngine;
using ItemType = ItemData.ItemType;

namespace OneFireUi
{
    public class EquipmentSlot : InventorySlot
    {
        public EquipmentSlotData equipmentSlotData;
        public ItemType itemType;

        public EquipmentSlot(
            VisualElement root,
            int slotIndex,
            EquipmentInventory equipmentInventory,
            EquipmentSlotData equipmentSlotData,
            ItemType itemType
        ) : base(root, slotIndex, equipmentInventory)
        {
            this.equipmentSlotData = equipmentSlotData;
            this.itemType = itemType;
            InitEquipmentSlot();
            // base.AssignQueryResults(root);
        }

        private void InitEquipmentSlot()
        {
            SlotBackingIcon.style.display = DisplayStyle.Flex;
            SlotBackingIcon.style.backgroundImage = equipmentSlotData.backingIcon;
        }
    }
}

