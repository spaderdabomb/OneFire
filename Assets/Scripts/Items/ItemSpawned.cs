using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;

public class ItemSpawned : InteractingObject
{
    public ItemData itemDataAsset;
    public ItemData itemData;

    private bool itemDataInitialized = false;

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

    public UnityAction<string> StackCountChanged;


    private void OnValidate()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Item"))
        {
            gameObject.layer = LayerMask.NameToLayer("Item");
            print($"Item {itemData.displayName} does not have a default layer assigned, assigning to 'item'");
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
}
