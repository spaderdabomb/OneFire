using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using System.Collections.Generic;

public partial class PlayerHotbarInventory : BaseInventory
{   
    public VisualElement TemplateRoot { get; private set; }
    public PlayerHotbarInventory(VisualElement root, int numInventorySlots) : base(root, numInventorySlots)
    {
        TemplateRoot = root;
        AssignQueryResults(root);
        Init();

        root.pickingMode = PickingMode.Ignore;
    }

    public void Init()
    {
        playerHotbarRoot.Clear();

        // Init slots
        inventorySlots = new List<InventorySlot>();
        for (int i = 0; i < numInventorySlots; i++)
        {
            VisualElement inventoryAsset = InventoryManager.Instance.inventorySlotAsset.CloneTree();
            InventorySlot inventorySlot = new InventorySlot(inventoryAsset, i, this);
            inventorySlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, inventorySlot));
            inventorySlots.Add(inventorySlot);
            playerHotbarRoot.Add(inventoryAsset);
        }
    }
}
