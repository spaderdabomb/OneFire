using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public partial class PopupMenuInventory
{
    ItemData itemData = null;
    public VisualElement root;
    public List<PopupStatsContainer> popupStatsList;
    public bool itemDataShowing = false;

    public PopupMenuInventory(VisualElement root)
    {
        this.root = root;
        AssignQueryResults(root);
    }

    public void SetItemData(ItemData newItemData)
    {
        itemData = newItemData;
        popupStatsList = new List<PopupStatsContainer>();

        if (itemData == null)
            return;

        itemNameLabel.text = itemData.displayName;
        itemDescriptionLabel.text = itemData.description;
        itemTypeLabel.text = itemData.itemType.ToString();
        shopSellLabel.text = itemData.baseSellValue.ToString();

        if (newItemData.itemStatList.Count > 0)
        {
            SetItemStats();
        }

        itemDataShowing = true;
    }

    public void SetItemStats()
    {
        foreach (var (itemStatsKVP, index) in itemData.itemStats.statsDict.Select((value, i) => (value, i)))
        {
            VisualElement statsContainerAsset = UiManager.Instance.popupMenuInventoryStatsContainer.CloneTree();
            PopupStatsContainer newStatsContainer = new PopupStatsContainer(statsContainerAsset, itemStatsKVP, index);
            statsListContainer.Add(statsContainerAsset);
            popupStatsList.Add(newStatsContainer);
        }
    }

    public void RemoveItemData()
    {
        statsListContainer.Clear();
        itemDataShowing = false;
    }
}
