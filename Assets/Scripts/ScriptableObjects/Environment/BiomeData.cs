using System;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using System.Linq;

[CreateAssetMenu(fileName = "BiomeData", menuName = "ZenFisher/Biomes/BiomeData")]
public class BiomeData : SerializedScriptableObject
{
    [Header("General")] 
    public string biomeID;
    public string displayName;
    public BiomeType biomeType;

    private void OnValidate()
    {
#if UNITY_EDITOR
        biomeID = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }
}

public static class BiomeExtensions
{
    private static readonly Dictionary<string, BiomeData> dataDict = new Dictionary<string, BiomeData>();
    private static readonly Dictionary<BiomeType, List<FishData>> allFishInBiomesDict = new Dictionary<BiomeType, List<FishData>>();
    public static BiomeData GetItemData(string uniqueID)
    {
        if (dataDict.Count <= 0)
        {
            var scriptableObjects = Resources.LoadAll<BiomeData>("ScriptableObjects/Biomes");
            foreach (var scriptableObject in scriptableObjects)
            {
                dataDict.Add(scriptableObject.biomeID, scriptableObject);
            }
        }

        if (dataDict.TryGetValue(uniqueID, out BiomeData data))
        {
            return data;
        }

        Debug.LogWarning($"Data not found: {uniqueID}");
        return null;
    }

    public static Dictionary<string, BiomeData> GetAllData()
    {
        if (dataDict.Count <= 0)
        {
            var scriptableObjects = Resources.LoadAll<BiomeData>("ScriptableObjects/Biomes");
            foreach (var scriptableObject in scriptableObjects)
            {
                dataDict.Add(scriptableObject.biomeID, scriptableObject);
            }
        }

        return dataDict;
    }

    public static Dictionary<BiomeType, List<FishData>> GetAllFishTypesInBiomes()
    {
        if (allFishInBiomesDict.Count == 0 || allFishInBiomesDict.Values.Any(v => v == null))
        {
            allFishInBiomesDict.Clear(); 

            Dictionary<string, FishData> fishData = FishRegistry.fishDictionary;

            foreach (var fish in fishData.Values)
            {
                if (fish == null) continue;

                if (!allFishInBiomesDict.TryGetValue(fish.biomeType, out var fishList) || fishList == null)
                {
                    fishList = new List<FishData>();
                }

                fishList.Add(fish);
                allFishInBiomesDict[fish.biomeType] = fishList;
            }
        }

        return allFishInBiomesDict;
    }
}

public enum BiomeType
{
    [Description("Tutorial Island")]
    TutorialIsland = 0,
    [Description("Grassy Biome")]
    Grassy = 1,
    [Description("Desert Island")]
    Desert = 2
}

