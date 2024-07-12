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

    private void Awake()
    {
        InteractingObject = GetComponent<InteractingObject>();
    }

    private void Start()
    {
        structureData = Instantiate(structureDataAsset);
        SetDisplay();
    }

    public void InteractWithStructure()
    {
        if (structureData.GetType() == typeof(ChestData))
        {
            InventoryManager.Instance.OpenChestInventory((ChestData)structureData);
        }
    }

    public void SetDisplay()
    {
        InteractingObject.DisplayPretext = structureData.interactDescription;
        InteractingObject.DisplayString = structureData.displayName;
    }

    private void OnValidate()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Structure"))
        {
            gameObject.layer = LayerMask.NameToLayer("Structure");
            print($"Structure {structureData.displayName} does not have a default layer assigned, assigning to 'Structure'");
        }
    }
}
