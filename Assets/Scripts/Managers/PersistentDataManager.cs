using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PersistentDataManager : MonoBehaviour
{
    public static PersistentDataManager Instance;

    [SerializeField] private bool clearData;
    public List<IPersistentData> persistentData = new List<IPersistentData>();

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
            print($"persistable save: {persistable}");
        }
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
