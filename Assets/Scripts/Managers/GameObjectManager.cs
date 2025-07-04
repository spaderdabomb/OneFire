using GinjaGaming.FinalCharacterController;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class GameObjectManager : SerializedMonoBehaviour, IPersistentData
{
    public static GameObjectManager Instance;

    public GameObject player;
    public Camera playerCamera;
    public Camera uiCamera;
    public PlayerInteract playerInteract;
    public PlayerStats playerStats;
    public PlayerState playerState;
    public PlayerAnimation playerAnimation;
    public PlayerEquippedItem playerEquippedItem;

    public List<WorldStructure> chestList = new();
    private List<SaveableWorldStructure> saveableChestList = new();

    public List<WorldStructure> craftingStationList = new();
    private List<SaveableWorldStructure> saveableCraftingStationList = new();

    private Dictionary<Type, List<WorldStructure>> structureDataDict = new();
    private Dictionary<Type, List<SaveableWorldStructure>> saveableStructureDataDict = new();

    [Header("Containers")]
    public GameObject itemContainer;
    public GameObject structureContainer;
    public GameObject effectsContainer;

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

    private void Start()
    {
        RegisterCallbacks();

        LoadData();
        AddToSaveable();

        structureDataDict.Add(typeof(ChestData), chestList);
        structureDataDict.Add(typeof(CraftingStationData), craftingStationList);

        saveableStructureDataDict.Add(typeof(ChestData), saveableChestList);
        saveableStructureDataDict.Add(typeof(CraftingStationData), saveableCraftingStationList);

        uiCamera.depth = 99;
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        InventoryManager.Instance.OnDroppedItem += SpawnItem;
    }

    private void UnregisterCallbacks()
    {
        InventoryManager.Instance.OnDroppedItem -= SpawnItem;
    }

    public void AddWorldStructure(WorldStructure worldStructure)
    {
        List<WorldStructure> structureList = structureDataDict[worldStructure.structureDataAsset.GetType()];
        List<SaveableWorldStructure> saveableStructureList = saveableStructureDataDict[worldStructure.structureDataAsset.GetType()];

        worldStructure.InstanceId = structureList.Count;
        structureList.Add(worldStructure);
        SaveableWorldStructure saveableStructure = new SaveableWorldStructure(worldStructure);
        saveableStructureList.Add(saveableStructure);
    }

    public void SpawnItem(ItemData itemData)
    {
        if (itemData.stackCount == 0)
            return;

        ItemData itemDataCloned = itemData.CloneItem();
        GameObject newItemSpawned = Instantiate(itemDataCloned.item3DPrefab, itemContainer.transform);
        WorldItem newItemSpanwedInst = newItemSpawned.GetComponent<WorldItem>();
        newItemSpanwedInst.InitItemData(itemDataCloned);

        float speedScaleFactor = 2f;
        float angularSpeed = 7f;
        newItemSpawned.transform.position = playerCamera.transform.position;
        Rigidbody newItemRb = newItemSpawned.GetComponent<Rigidbody>();

        newItemRb.angularVelocity = new Vector3(Random.Range(0f, angularSpeed), Random.Range(0f, angularSpeed), Random.Range(0f, angularSpeed));
        newItemRb.linearVelocity = new Vector3(
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