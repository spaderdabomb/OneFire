using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;

public partial class PopupStatsContainer
{
    public KeyValuePair<ItemStat, object> itemStatsKVP;
    public int statsContainerIndex;
    public PopupStatsContainer(VisualElement root, KeyValuePair<ItemStat, object> itemsStatsKVP, int statsContainerIndex)
    {
        this.itemStatsKVP = itemsStatsKVP;
        this.statsContainerIndex = statsContainerIndex;
        AssignQueryResults(root);
        InitStatsContainer();
    }

    public void InitStatsContainer()
    {
        statDisplayNameLabel.text = itemStatsKVP.Key.displayName;
        statValueLabel.text = itemStatsKVP.Value.ToString();

        if (statsContainerIndex % 2 == 1)
        {
            statsContainerBg.style.unityBackgroundImageTintColor = new Color(0f, 0f, 0f, 0f);
        }
    }
}
