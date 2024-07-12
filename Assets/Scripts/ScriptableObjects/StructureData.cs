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
    [ReadOnly] public string id = Guid.NewGuid().ToString();
    public string baseName;
    public string displayName;
    public string description;
    public string interactDescription;

    [Header("Quantities")]
    public float health = 100;
    public int baseSellValue = 1;

    [Header("Assets")]
    public ItemData itemDataAsset;
    public GameObject item3DPrefab;

    private void OnValidate()
    {
        if (!PrefabUtility.IsPartOfPrefabAsset(item3DPrefab)) 
            Debug.LogError($"{this} {item3DPrefab} is a GameObject - set to prefab!");
    }
}
