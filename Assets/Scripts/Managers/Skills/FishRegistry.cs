using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FishRegistry
{
    private static Dictionary<string, FishData> fishDictionary = new Dictionary<string, FishData>();

    static FishRegistry()
    {
        fishDictionary.Clear();
    }

    public static bool Register(FishData fish)
    {
        if (fishDictionary.ContainsKey(fish.itemID))
        {
            if (fishDictionary[fish.itemID].baseName != fish.baseName)
                Debug.LogError($"Two fish have duplicate id - in database: {fishDictionary[fish.itemID].baseName}; adding to registry {fish.baseName}");

            return false;
        }

        fishDictionary[fish.itemID] = fish;
        return true;
    }

    public static void Unregister(FishData fish)
    {
        fishDictionary.Remove(fish.itemID);
    }

    public static FishData GetItem(string id)
    {
        return fishDictionary.ContainsKey(id) ? fishDictionary[id] : null;
    }
}
