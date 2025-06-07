using UnityEngine.UIElements;
using UnityEngine;

public partial class MaterialContainer
{
    public ItemData ItemData { get; private set; }
    public int ItemQuantity { get; private set; }
    public int ContainerIndex { get; private set; }
    private VisualElement root;
    private CraftingMenu parentCraftingMenu;
    private static string _materialsContainerEven = "materials-container-even";
    private static string _materialsContainerOdd = "materials-container-odd";
    public MaterialContainer(VisualElement root, ItemData itemData, int itemQuantity, CraftingMenu craftingMenu, int containerIndex)
    {
        AssignQueryResults(root);

        this.root = root;
        ContainerIndex = containerIndex;

        ItemData = itemData;
        ItemQuantity = itemQuantity;
        parentCraftingMenu = craftingMenu;

        Init();
    }

    private void Init()
    {
        materialIcon.style.backgroundImage = ItemData.itemSprite.texture;
        materialLabel.text = ItemData.displayName;

        UpdateMaterialContainerSlots();
        UpdateLabels(0, 0);
    }

    public void UpdateMaterialContainerSlots()
    {
        if (ContainerIndex % 2 == 0)
            return;

        materialContainer.RemoveFromClassList(_materialsContainerEven);
        materialContainer.AddToClassList(_materialsContainerOdd);
        
        Color currentColor = materialCountLabel.resolvedStyle.backgroundColor;
        currentColor.a = 0.65f;
        materialCountLabel.style.backgroundColor = new StyleColor(currentColor);
    }


    public void UpdateLabels(int numToCraft, int numRequired)
    {
        int numOwned = InventoryManager.Instance.GetNumItemOwned(ItemData);

        if (parentCraftingMenu.isCrafting)
        {
            materialCountLabel.text = (numOwned + (numToCraft - 1) * numRequired) + "/" + ItemQuantity * Mathf.Max(numRequired, 1);
        }
        else
        {
            materialCountLabel.text = numOwned + "/" + ItemQuantity * Mathf.Max(numRequired, 1);
        }

        if (numOwned + numToCraft >= ItemQuantity)
            materialCountLabel.SetEnabled(true);
        else
            materialCountLabel.SetEnabled(false);
    }
}
