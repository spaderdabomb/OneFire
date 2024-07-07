using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using System.Collections.Generic;

public partial class PlayerHotbarInventory : BaseInventory
{   
    public VisualElement TemplateRoot { get; private set; }
    public PlayerHotbarInventory(VisualElement root, int inventoryRows, int inventoryCols) : base(root, inventoryRows, inventoryCols)
    {
        TemplateRoot = root;
        AssignQueryResults(root);
        Init();
    }

    public void Init()
    {
        playerHotbarRoot.Clear();

        // Init slots
        inventorySlots = new List<InventorySlot>();
        for (int i = 0; i < inventoryRows; i++)
        {
            for (int j = 0; j < inventoryCols; j++)
            {
                VisualElement inventoryAsset = InventoryManager.Instance.inventorySlotAsset.CloneTree();
                InventorySlot inventorySlot = new InventorySlot(inventoryAsset, j + i * inventoryCols, this);
                inventorySlot.root.RegisterCallback<PointerDownEvent>(evt => InventoryManager.Instance.BeginDragHandler(evt, inventorySlot));
                inventorySlots.Add(inventorySlot);
                playerHotbarRoot.Add(inventoryAsset);
            }
        }
    }
}
