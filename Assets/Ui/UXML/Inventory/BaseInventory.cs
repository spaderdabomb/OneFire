using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OneFireUi
{
    public class BaseInventory : IUiToolkitElement
    {
        public VisualElement root;
        public int inventoryRows;
        public int inventoryCols;
        public bool readOnly = false;
        public List<BaseInventorySlot> inventorySlots;
        public BaseInventorySlot currentDraggedInventorySlot { get; set; } = null;
        public BaseInventorySlot currentHoverSlot { get; set; } = null;

        public string inventoryID;
        public BaseInventory(VisualElement root, int inventoryRows, int inventoryCols)
        {
            this.root = root;
            this.inventoryRows = inventoryRows;
            this.inventoryCols = inventoryCols;
            RegisterCallbacks();
        }

        public void RegisterCallbacks()
        {
            root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void UnregisterCallbacks()
        {
            
        }

        public void OnGeometryChanged(GeometryChangedEvent evt)
        {
            root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        public void SetCurrentSlot(BaseInventorySlot newSlot)
        {
            currentHoverSlot = newSlot;
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
                BaseInventorySlot inventorySlot = inventorySlots[slotIndex];
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

        public int AddItem(ItemData itemData, BaseInventorySlot addSlot)
        {
            int numItemsRemaining = addSlot.AddItemToSlot(itemData);

            return numItemsRemaining;
        }

        public void RemoveItem(BaseInventorySlot removeSlot)
        {
            removeSlot.RemoveItemFromSlot();
        }

        public virtual bool CanMoveItem(BaseInventorySlot dragEndSlot, BaseInventorySlot dragBeginSlot)
        {
            bool isSameSlotIndex = dragEndSlot.slotIndex == dragBeginSlot.slotIndex;
            bool isSameParentContainer = dragEndSlot.parentContainer == dragBeginSlot.parentContainer;

            return (isSameSlotIndex && isSameParentContainer) ? false : true;
        }

        public void MoveItem(BaseInventorySlot dragEndSlot, BaseInventorySlot dragBeginSlot)
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

        public void SwapItems(BaseInventorySlot dragEndSlot, BaseInventorySlot dragBeginSlot)
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
    }
}
