using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "VeinData", menuName = "OneFire/Environment/VeinData")]
public class VeinData : SerializedScriptableObject, IDamageable
{
    [Header("Details")]
    public string id = Guid.NewGuid().ToString();
    public string interactDescription;

    [Header("Quantities")]
    [SerializeField] private float baseHealth = 100;
    public float BaseHealth => baseHealth;
    public float orePerHealth;
    public int oreWhenDestroyed;
    public float respawnTime;

    [Header("Assets")]
    public ItemData itemCollected;
    public GameObject veinPrefab;

    private void OnValidate()
    {
        if (!PrefabUtility.IsPartOfPrefabAsset(veinPrefab))
            Debug.LogError($"{this} {veinPrefab} is a GameObject - set to prefab!");

        if (veinPrefab.GetComponentInChildren<WorldVein>() == null)
            Debug.LogError($"{this} {veinPrefab} does not contain a WorldVein component!");

        if (id == null || id == String.Empty)
            id = Guid.NewGuid().ToString();
    }
}
