using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace OneFireUi
{
    public class BaseInventory : IPersistentData
    {
        public VisualElement root;

        public int numInventorySlots;
        public bool readOnly = false;

        public List<InventorySlot> inventorySlots;
        public BaseItemData[] inventoryItemData;

        public InventorySlot currentDraggedInventorySlot { get; set; } = null;
        public InventorySlot currentHoverSlot { get; set; } = null;

        public string InventoryId { get; private set; }
        public BaseInventory(VisualElement root, int numInventorySlots, string inventoryId)
        {
            this.root = root;
            this.numInventorySlots = numInventorySlots;
            this.InventoryId = inventoryId;

            InitInventorySlots();
            LoadData();
            AddToSaveable();
        }

        private void InitInventorySlots()
        {
            // Init slots
            inventorySlots = new List<InventorySlot>();
            for (int i = 0; i < numInventorySlots; i++)
            {
                VisualElement inventoryAsset = InventoryManager.Instance.inventorySlotAsset.CloneTree();
                InventorySlot inventorySlot = new InventorySlot(inventoryAsset, i, this);
                inventorySlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, inventorySlot));
                inventorySlots.Add(inventorySlot);
            }
        }

        public void SetCurrentHoverSlot(InventorySlot newSlot)
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

        public void TrySplitItem(bool splitHalf)
        {
            if (!ItemExistsInHoverSlot())
                return;

            ItemData newItemData = currentHoverSlot.currentItemData.CloneItemData();
            int firstSlot = GetFirstFreeSlot(newItemData, mergeSameItems: false);
            if (firstSlot == -1 || currentHoverSlot.currentItemData.stackCount == 1)
                return;

            int newStackCount;
            if (splitHalf)
                newStackCount = Mathf.CeilToInt(currentHoverSlot.currentItemData.stackCount / 2f);
            else
                newStackCount = 1;

            int oldStackCount = currentHoverSlot.currentItemData.stackCount - newStackCount;
            currentHoverSlot.SetStackCount(oldStackCount);
            newItemData.stackCount = newStackCount;

            AddItem(newItemData, inventorySlots[firstSlot]);
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

        private BaseItemData[] GetInventorySlotData()
        {
            BaseItemData[] baseItemDataArray = new BaseItemData[inventorySlots.Count];
            for (int i = 0; i < inventorySlots.Count; i++)
            {
                if (inventorySlots[i].currentItemData != null)
                {
                    BaseItemData baseItemData = new BaseItemData(inventorySlots[i].currentItemData);
                    baseItemDataArray[i] = baseItemData;
                }
            }

            return baseItemDataArray;
        }

        public void AddToSaveable()
        {
            PersistentDataManager.Instance.AddToPersistentDataList(this);
        }

        public void LoadData()
        {
            inventoryItemData = new BaseItemData[numInventorySlots];
            inventoryItemData = ES3.Load(InventoryId, defaultValue: inventoryItemData);

            for (int i = 0; i < inventoryItemData.Length; i++)
            {
                BaseItemData baseItem = inventoryItemData[i];
                if (baseItem != null)
                {
                    ItemData itemDataAsset = ItemExtensions.GetItemData(baseItem.itemID);
                    ItemData newItem = itemDataAsset.GetItemDataInstantiated();
                    newItem.SetItemDataToBaseItemData(baseItem);
                    AddItem(newItem, inventorySlots[i]);
                }
            }
        }

        public void SaveData()
        {
            inventoryItemData = GetInventorySlotData();
            ES3.Save(InventoryId, inventoryItemData);
        }
    }
}
