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
        AddSlotsToContainer(chestContainer);
        ShowInventory();
        InitMenu();

        root.pickingMode = PickingMode.Ignore;
    }

    private void RegisterCallbacks()
    {
        root.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        allToChestButton.clickable.clicked += DepositAllToChest;
        allToInventoryButton.clickable.clicked += DepositAllToInventory;
        sameToChestButton.clickable.clicked += DepositSameToChest;
        sameToInventoryButton.clickable.clicked += DepositSameToInventory;
    }

    public void UnregisterCallbacks()
    {
        root.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        allToChestButton.clickable.clicked -= DepositAllToChest;
        allToInventoryButton.clickable.clicked -= DepositAllToInventory;
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

    public void AddSlotsToContainer(VisualElement containerRoot)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            slot.root.parent?.Remove(slot.root);
            containerRoot.Add(slot.root);
        }
    }

    public void ShowInventory()
    {
        UiManager.Instance.uiGameManager.PlayerInteractionMenu.root.Clear();
        UiManager.Instance.uiGameManager.PlayerInteractionMenu.root.Add(root);
        ShowPlayerInventory();
    }

    private void ShowPlayerInventory()
    {
        playerContainer.Clear();
        InventoryManager.Instance.PlayerInventory.AddSlotsToContainer(playerContainer);
    }

    private void DepositAllToInventory()
    {
        InventoryManager.Instance.DepositAll(this, InventoryManager.Instance.PlayerInventory);
    }

    private void DepositAllToChest()
    {
        InventoryManager.Instance.DepositAll(InventoryManager.Instance.PlayerInventory, this);
    }

    private void DepositSameToInventory()
    {
        InventoryManager.Instance.DepositSame(this, InventoryManager.Instance.PlayerInventory);
    }

    private void DepositSameToChest()
    {
        InventoryManager.Instance.DepositSame(InventoryManager.Instance.PlayerInventory, this);
    }
}
