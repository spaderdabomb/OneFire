using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "StructureData", menuName = "OneFire/Structures/StructureData")]
public class StructureData : SerializedScriptableObject
{
    [Header("Details")]
    public string id = Guid.NewGuid().ToString();
    public string interactDescription;

    [Header("Quantities")]
    public float health = 100;
    public int baseSellValue = 1;

    [Header("Assets")]
    public ItemData itemDataAsset;
    public GameObject structurePrefab;
    public GameObject structurePreviewPrefab;

    private void OnValidate()
    {
        if (!PrefabUtility.IsPartOfPrefabAsset(structurePrefab)) 
            Debug.LogError($"{this} {structurePrefab} is a GameObject - set to prefab!");

        if (!PrefabUtility.IsPartOfPrefabAsset(structurePreviewPrefab))
            Debug.LogError($"{this} {structurePreviewPrefab} is a GameObject - set to prefab!");

        if (structurePrefab.GetComponent<WorldStructure>() == null)
            Debug.LogError($"{this} {structurePrefab} does not contain a WorldStructure component!");

        if (id == null || id == String.Empty)
            id = Guid.NewGuid().ToString();

        if (!structurePreviewPrefab.GetComponent<Collider>().isTrigger)
            Debug.LogError($"{this} {structurePreviewPrefab} trigger is not checked!");

        if (!StructureRegistry.Register(this))
            Debug.LogError($"Duplicate structure id found for {this}. Please regenerate the ID.");
    }
}
