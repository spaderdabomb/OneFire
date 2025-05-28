using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance;

    [SerializeField] private bool clearData;
    public List<IPersistentData> persistentData = new List<IPersistentData>();
    
    [HideInInspector] public int TotalFishCaught { get; set; } = 0;
    [HideInInspector] public Dictionary<string, bool> FishCaughtDict { get; set; } = new Dictionary<string, bool>();
    [HideInInspector] public Dictionary<string, DateTime> FishDateCaughtDict { get; set; } = new Dictionary<string, DateTime>();
    [HideInInspector] public Dictionary<string, int> FishTotalCaughtDict { get; set; } = new Dictionary<string, int>();
    [HideInInspector] public Dictionary<string, float> FishCaughtBestWeightDict { get; set; } = new Dictionary<string, float>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (clearData)
        {
            ClearAllData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveAllData();
    }

    public void AddToPersistentDataList(IPersistentData newData)
    {
        if (!persistentData.Contains(newData))
        {
            persistentData.Add(newData);
        }
    }

    public void SaveAllData()
    {
        print("Saving all data");

        // IPersistentData[] persistables = FindObjectsOfType<MonoBehaviour>().OfType<IPersistentData>().ToArray();
        foreach (var persistable in persistentData)
        {
            persistable.SaveData();
        }
    }

    public void ClearAllData()
    {
        ES3.DeleteFile();
    }
    
    #region Utility Methods for saved values

    public int GetTotalFishCollected()
    {
        return FishCaughtDict.Count;
    }

    public bool IsFishCaught(string fishID)
    {
        if (FishCaughtDict.TryGetValue(fishID, out bool fishCaught))
        {
            return fishCaught;
        }
        else
        {
            return false;
        }
    }

    public bool IsFishTypeCaught(FishData fishData)
    {
        string fishName = fishData.baseName;
        foreach (ItemRarity itemRarity in Enum.GetValues(typeof(ItemRarity)))
        {
            string currentFishID = fishName + itemRarity;
            if (FishCaughtDict.TryGetValue(currentFishID, out bool fishCaught))
            {
                return fishCaught;
            }
        }

        return false;
    }

    public int GetFishCollectedInBiome(BiomeType biomeType)
    {
        int fishCollectedInBiome = 0;
        List<FishData> biomeFishData = BiomeExtensions.GetAllFishInBiomes()[biomeType];
        foreach (FishData biomeFish in  biomeFishData)
        {
            if (FishCaughtDict.TryGetValue(biomeFish.itemID, out bool fishCaught))
            {
                if (fishCaught)
                {
                    fishCollectedInBiome++;
                }
            }
        }

        return fishCollectedInBiome;
    }

    public static void IncrementOrCreateKey(Dictionary<string, int> dictionary, string key)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key]++;
        }
        else
        {
            dictionary[key] = 1;
        }
    }

    static void UpdateIfBiggerOrCreateKeyWithFloatValue(Dictionary<string, float> dictionary, string key, float newValue)
    {
        if (dictionary.ContainsKey(key))
        {
            if (newValue > dictionary[key])
            {
                dictionary[key] = newValue;
            }
        }
        else
        {
            dictionary[key] = newValue;
        }
    }

    #endregion
}

public interface IPersistentData
{
    void AddToSaveable();
    void LoadData();
    void SaveData();
}
