using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using System.Collections.Generic;

public partial class PlayerHotbarInventory : BaseInventory
{   
    public VisualElement TemplateRoot { get; private set; }
    public PlayerHotbarInventory(VisualElement root, int numInventorySlots, string inventoryId) : base(root, numInventorySlots, inventoryId)
    {
        AssignQueryResults(root);
        TemplateRoot = root;
        root.userData = this;
        root.pickingMode = PickingMode.Ignore;

        ShowInventory();
    }

    public void ShowInventory()
    {
        playerHotbarRoot.Clear();
        AddSlotsToContainer(playerHotbarRoot);
    }

    public void AddSlotsToContainer(VisualElement containerRoot)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            slot.root.parent?.Remove(slot.root);
            containerRoot.Add(slot.root);
        }
    }
}
