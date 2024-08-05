using GinjaGaming.FinalCharacterController;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

[DefaultExecutionOrder(-1)]
public class GameObjectManager : SerializedMonoBehaviour, IPersistentData
{
    public static GameObjectManager Instance;

    public GameObject player;
    public Camera playerCamera;
    public PlayerInteract playerInteract;

    public List<WorldStructure> chestList = new();
    private List<SaveableWorldStructure> saveableChestList = new();

    public List<WorldStructure> craftingStationList = new();
    private List<SaveableWorldStructure> saveableCraftingStationList = new();

    private Dictionary<Type, List<WorldStructure>> structureDataDict = new();
    private Dictionary<Type, List<SaveableWorldStructure>> saveableStructureDataDict = new();

    [Header("Containers")]
    public GameObject itemContainer;
    public GameObject structureContainer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnDroppedItem += SpawnItem;
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnDroppedItem -= SpawnItem;
    }

    private void Start()
    {
        LoadData();
        AddToSaveable();

        structureDataDict.Add(typeof(ChestData), chestList);
        structureDataDict.Add(typeof(CraftingStationData), craftingStationList);

        saveableStructureDataDict.Add(typeof(ChestData), saveableChestList);
        saveableStructureDataDict.Add(typeof(CraftingStationData), saveableCraftingStationList);
    }

    public void AddWorldStructure(WorldStructure worldStructure)
    {
        List<WorldStructure> structureList = structureDataDict[worldStructure.structureDataAsset.GetType()];
        List<SaveableWorldStructure> saveableStructureList = saveableStructureDataDict[worldStructure.structureDataAsset.GetType()];

        print(structureList == chestList);
        print(structureDataDict[worldStructure.structureDataAsset.GetType()] == chestList);

        worldStructure.InstanceId = structureList.Count;
        structureList.Add(worldStructure);
        SaveableWorldStructure saveableStructure = new SaveableWorldStructure(worldStructure);
        saveableStructureList.Add(saveableStructure);
    }

    public void SpawnItem(ItemData itemData)
    {
        ItemData itemDataCloned = itemData.CloneItemData();
        GameObject newItemSpawned = Instantiate(itemDataCloned.item3DPrefab, itemContainer.transform);
        WorldItem newItemSpanwedInst = newItemSpawned.GetComponent<WorldItem>();
        newItemSpanwedInst.InitItemData(itemDataCloned);

        float speedScaleFactor = 2f;
        float angularSpeed = 7f;
        newItemSpawned.transform.position = playerCamera.transform.position;
        Rigidbody newItemRb = newItemSpawned.GetComponent<Rigidbody>();

        newItemRb.angularVelocity = new Vector3(Random.Range(0f, angularSpeed), Random.Range(0f, angularSpeed), Random.Range(0f, angularSpeed));
        newItemRb.velocity = new Vector3(
            speedScaleFactor * Mathf.Sign(playerCamera.transform.forward.x) * Random.Range(0f, Mathf.Abs(playerCamera.transform.forward.x)),
            Random.Range(0.5f, 2f),
            speedScaleFactor * Mathf.Sign(playerCamera.transform.forward.z) * Random.Range(0f, Mathf.Abs(playerCamera.transform.forward.z))
            );

    }

    public void AddToSaveable()
    {
        PersistentDataManager.Instance.AddToPersistentDataList(this);
    }

    public void LoadData()
    {
        saveableChestList = ES3.Load(nameof(saveableChestList), defaultValue: saveableChestList);
        chestList = LoadStructures(saveableChestList);

        saveableCraftingStationList = ES3.Load(nameof(saveableCraftingStationList), defaultValue: saveableCraftingStationList);
        craftingStationList = LoadStructures(saveableCraftingStationList);
    }

    public List<WorldStructure> LoadStructures(List<SaveableWorldStructure> saveableWorldStructures)
    {
        List<WorldStructure> newStructureDict = new();

        foreach (var currentWorldStructure in saveableWorldStructures)
        {
            if (currentWorldStructure != null)
            {
                StructureData chestDataAsset = StructureRegistry.GetStructureData(currentWorldStructure.assetId);
                GameObject newWorldStructureObj = Instantiate(chestDataAsset.structurePrefab, currentWorldStructure.position, currentWorldStructure.rotation, structureContainer.transform);
                WorldStructure newWorldStructure = newWorldStructureObj.GetComponent<WorldStructure>();
                newWorldStructure.InstanceId = currentWorldStructure.instanceId;
                newStructureDict.Add(newWorldStructure);
            }
        }

        return newStructureDict;
    }

    public void SaveData()
    {
        ES3.Save(nameof(saveableChestList), saveableChestList);
        ES3.Save(nameof(saveableCraftingStationList), saveableCraftingStationList);
    }
}