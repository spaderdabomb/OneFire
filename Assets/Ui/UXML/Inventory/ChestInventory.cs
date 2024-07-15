using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using System.Collections.Generic;

public partial class ChestInventory : BaseInventory
{
    private ExitButton chestExitButton;
    public ChestInventory(VisualElement root, int numInventorySlots, string inventoryId) : base(root, numInventorySlots, inventoryId)
    {
        AssignQueryResults(root);
        RegisterCallbacks();
        InitInventory();
        ShowInventory();
        InitMenu();

        root.pickingMode = PickingMode.Ignore;
    }

    private void RegisterCallbacks()
    {
        root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
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

    private void InitMenu()
    {
        chestExitButton = new ExitButton(exitButton);
    }

    public void OnGeometryChanged(GeometryChangedEvent evt)
    {
        if (inventorySlots.Count > 0)
            chestContainer.style.width = inventorySlots[0].root.resolvedStyle.width * 6;

        if (InventoryManager.Instance.PlayerInventory.inventorySlots.Count > 0)
            playerContainer.style.width = inventorySlots[0].root.resolvedStyle.width * 8;

        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
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
