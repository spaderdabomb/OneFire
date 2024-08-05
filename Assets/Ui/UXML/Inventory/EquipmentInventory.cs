using UnityEngine.UIElements;
using UnityEngine;
using static ItemData;
using System.Collections.Generic;

namespace OneFireUi
{
    public partial class EquipmentInventory : BaseInventory
    {
        public List<EquipmentSlot> equipmentSlots = new();
        public List<EquipmentSlotData> equipmentSlotData = new();
        public List<ItemType> equipmentSlotKeys = new();
        private List<EquipmentSlot> equipmentSlotsHighlighted = new List<EquipmentSlot>();
        public EquipmentInventory(VisualElement root, int numInventorySlots, string inventoryId, bool selectable) : base(root, numInventorySlots, inventoryId, selectable)
        {
            AssignQueryResults(root);
            InitEquipmentInventory();
            InitEquipmentSlots();
        }

        private void InitEquipmentInventory()
        {
            // Clear current layout
            root.Clear();

            // Make lists
            foreach (var kvp in InventoryManager.Instance.equipmentSlotDataDict)
            {
                equipmentSlotData.Add(kvp.Value);
                equipmentSlotKeys.Add(kvp.Key);
            }
        }

        private void InitEquipmentSlots()
        {
            inventorySlots = new List<InventorySlot>();
            for (int i = 0; i < numInventorySlots; i++)
            {
                VisualElement slotClone = InventoryManager.Instance.inventorySlotAsset.CloneTree();
                EquipmentSlotData tempEquipementSlotData = i >= equipmentSlotData.Count ? null : equipmentSlotData[i];
                ItemType tempItemType = i >= equipmentSlotKeys.Count ? ItemType.None : equipmentSlotKeys[i];
                EquipmentSlot newEquipmentSlot = new EquipmentSlot(slotClone, i, this, tempEquipementSlotData, tempItemType);
                newEquipmentSlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, newEquipmentSlot));
                equipmentSlots.Add(newEquipmentSlot);
                inventorySlots.Add(newEquipmentSlot);
                root.Add(slotClone);
            }

/*            // Init persistent data
            BaseItemData[] baseItemData = DataManager.Instance.LoadGearContainerData(gearContainerType);
            for (int i = 0; i < baseItemData.Length; i++)
            {
                BaseItemData baseItem = baseItemData[i];
                if (baseItem != null)
                {
                    ItemData itemDataAsset = ItemExtensions.GetItemData(baseItem.itemID);
                    ItemData newItem = itemDataAsset.GetItemDataInstantiated();
                    newItem.SetItemDataToBaseItemData(baseItem);
                    AddItem(newItem, inventorySlots[i]);
                }
            }*/
        }

        public override bool CanMoveItem(InventorySlot dragEndSlot, InventorySlot dragBeginSlot)
        {
            if (!base.CanMoveItem(dragEndSlot, dragBeginSlot))
            {
                return false;
            }

            EquipmentSlot dragEndGearSlot = (EquipmentSlot)dragEndSlot;

            bool validItemType = dragEndGearSlot.itemType == dragBeginSlot.currentItemData.itemType;

            return validItemType;
        }

        public void SetAllValidSlotHighlights(ItemData itemData)
        {
            foreach (EquipmentSlot equipmentSlot in equipmentSlots)
            {
                if (equipmentSlot.itemType == itemData.itemType)
                {
                    equipmentSlot.SetHighlight();
                    equipmentSlotsHighlighted.Add(equipmentSlot);
                }
            }
        }

        public void ResetAllValidSlotHighlights()
        {
            equipmentSlotsHighlighted.ForEach(slot => slot.ResetHighlight());
            equipmentSlotsHighlighted.Clear();
        }
    }
}
