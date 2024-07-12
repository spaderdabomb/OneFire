using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using System.Collections.Generic;

public partial class ChestInventory : BaseInventory
{
    public ChestInventory(VisualElement root, int numInventorySlots) : base(root, numInventorySlots)
    {
        AssignQueryResults(root);
        RegisterCallbacks();
        InitInventory();
        ShowInventory();
    }

    private void RegisterCallbacks()
    {
        root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void UnregisterCallbacks()
    {
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void InitInventory()
    {
        chestContainer.Clear();

        inventorySlots = new List<InventorySlot>();
        for (int i = 0; i < numInventorySlots; i++)
        {
            VisualElement inventoryAsset = InventoryManager.Instance.inventorySlotAsset.CloneTree();
            InventorySlot inventorySlot = new InventorySlot(inventoryAsset, i, this);
            inventorySlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, inventorySlot));
            inventorySlots.Add(inventorySlot);
            chestContainer.Add(inventoryAsset);
        }
    }

    public void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (inventorySlots.Count > 0)
            chestContainer.style.width = inventorySlots[0].root.resolvedStyle.width * 6;

        if (InventoryManager.Instance.PlayerInventory.inventorySlots.Count > 0)
            playerContainer.style.width = inventorySlots[0].root.resolvedStyle.width * 8;

        Debug.Log(InventoryManager.Instance.PlayerInventory.inventorySlots[0].root.resolvedStyle.width);
    }

    public void ShowInventory()
    {
        ShowPlayerInventory();
    }

    private void ShowPlayerInventory()
    {
        playerContainer.Clear();
        InventoryManager.Instance.PlayerInventory.AddSlotsToContainer(playerContainer);
    }

    public void HideInventory()
    {

    }
}
