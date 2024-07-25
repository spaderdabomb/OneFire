using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;
using JSAM;

[RequireComponent(typeof(InteractingObject), typeof(Rigidbody), typeof(BoxCollider))]
public class WorldItem : MonoBehaviour
{
    public ItemData itemDataAsset;
    [ReadOnly] public ItemData itemData;

    public InteractingObject InteractingObject {  get; private set; }
    public Action<string> StackCountChanged;

    private void Awake()
    {
        InteractingObject = GetComponent<InteractingObject>();
    }

    private void Start()
    {
        if (itemData == null)
            InitItemData(Instantiate(itemDataAsset));
    }

    public void InitItemData(ItemData newItemData)
    {
        itemData = newItemData;
        SetStackCount(itemData.stackCount);
    }

    public void SetStackCount(int newStackCount)
    {
        itemData.stackCount = newStackCount;
        SetDisplay();
    }

    public void SetDisplay()
    {
        InteractingObject.DisplayPretext = itemData.interactDescription;
        InteractingObject.DisplayString = itemData.displayName + " x" + itemData.stackCount.ToString();
        StackCountChanged?.Invoke(InteractingObject.DisplayString);
    }

    public void PickUpItem()
    {
        ItemData clonedItemData = itemData.CloneItemData();
        int itemsRemaining = InventoryManager.Instance.TryAddItem(clonedItemData);

        if (itemsRemaining == itemData.stackCount)
        {
            AudioManager.PlaySound(MainLibrarySounds.InventoryFull);
        }

        if (itemsRemaining < itemData.stackCount)
        {
            SetStackCount(itemsRemaining);
            AudioManager.PlaySound(MainLibrarySounds.ItemPickup);
        }

        if (itemsRemaining == 0)
        {
            InteractingObject.playerInteract.RemoveInteractingObject(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        GetComponent<Rigidbody>().isKinematic = false;

        if (gameObject.layer != LayerMask.NameToLayer("Item"))
        {
            gameObject.layer = LayerMask.NameToLayer("Item");
            print($"Item {itemData.displayName} does not have a default layer assigned, assigning to 'item'");
        }
    }
}
