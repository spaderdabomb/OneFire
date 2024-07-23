using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingStationData", menuName = "OneFire/Crafting/CraftingStationData")]
public class CraftingStationData : StructureData
{
    [Header("Classification")]
    public RecipeData[] recipesAvailable;

}

public static class CraftingStationExtensions
{
    private static readonly Dictionary<string, CraftingStationData> _recipes = new Dictionary<string, CraftingStationData>();
    public static CraftingStationData GetCraftingStationData(string uniqueID)
    {
        if (_recipes.Count <= 0)
        {
            var recipes = Resources.LoadAll<CraftingStationData>("ScriptableObjects/CraftingStations");
            foreach (var item in recipes)
            {
                _recipes.Add(item.id, item);
            }
        }

        if (_recipes.TryGetValue(uniqueID, out CraftingStationData craftingStationData))
        {
            return craftingStationData;
        }

        Debug.LogWarning($"Recipe not found: {uniqueID}");
        return null;
    }

    public static CraftingStationData GetCraftingStationDataInstantiated(this CraftingStationData craftingStationData)
    {
        CraftingStationData originalCraftingStationData = GetCraftingStationData(craftingStationData.id);
        if (originalCraftingStationData == null)
            return null;

        CraftingStationData spawnedCraftingStationData = ScriptableObject.Instantiate(originalCraftingStationData);
        return spawnedCraftingStationData;
    }

    public static CraftingStationData CloneCraftingStationData(this CraftingStationData data)
    {
        CraftingStationData spawnedData = ScriptableObject.Instantiate(data);
        return spawnedData;
    }
}