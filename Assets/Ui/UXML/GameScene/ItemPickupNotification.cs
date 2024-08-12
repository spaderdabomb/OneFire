using UnityEngine.UIElements;
using UnityEngine;
using System;

namespace Game.Ui
{
    public partial class ItemPickupNotification
    {
        public VisualElement root;
        public ItemData itemData;

        private ItemPickupContainer parentContainer;
        public ItemPickupNotification(VisualElement root, ItemData itemData, ItemPickupContainer parentContainer)
        {
            this.itemData = itemData;
            this.root = root;
            this.parentContainer = parentContainer;
            AssignQueryResults(root);
            RegisterCallbacks();
            Init();
        }

        private void Init()
        {
            string displayStr = "+" + itemData.stackCount + "  " + itemData.displayName;
            itemNameLabel.text = displayStr;

            itemIcon.style.backgroundImage = itemData.itemSprite.texture;
            totalItemsLabel.text = "(" + InventoryManager.Instance.GetNumItemOwned(itemData).ToString() + ")";
        }

        private void RegisterCallbacks()
        {
            root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void UnregisterCallbacks()
        {
            root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            FadeIn();
        }

        public void UpdateQuantitiy(int additionalQuantity)
        {
            itemData.stackCount += additionalQuantity;
            string displayStr = "+" + itemData.stackCount + "  " + itemData.displayName;

            itemNameLabel.text = displayStr;
            totalItemsLabel.text = "(" + InventoryManager.Instance.GetNumItemOwned(itemData).ToString() + ")";
        }

        public void SetItemNameLabel(string displayStr)
        {
            itemNameLabel.text = displayStr;
        }

        public void SetTotalItemsLabel(string displayStr)
        {
            totalItemsLabel.text = displayStr;
        }

        public void FadeIn()
        {
            itemPickupNotificationBg.AddToClassList("visible");
        }

        private void FadeOutFinished(TransitionEndEvent evt)
        {
            parentContainer.RemoveNotification(this);
        }

        public void FadeOut()
        {
            itemPickupNotificationBg.RemoveFromClassList("visible");
            itemPickupNotificationBg.AddToClassList("hidden");
            itemPickupNotificationBg.RegisterCallback<TransitionEndEvent>(FadeOutFinished);
        }
    }
}
