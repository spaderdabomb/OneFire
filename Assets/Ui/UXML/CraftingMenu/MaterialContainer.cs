using UnityEngine.UIElements;
using UnityEngine;

public partial class MaterialContainer
{
    public ItemData ItemData { get; private set; }
    public int ItemQuantity { get; private set; }
    public MaterialContainer(VisualElement root, ItemData itemData, int itemQuantity)
    {
        AssignQueryResults(root);

        ItemData = itemData;
        ItemQuantity = itemQuantity;

        Init();
    }

    private void Init()
    {
        materialIcon.style.backgroundImage = ItemData.itemSprite.texture;
        materialLabel.text = ItemData.displayName;

        UpdateLabels(0, 0);
    }

    public void UpdateLabels(int numToCraft, int numRequired)
    {
        int numOwned = InventoryManager.Instance.GetNumItemOwned(ItemData);
        materialCountLabel.text = (numOwned + numToCraft) + "/" + ItemQuantity * Mathf.Max(numRequired, 1);

        if (numOwned + numToCraft >= ItemQuantity)
            materialCountLabel.SetEnabled(true);
        else
            materialCountLabel.SetEnabled(false);
    }
}
