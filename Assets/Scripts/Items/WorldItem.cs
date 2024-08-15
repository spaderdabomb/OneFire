using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;
using JSAM;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(InteractingObject), typeof(Rigidbody), typeof(BoxCollider))]
[RequireComponent(typeof(Outline))]
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

        // Inventory is full so set stack count to remaining items
        if (itemsRemaining < itemData.stackCount)
        {
            SetStackCount(itemsRemaining);
        }

        if (itemsRemaining == 0)
        {
            InteractingObject.playerInteract.RemoveInteractingObject(gameObject);
            Destroy(gameObject);
        }
    }

    public void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void OnValidate()
    {
        if (GetComponent<Rigidbody>().isKinematic)
        {
            Debug.Log($"{this} is kinematic checked - automatically changing");
            GetComponent<Rigidbody>().isKinematic = false;
        }

        if (GetComponent<Rigidbody>().collisionDetectionMode != CollisionDetectionMode.Continuous)
        {
            Debug.Log($"{this} collision detection is not set to continuous - automatically changing");
            GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        if (gameObject.layer != LayerMask.NameToLayer("Item"))
        {
            Debug.Log($"{this} does not have a default layer assigned, assigning to 'item'");
            gameObject.layer = LayerMask.NameToLayer("Item");
        }

        foreach (Transform child in transform)
        {
            SetLayerRecursively(child.gameObject, LayerMask.NameToLayer("Item"));
        }

        if (GetComponent<Outline>().OutlineMode != Outline.Mode.OutlineVisible)
        {
            Debug.Log($"{this} outline mode not set to visible - automatically changing");
            GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
        }

        if (GetComponent<Outline>().OutlineWidth != 4f)
        {
            Debug.Log($"{this} outline width not set to {4f} - automatically changing");
            GetComponent<Outline>().OutlineWidth = 4f;
        }

        if (GetComponent<Outline>().OutlineColor != new Color(1, 1, 1, 0.3f))
        {
            Debug.Log($"{this} outline color not set to {new Color(1, 1, 1, 0.3f)} - automatically changing");
            GetComponent<Outline>().OutlineColor = new Color(1, 1, 1, 0.3f);
        }
    }
}
