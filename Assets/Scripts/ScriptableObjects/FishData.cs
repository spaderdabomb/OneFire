using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "FishData", menuName = "OneFire/Items/FishData")]
public class FishData : ItemData
{
    [Header("Fish Parameters")]
    public float catchExp = 10f;
    public float escapeBonus = 1f;
    public float timeToEscape = 10f;
    public int catchSegments = 3;

    [Header("Assets")] 
    public Sprite uncaughtFishIcon;

    [Header("Map")] 
    public BiomeType biomeType = BiomeType.Grassy;

    public override void OnValidate()
    {
        base.OnValidate();

        FishRegistry.Register(this);
    }
}

public static class FishDataExtensions
{
    public static readonly Dictionary<string, FishData> dataDict = new Dictionary<string, FishData>();
    public static string path = "ScriptableObjects/Fish";

    public static FishData GetFishData(string uniqueID)
    {
        if (dataDict.Count <= 0)
        {
            CreateDataDictionary();
        }

        if (dataDict.TryGetValue(uniqueID, out FishData data))
        {
            return data;
        }

        Debug.LogWarning($"Data not found: {uniqueID}");
        return null;
    }

    public static Dictionary<string, FishData> GetAllData()
    {
        if (dataDict.Count <= 0)
        {
            CreateDataDictionary();
        }

        return dataDict;
    }

    public static int GetTotalUniqueFish()
    {
        if (dataDict.Count <= 0)
        {
            CreateDataDictionary();
        }

        return dataDict.Count * Enum.GetValues(typeof(ItemRarity)).Length;
    }

    private static void CreateDataDictionary()
    {
        var scriptableObjects = Resources.LoadAll<FishData>(path);
        foreach (var scriptableObject in scriptableObjects)
        {
            dataDict.Add(scriptableObject.baseName, scriptableObject);
        }
    }

    public static FishData CreateNew(this FishData data, ItemRarity itemRarity)
    {
        FishData spawnedData = ScriptableObject.Instantiate(data);
        spawnedData.itemRarity = itemRarity;
        
        return spawnedData;
    }
}
