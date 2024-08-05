using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using JSAM;
using System.Collections.Generic;

namespace OneFireUI
{
    public partial class PlayerInventory : BaseInventory
    {
        public PlayerInventory(VisualElement root, int numInventorySlots, string inventoryId, bool selectable) : base(root, numInventorySlots, inventoryId, selectable)
        {
            AssignQueryResults(root);
            InitMenuButtons();
            AddSlotsToContainer(inventoryContainer);
        }

        private void InitMenuButtons()
        {

        }

        public override bool CanMoveItem(InventorySlot dragEndSlot, InventorySlot dragBeginSlot)
        {
            // If base method can't move item, return false
            if (!base.CanMoveItem(dragEndSlot, dragBeginSlot))
            {
                return false;
            }

            // If beginning slot is not a gear slot or we're dragging to an empty slot, return true
            if (dragBeginSlot is not EquipmentSlot || dragEndSlot.currentItemData == null)
                return true;

            // Gear slot checks
            EquipmentSlot dragBeginGearSlot = (EquipmentSlot)dragBeginSlot;

            // Swapping from gear container to inventory container
            bool canMoveItem = true;
            bool isSwappingNonIdenticalItem = (dragBeginGearSlot.currentItemData != null && dragBeginGearSlot.currentItemData.itemID != dragEndSlot.currentItemData.itemID);
            if (isSwappingNonIdenticalItem)
            {
                bool validItemType = dragBeginGearSlot.itemType == dragEndSlot.currentItemData.itemType;
                canMoveItem = validItemType;
            }

            return canMoveItem;
        }

        public void ShowPlayerInventory()
        {
            inventoryContainer.Clear();
            AddSlotsToContainer(inventoryContainer);
        }

        public void AddSlotsToContainer(VisualElement containerRoot)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                slot.root.parent?.Remove(slot.root);
                containerRoot.Add(slot.root);
            }
        }

        public bool IsPointerOverDropButton(PointerUpEvent evt)
        {
            return dropButton.worldBound.Contains(evt.position);
        }

        public bool IsPointerOverDiscardButton(PointerUpEvent evt)
        {
            return discardButton.worldBound.Contains(evt.position);
        }
    }
}
