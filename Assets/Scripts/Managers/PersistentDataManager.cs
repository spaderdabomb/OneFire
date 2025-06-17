using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-4)]
public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance;

    [SerializeField] private bool clearData;
    public List<IPersistentData> PersistentData = new List<IPersistentData>();

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
        if (!PersistentData.Contains(newData))
        {
            PersistentData.Add(newData);
        }
    }

    public void SaveAllData()
    {
        print("Saving all data");
        foreach (var persistable in PersistentData)
            persistable.SaveData();
    }

    public void ClearAllData()
    {
        ES3.DeleteFile();
    }
}

public interface IPersistentData
{
    void AddToSaveable();
    void LoadData();
    void SaveData();
}
