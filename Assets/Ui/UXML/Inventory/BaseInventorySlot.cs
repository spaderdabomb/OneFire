using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

namespace OneFireUi
{
    public partial class BaseInventorySlot : IUiToolkitElement
    {
        public VisualElement root;
        public ItemData currentItemData;
        public BaseInventory parentContainer;
        public int slotIndex;
        public bool slotFilled = false;

        private Color labelColorDefault;
        private Color iconTintColorDefault;
        private Color slotContainerColorDefault;

        public BaseInventorySlot(VisualElement root, int slotIndex, BaseInventory parentContainer)
        {
            this.root = root;
            this.slotIndex = slotIndex;
            this.parentContainer = parentContainer;
            AssignQueryResults(root);
            RegisterCallbacks();
        }

        public void RegisterCallbacks()
        {
            root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            root.RegisterCallback<PointerEnterEvent>(PointerEnterSlot);
            root.RegisterCallback<PointerLeaveEvent>(PointerLeaveSlot);
        }

        public void UnregisterCallbacks()
        {
            throw new System.NotImplementedException();
        }

        public void OnGeometryChanged(GeometryChangedEvent evt)
        {
            labelColorDefault = slotLabel.resolvedStyle.color;
            iconTintColorDefault = slotIcon.resolvedStyle.unityBackgroundImageTintColor;
            slotContainerColorDefault = baseInventorySlotRoot.resolvedStyle.unityBackgroundImageTintColor;
            root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        // Returns number of items in stack remaining
        public int AddItemToSlot(ItemData itemData)
        {
            int itemsRemaining = 0;
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
            else
            {
                currentItemData = itemData;
            }

            slotFilled = true;
            SetSlotUI();

            return itemsRemaining;
        }

        public void SetStackCount(int newCount)
        {
            currentItemData.stackCount = newCount;
            slotLabel.text = newCount.ToString();
        }

        public void RemoveItemFromSlot()
        {
            currentItemData = null;
            slotIcon.style.backgroundImage = null;
            slotLabel.text = string.Empty;
            slotFilled = false;
        }

        private void SetSlotUI()
        {
            slotIcon.style.backgroundImage = currentItemData.itemSprite.texture;
            slotLabel.text = currentItemData.stackCount.ToString();
        }

        public void SetTinted()
        {
            slotIcon.style.unityBackgroundImageTintColor = GameDataManager.Instance.standardUiTintColor;
            slotLabel.style.color = GameDataManager.Instance.standardUiTintColor;
            baseInventorySlotRoot.style.unityBackgroundImageTintColor = GameDataManager.Instance.standardUiTintColor;

            Debug.Log("setting tint");

        }

        public void ResetTint()
        {
            slotIcon.style.unityBackgroundImageTintColor = iconTintColorDefault;
            slotLabel.style.color = labelColorDefault;
            baseInventorySlotRoot.style.unityBackgroundImageTintColor = slotContainerColorDefault;
        }

        public void SetHighlight()
        {
            baseInventorySlotRoot.AddToClassList("inventory-slot-highlighted");
        }

        public void ResetHighlight()
        {
            baseInventorySlotRoot.RemoveFromClassList("inventory-slot-highlighted");
        }

        public void PointerEnterSlot(PointerEnterEvent evt)
        {
            InventoryManager.Instance.UpdateCurrentHoverSlot(this, true);
            parentContainer.currentHoverSlot = this;
            AudioManager.PlaySound(MainLibrarySounds.WoodenTick);
        }

        public void PointerLeaveSlot(PointerLeaveEvent evt)
        {
            InventoryManager.Instance.UpdateCurrentHoverSlot(this, false);
            parentContainer.currentHoverSlot = null;
        }
    }
}
