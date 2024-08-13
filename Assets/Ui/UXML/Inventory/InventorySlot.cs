using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

namespace OneFireUi
{
    public partial class InventorySlot : IUiToolkitElement
    {
        public VisualElement root;
        public ItemData currentItemData;
        public BaseInventory parentContainer;
        public int slotIndex;
        public bool slotFilled = false;
        public bool selectable = false;
        public Vector2 mousePosition;

        private Color labelColorDefault;
        private Color iconTintColorDefault;
        private Color slotContainerColorDefault;

        private static string unselectedStyle = "inventory-slot";
        private static string selectedStyle = "inventory-slot-selected";

        protected VisualElement SlotBackingIcon
        {
            get { return slotBackingIcon; }
        }

        public InventorySlot(VisualElement root, int slotIndex, BaseInventory parentContainer, bool selectable = false)
        {
            this.root = root;
            this.root.userData = this;
            this.slotIndex = slotIndex;
            this.parentContainer = parentContainer;
            this.selectable = selectable;
            AssignQueryResults(root);
            RegisterCallbacks();
            this.selectable = selectable;
        }

        public void RegisterCallbacks()
        {
            root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            root.RegisterCallback<PointerEnterEvent>(PointerEnterSlot);
            root.RegisterCallback<PointerLeaveEvent>(PointerLeaveSlot);
            root.RegisterCallback<PointerMoveEvent>(PointerMoveInSlot);
        }

        public void UnregisterCallbacks()
        {
            root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            root.UnregisterCallback<PointerEnterEvent>(PointerEnterSlot);
            root.UnregisterCallback<PointerLeaveEvent>(PointerLeaveSlot);
            root.UnregisterCallback<PointerMoveEvent>(PointerMoveInSlot);
        }

        public void OnGeometryChanged(GeometryChangedEvent evt)
        {
            labelColorDefault = slotLabel.resolvedStyle.color;
            iconTintColorDefault = slotIcon.resolvedStyle.unityBackgroundImageTintColor;
            slotContainerColorDefault = inventorySlotRoot.resolvedStyle.unityBackgroundImageTintColor;
            root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        // Returns number of items in stack remaining
        public int AddItemToSlot(ItemData itemData)
        {
            int itemsRemaining = 0;

            // Add to slot with existing item
            if (currentItemData != null)
            {
                int stackCountRemaining = currentItemData.maxStackCount - currentItemData.stackCount;
                if (itemData.stackCount > stackCountRemaining)
                {
                    currentItemData.stackCount = currentItemData.maxStackCount;
                    itemsRemaining = itemData.stackCount - stackCountRemaining;
                    itemData.stackCount = itemsRemaining;
                }
                else
                {
                    currentItemData.stackCount += itemData.stackCount;
                    itemData.stackCount = 0;
                }
            }
            // Add to empty slot
            else
            {
                currentItemData = itemData;
                itemsRemaining = Mathf.Max(0, currentItemData.stackCount - currentItemData.maxStackCount);
                currentItemData.stackCount = Mathf.Min(currentItemData.stackCount, currentItemData.maxStackCount);
            }

            slotFilled = true;
            UpdateSelectedSlot();
            SetSlotUI();

            return itemsRemaining;
        }

        public int SubtractItemFromSlot(ItemData itemData, int itemQuantity)
        {
            int itemsRemaining = -1;

            if (itemData.itemID != currentItemData.itemID)
            {
                Debug.Log($"Trying to subtract {itemData} from slot with {currentItemData}, returning");
                return itemsRemaining;
            }

            if (itemQuantity < currentItemData.stackCount)
            {
                SetStackCount(currentItemData.stackCount - itemQuantity);
                itemsRemaining = 0;
            }
            else
            {
                itemsRemaining = itemQuantity - currentItemData.stackCount;
                RemoveItemFromSlot();
            }

            return itemsRemaining;
        }

        public void SetStackCount(int newCount)
        {
            currentItemData.stackCount = newCount;

            if (newCount > 1)
                slotLabel.text = newCount.ToString();
            else
                slotLabel.text = string.Empty;
        }

        public void RemoveItemFromSlot()
        {
            currentItemData = null;
            slotIcon.style.backgroundImage = null;
            slotLabel.text = string.Empty;
            slotFilled = false;
        }

        public bool ContainsItem()
        {
            return currentItemData != null;
        }

        private void SetSlotUI()
        {
            slotIcon.style.backgroundImage = currentItemData.itemSprite.texture;
            SetStackCount(currentItemData.stackCount);
        }

        public void SetHoverStyle()
        {
            slotIcon.style.unityBackgroundImageTintColor = GameDataManager.Instance.standardUiTintColor;
            slotLabel.style.color = GameDataManager.Instance.standardUiTintColor;
            inventorySlotRoot.style.unityBackgroundImageTintColor = GameDataManager.Instance.standardUiTintColor;
        }

        public void ResetStyle()
        {
            slotIcon.style.unityBackgroundImageTintColor = iconTintColorDefault;
            slotLabel.style.color = labelColorDefault;
            inventorySlotRoot.style.unityBackgroundImageTintColor = slotContainerColorDefault;
        }

        public void SetHighlight()
        {
            inventorySlotRoot.AddToClassList("inventory-slot-highlighted");
        }

        public void ResetHighlight()
        {
            inventorySlotRoot.RemoveFromClassList("inventory-slot-highlighted");
        }

        public void PointerEnterSlot(PointerEnterEvent evt)
        {
            if (InventoryManager.Instance.IsDragging)
                return;

            InventoryManager.Instance.UpdateCurrentHoverSlot(this, true);
            parentContainer.currentHoverSlot = this;
            AudioManager.PlaySound(MainLibrarySounds.WoodenTick);
        }

        public void PointerLeaveSlot(PointerLeaveEvent evt)
        {
            InventoryManager.Instance.UpdateCurrentHoverSlot(this, false);
            parentContainer.currentHoverSlot = null;
        }

        public void PointerMoveInSlot(PointerMoveEvent evt)
        {
            mousePosition = evt.position;
        }

        public void SetSelected()
        {
            inventorySlotRoot.ClearClassList();
            inventorySlotRoot.AddToClassList(selectedStyle);
        }

        public void SetUnselected()
        {
            inventorySlotRoot.ClearClassList();
            inventorySlotRoot.AddToClassList(unselectedStyle);
        }

        private void UpdateSelectedSlot()
        {
            if (!selectable || slotIndex != parentContainer.selectedIndex)
                return;

            InventorySlot currentSlot = parentContainer.GetSelectedSlot();
            if (currentSlot != null)
            {
                InventoryManager.Instance.OnHotbarItemSelectedChanged.Invoke(currentSlot.currentItemData);
            }
        }
    }
}
