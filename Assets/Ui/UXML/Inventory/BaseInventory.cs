using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OneFireUi
{
    public class BaseInventory
    {
        public VisualElement root;
        public int inventoryRows;
        public int inventoryCols;
        public bool readOnly = false;
        public List<InventorySlot> inventorySlots;
        public InventorySlot currentDraggedInventorySlot { get; set; } = null;
        public InventorySlot currentHoverSlot { get; set; } = null;

        public string inventoryID;
        public BaseInventory(VisualElement root, int inventoryRows, int inventoryCols)
        {
            this.root = root;
            this.inventoryRows = inventoryRows;
            this.inventoryCols = inventoryCols;
        }

        public void SetCurrentSlot(InventorySlot newSlot)
        {
            currentHoverSlot = newSlot;
        }

        public bool ItemExistsInHoverSlot()
        {
            return currentHoverSlot != null && currentHoverSlot.currentItemData != null;
        }

        // Returns number of remaining items in stack
        public int TryAddItem(ItemData itemData)
        {
            int slotIndex = GetFirstFreeSlot(itemData);
            if (slotIndex == -1)
                return itemData.stackCount;

            bool itemsRemainingBool = true;
            int itemsRemaining = 0;
            while (itemsRemainingBool)
            {
                InventorySlot inventorySlot = inventorySlots[slotIndex];
                itemsRemaining = AddItem(itemData, inventorySlot);
                itemsRemainingBool = (itemsRemaining > 0) ? true : false;

                slotIndex = GetFirstFreeSlot(itemData);
                if (slotIndex == -1)
                {
                    break;
                }
            }

            return itemsRemaining;
        }

        public int AddItem(ItemData itemData, InventorySlot addSlot)
        {
            int numItemsRemaining = addSlot.AddItemToSlot(itemData);

            return numItemsRemaining;
        }

        public void RemoveItem(InventorySlot removeSlot)
        {
            removeSlot.RemoveItemFromSlot();
        }

        public virtual bool CanMoveItem(InventorySlot dragEndSlot, InventorySlot dragBeginSlot)
        {
            bool isSameSlotIndex = dragEndSlot.slotIndex == dragBeginSlot.slotIndex;
            bool isSameParentContainer = dragEndSlot.parentContainer == dragBeginSlot.parentContainer;

            return (isSameSlotIndex && isSameParentContainer) ? false : true;
        }

        public void MoveItem(InventorySlot dragEndSlot, InventorySlot dragBeginSlot)
        {
            /*        if (!CanMoveItem(dragEndSlot, dragBeginSlot))
                        return;*/

            ItemData dragBeginItemData = dragBeginSlot.currentItemData.CloneItemData();

            // If target slot has no items
            if (dragEndSlot.currentItemData == null)
            {
                AddItem(dragBeginItemData, dragEndSlot);
                RemoveItem(dragBeginSlot);
            }
            // If items in both slots are the same itemID
            else if (dragEndSlot.currentItemData.itemID == dragBeginSlot.currentItemData.itemID)
            {
                int totalItemCount = dragEndSlot.currentItemData.stackCount + dragBeginSlot.currentItemData.stackCount;
                bool exeedsStackCount = totalItemCount > dragEndSlot.currentItemData.maxStackCount;
                if (exeedsStackCount)
                {
                    SwapItems(dragEndSlot, dragBeginSlot);
                }
                else
                {
                    AddItem(dragBeginItemData, dragEndSlot);
                    RemoveItem(dragBeginSlot);
                }
            }
            // If both slots have items but are not same itemID
            else
            {
                SwapItems(dragEndSlot, dragBeginSlot);
            }
        }

        public void SwapItems(InventorySlot dragEndSlot, InventorySlot dragBeginSlot)
        {
            if (dragBeginSlot.currentItemData != null && dragEndSlot.currentItemData != null)
            {
                ItemData dragBeginItemData = dragBeginSlot.currentItemData;
                ItemData dragEndItemData = dragEndSlot.currentItemData;
                RemoveItem(dragBeginSlot);
                RemoveItem(dragEndSlot);
                AddItem(dragEndItemData, dragBeginSlot);
                AddItem(dragBeginItemData, dragEndSlot);
            }
        }

        public int GetFirstFreeSlot(ItemData itemData, bool mergeSameItems = true)
        {
            int slotIndex = -1;
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                // Add item to free slot
                if (!inventorySlots[i].slotFilled)
                {
                    slotIndex = i;
                    break;
                }
                // Unfilled slot with identical item
                else if (inventorySlots[i].currentItemData.itemID == itemData.itemID &&
                         inventorySlots[i].currentItemData.stackCount < inventorySlots[i].currentItemData.maxStackCount &&
                         mergeSameItems)
                {
                    slotIndex = i;
                    break;
                }
            }

            return slotIndex;
        }

        public virtual InventorySlot GetCurrentSlotMouseOver(PointerUpEvent evt)
        {
            InventorySlot currentSlot = null;
            foreach (InventorySlot slot in inventorySlots)
            {
                if (slot.root.worldBound.Contains(evt.position))
                {
                    currentSlot = slot;
                    break;
                }
            }

            return currentSlot;
        }
    }
}
