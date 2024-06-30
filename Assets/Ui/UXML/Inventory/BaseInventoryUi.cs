using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace OneFireUi
{
    public partial class BaseInventoryUi : IUiToolkitElement
    {
        public VisualElement root;
        public int inventoryRows;
        public int inventoryCols;
        public bool readOnly = false;
        public List<BaseInventorySlotUi> inventorySlots;
        public BaseInventorySlotUi currentDraggedInventorySlot { get; set; } = null;
        public BaseInventorySlotUi currentHoverSlot { get; set; } = null;

        public string inventoryID;
        public BaseInventoryUi(VisualElement root, int inventoryRows, int inventoryCols)
        {
            this.root = root;
            this.inventoryRows = inventoryRows;
            this.inventoryCols = inventoryCols;
            AssignQueryResults(root);
            RegisterCallbacks();
            InitInventorySlots();
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

        private void InitInventorySlots()
        {
            // Init slots
            inventorySlots = new List<BaseInventorySlotUi>();
            Debug.Log(inventoryRows);
            Debug.Log(inventoryCols);

            for (int i = 0; i < inventoryRows; i++)
            {
                for (int j = 0; j < inventoryCols; j++)
                {
                    VisualElement inventoryAsset = InventoryManager.Instance.inventorySlotAsset.CloneTree();
                    BaseInventorySlotUi inventorySlot = new BaseInventorySlotUi(inventoryAsset, j + i * inventoryCols, this);
                    inventorySlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, inventorySlot));
                    inventorySlots.Add(inventorySlot);
                    Debug.Log($"Adding to bas{inventorySlot}");
                    baseInventoryRoot.Add(inventoryAsset);
                }
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

        public void SetCurrentSlot(BaseInventorySlotUi newSlot)
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
                BaseInventorySlotUi inventorySlot = inventorySlots[slotIndex];
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

        public int AddItem(ItemData itemData, BaseInventorySlotUi addSlot)
        {
            int numItemsRemaining = addSlot.AddItemToSlot(itemData);

            return numItemsRemaining;
        }

        public void RemoveItem(BaseInventorySlotUi removeSlot)
        {
            removeSlot.RemoveItemFromSlot();
        }

        public virtual bool CanMoveItem(BaseInventorySlotUi dragEndSlot, BaseInventorySlotUi dragBeginSlot)
        {
            bool isSameSlotIndex = dragEndSlot.slotIndex == dragBeginSlot.slotIndex;
            bool isSameParentContainer = dragEndSlot.parentContainer == dragBeginSlot.parentContainer;

            return (isSameSlotIndex && isSameParentContainer) ? false : true;
        }

        public void MoveItem(BaseInventorySlotUi dragEndSlot, BaseInventorySlotUi dragBeginSlot)
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

        public void SwapItems(BaseInventorySlotUi dragEndSlot, BaseInventorySlotUi dragBeginSlot)
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
