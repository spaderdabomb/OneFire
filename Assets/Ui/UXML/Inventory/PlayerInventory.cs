using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using JSAM;
using System.Collections.Generic;

namespace OneFireUI
{
    public partial class PlayerInventory : BaseInventory
    {
        public BaseItemData[] inventoryItemData;
        public PlayerInventory(VisualElement root, int numInventorySlots) : base(root, numInventorySlots)
        {
            AssignQueryResults(root);
            InitMenuButtons();
            InitInventorySlots();
        }

        private void InitMenuButtons()
        {

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
                inventoryContainer.Add(inventoryAsset);
            }

            // Load item data
            //DataManager.Instance.inventoryItemData = DataManager.Instance.Load(nameof(DataManager.Instance.inventoryItemData), DataManager.Instance.inventoryItemData);

            //inventoryItemData = ES3.Load(nameof(inventoryItemData), defaultValue: inventoryItemData);
            //for (int i = 0; i < DataManager.Instance.inventoryItemData.Length; i++)
            //{
            //    BaseItemData baseItem = DataManager.Instance.inventoryItemData[i];
            //    if (baseItem != null)
            //    {
            //        ItemData itemDataAsset = ItemExtensions.GetItemData(baseItem.itemID);
            //        ItemData newItem = itemDataAsset.GetItemDataInstantiated();
            //        newItem.SetItemDataToBaseItemData(baseItem);
            //        AddItem(newItem, inventorySlots[i]);
            //    }
            //}
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

        public void GetItemAtIndex()
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

        public void AddSlotsToContainer(VisualElement containerRoot)
        {
            foreach (InventorySlot slot in inventorySlots)
            {
                slot.root.parent.Remove(slot.root);
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

        public void SaveData()
        {
            ES3.Save(nameof(inventoryItemData), inventoryItemData);
        }
    }
}
