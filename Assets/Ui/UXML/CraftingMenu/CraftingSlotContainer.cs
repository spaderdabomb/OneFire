using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

public partial class CraftingSlotContainer
{   
    public CraftingSlot[] CraftingSlots { get; private set; }
    private CraftingStationData craftingStationData;
    public CraftingMenu parentMenu;

    public VisualElement root;

    public int slotIndex;
    public CraftingSlotContainer(VisualElement root, CraftingStationData craftingStationData, CraftingMenu parentMenu)
    {
        AssignQueryResults(root);

        this.root = root;
        this.craftingStationData = craftingStationData;
        this.parentMenu = parentMenu;

        Init();
    }

    private void Init()
    {
        CraftingSlots = new CraftingSlot[craftingStationData.recipesAvailable.Length];

        for (int i = 0; i < CraftingSlots.Length; i++)
        {
            VisualElement craftingSlotClone = CraftingManager.Instance.craftingSlotAsset.CloneTree();
            CraftingSlot craftingSlot = new CraftingSlot(craftingSlotClone, craftingStationData.recipesAvailable[i], i, this);
            CraftingSlots[i] = craftingSlot;
            slotContainer.Add(craftingSlotClone);
        }
    }

}
