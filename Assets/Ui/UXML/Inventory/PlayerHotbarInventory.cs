using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using System.Collections.Generic;

public partial class PlayerHotbarInventory : BaseInventory
{   
    public PlayerHotbarInventory(VisualElement root, int numInventorySlots, string inventoryId, bool selectable): base(root, numInventorySlots, inventoryId, selectable)
    {
        AssignQueryResults(root);
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

    public void ShowMenu()
    {
        root.style.display = DisplayStyle.Flex;
    }

    public void HideMenu()
    {
        root.style.display = DisplayStyle.None;
    }
}
