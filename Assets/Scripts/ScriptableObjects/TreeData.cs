using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TreeData", menuName = "OneFire/Environment/TreeData")]
public class TreeData : SerializedScriptableObject
{
    [Header("Details")]
    public string id = Guid.NewGuid().ToString();
    public string interactDescription;

    [Header("Quantities")]
    public float health = 100;

    [Header("Assets")]
    public ItemData itemCollected;
    public GameObject treePrefab;

    private void OnValidate()
    {
        if (!PrefabUtility.IsPartOfPrefabAsset(treePrefab))
            Debug.LogError($"{this} {treePrefab} is a GameObject - set to prefab!");

        if (treePrefab.GetComponentInChildren<WorldTree>() == null)
            Debug.LogError($"{this} {treePrefab} does not contain a WorldTree component!");

        if (id == null || id == String.Empty)
            id = Guid.NewGuid().ToString();
    }
}
