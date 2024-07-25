using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class ItemRegistry
{
    private static Dictionary<string, ItemData> itemDictionary = new Dictionary<string, ItemData>();

    static ItemRegistry()
    {
        itemDictionary.Clear();
    }

    public static bool Register(ItemData item)
    {
        if (itemDictionary.ContainsKey(item.itemID))
        {
            return false; // Duplicate found
        }

        itemDictionary[item.itemID] = item;
        return true;
    }

    public static void Unregister(ItemData item)
    {
        itemDictionary.Remove(item.itemID);
    }
}
