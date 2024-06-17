using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemStat", menuName = "Fishing Odyssey/Items/ItemStat")]
public class ItemStat : SerializedScriptableObject
{
    public StatType statType;
    public string displayName;
}

public enum StatType
{
    None,
    ObjectivesRequired,
    FishingLevelRequired,
    FishingBonus,
    FishingMethod,
    Speed,
}

public class ItemStats
{
    public Dictionary<ItemStat, object> statsDict = new Dictionary<ItemStat, object>();

    public void SetStat(ItemStat statName, object value)
    {
        if (statsDict.ContainsKey(statName))
        {
            statsDict[statName] = value;
        }
        else
        {
            statsDict.Add(statName, value);
        }
    }

    public T GetStat<T>(ItemStat statName, T defaultValue = default)
    {
        if (statsDict.ContainsKey(statName))
        {
            object value = statsDict[statName];
            if (value is T typedValue)
            {
                return typedValue;
            }
            else
            {
                Debug.LogWarning($"Stat '{statName}' has a different type than expected.");
                return defaultValue;
            }
        }
        else
        {
            Debug.LogWarning($"Stat '{statName}' not found. Returning default value.");
            return defaultValue;
        }
    }
}