using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[RequireComponent(typeof(InteractingObject))]
public class WorldStructure : MonoBehaviour
{
    public StructureData structureDataAsset;
    [ReadOnly] public StructureData structureData;
    public InteractingObject InteractingObject { get; private set; }

    [field: SerializeField] public int InstanceId { get; set; } = -1;

    private void Awake()
    {
        InteractingObject = GetComponent<InteractingObject>();
        structureData = Instantiate(structureDataAsset);
    }

    private void Start()
    {
        SetDisplay();
    }

    public void InteractWithStructure()
    {
        if (structureData.GetType() == typeof(ChestData))
        {
            InventoryManager.Instance.OpenChestInventory((ChestData)structureData, InstanceId);
        }
        else if (structureData.GetType() == typeof(CraftingStationData))
        {
            UiManager.Instance.uiGameManager.ToggleCraftingMenu((CraftingStationData)structureData, InstanceId);
        }
    }

    public void SetDisplay()
    {
        InteractingObject.DisplayPretext = structureData.interactDescription;
        InteractingObject.DisplayString = structureData.itemDataAsset.displayName;
    }

    private void OnValidate()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Structure"))
        {
            gameObject.layer = LayerMask.NameToLayer("Structure");
            print($"Structure {structureData.itemDataAsset.displayName} does not have a default layer assigned, assigning to 'Structure'");
        }
    }
}

public class SaveableWorldStructure
{
    public int instanceId;
    public string assetId;
    public Vector3 position;
    public Quaternion rotation;

    public SaveableWorldStructure(WorldStructure worldStructure)
    {
        instanceId = worldStructure.InstanceId;
        assetId = worldStructure.structureDataAsset.id;
        position = worldStructure.transform.position;
        rotation = worldStructure.transform.rotation;
    }
}
