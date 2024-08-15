using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace Game.Ui
{
    public partial class ItemPickupContainer : MonoBehaviour
    {
        [HideInInspector] public VisualElement root;
        [SerializeField] private VisualTreeAsset itemPickupNotificationAsset;
        [SerializeField] private float notificationTime = 2f;
        [SerializeField] ItemData inventoryFullItemData;

        private List<ItemPickupNotification> itemPickupNotifications;
        private List<float> timeElapsedList;

        public void Init(VisualElement root)
        {
            this.root = root;
            AssignQueryResults(root);

            itemPickupNotifications = new List<ItemPickupNotification>();
            timeElapsedList = new List<float>();
        }

        private void OnEnable()
        {
            InventoryManager.Instance.OnAddedItem += ShowPickupNotification;
            InventoryManager.Instance.OnInventoryFull += AddInventoryFullNotification;
        }

        private void OnDisable()
        {
            InventoryManager.Instance.OnAddedItem -= ShowPickupNotification;
            InventoryManager.Instance.OnInventoryFull -= AddInventoryFullNotification;
        }

        private void Update()
        {
            for (int i = 0; i < timeElapsedList.Count; i++)
            {
                timeElapsedList[i] += Time.deltaTime;
                if (timeElapsedList[i] > notificationTime)
                    itemPickupNotifications[i].FadeOut();
            }
        }

        public int GetNotificationIndex(ItemData itemData)
        {
            foreach (var notification in itemPickupNotifications)
            {
                ItemData tempItemData = notification.itemData;
                if (itemData.itemID == tempItemData.itemID)
                {
                    return itemPickupNotifications.IndexOf(notification);
                }
            }

            return -1;
        }

        private void AddInventoryFullNotification()
        {
            ShowPickupNotification(inventoryFullItemData);
            ItemPickupNotification pickupNotification = itemPickupNotifications[itemPickupNotifications.Count - 1];
            pickupNotification.SetItemNameLabel("Inventory Full!");
            pickupNotification.SetTotalItemsLabel(String.Empty);
        }

        public void ShowPickupNotification(ItemData itemData, bool showNotification = true)
        {
            if (!showNotification)
                return;

            int notificationIdx = GetNotificationIndex(itemData);
            if (notificationIdx != -1)
            {
                itemPickupNotifications[notificationIdx].UpdateQuantitiy(itemData.stackCount);
                timeElapsedList[notificationIdx] = 0f;
                return;
            }


            VisualElement itemPickupInst = itemPickupNotificationAsset.Instantiate();
            ItemPickupNotification pickupNotification = new ItemPickupNotification(itemPickupInst, itemData, this);
            AddNotification(pickupNotification);
        }

        public void AddNotification(ItemPickupNotification notification)
        {
            itemPickupContainer.Add(notification.root);
            itemPickupNotifications.Add(notification);
            timeElapsedList.Add(0f);
        }

        public void RemoveNotification(ItemPickupNotification pickupNotification)
        {
            int index = itemPickupNotifications.IndexOf(pickupNotification);
            if (index > itemPickupNotifications.Count)
            {
                Debug.LogError("Trying to remove item notification that doesn't exist");
                return;
            }

            itemPickupContainer.Remove(pickupNotification.root);
            timeElapsedList.RemoveAt(index);
            itemPickupNotifications.RemoveAt(index);
        }
    }
}
