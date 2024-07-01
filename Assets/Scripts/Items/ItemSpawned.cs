using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;
using JSAM;

public class ItemSpawned : InteractingObject
{
    public ItemData itemDataAsset;
    [ReadOnly] public ItemData itemData;

    private bool itemDataInitialized = false;

    public Action<string> StackCountChanged;
    private string displayString = string.Empty;
    protected override string DisplayString
    {
        get { return displayString; }
        set
        {
            displayString = value;
            StackCountChanged?.Invoke(displayString);
        }
    }

    protected override void Start()
    {
        base.Start();

        if (!itemDataInitialized)
        {
            InitItemData();
            itemDataInitialized = true;
        }
    }

    public void InitItemData()
    {
        itemData = Instantiate(itemDataAsset);
        SetStackCount(itemData.stackCount);
        itemDataInitialized = true;
    }

    public void SetStackCount(int newStackCount)
    {
        itemData.stackCount = newStackCount;
        DisplayString = itemData.displayName + " x" + itemData.stackCount.ToString();
    }

    public void PickUpItem()
    {
        int itemsRemaining = InventoryManager.Instance.PlayerInventory.TryAddItem(itemData);

        if (itemsRemaining < itemData.stackCount)
        {
            AudioManager.PlaySound(MainLibrarySounds.ItemPickup);
        }

        if (itemsRemaining == 0)
        {
            playerInteract.RemoveInteractingObject(gameObject);
            Destroy(gameObject);
        }
    }

    private void OnValidate()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Item"))
        {
            gameObject.layer = LayerMask.NameToLayer("Item");
            print($"Item {itemData.displayName} does not have a default layer assigned, assigning to 'item'");
        }
    }
}
