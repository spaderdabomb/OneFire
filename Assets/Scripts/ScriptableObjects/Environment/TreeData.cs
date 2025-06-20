using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeData", menuName = "OneFire/Environment/TreeData")]
public class TreeData : SerializedScriptableObject, IDamageable
{
    [Header("Details")]
    public string id = Guid.NewGuid().ToString();
    public string interactDescription;

    [Header("Quantities")]
    [SerializeField] private float baseHealth = 100;
    public float BaseHealth => baseHealth;
    public float logsPerHealth;
    public int logsWhenDestroyed;
    public float respawnTime;

    [Header("Assets")]
    public ItemData itemCollected;
    public GameObject treePrefab;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!PrefabUtility.IsPartOfPrefabAsset(treePrefab))
            Debug.LogError($"{this} {treePrefab} is a GameObject - set to prefab!");
#endif

        if (treePrefab.GetComponentInChildren<WorldTree>() == null)
            Debug.LogError($"{this} {treePrefab} does not contain a WorldTree component!");

        if (id == null || id == String.Empty)
            id = Guid.NewGuid().ToString();
    }
}
