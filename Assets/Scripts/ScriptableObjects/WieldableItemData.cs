using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;
using UnityEditor;
using System;

[CreateAssetMenu(fileName = "ItemData", menuName = "OneFire/Items/WieldableItemData")]
public class WieldableItemData : ItemData
{
    [Header("Wieldable Data")]
    public SoundFileObject swingSound;
    public GameObject itemHeldPrefab;

    public override void OnValidate()
    {
        base.OnValidate();

#if UNITY_EDITOR
        if (!PrefabUtility.IsPartOfPrefabAsset(itemHeldPrefab))
            Debug.LogError($"{this} {itemHeldPrefab} is a GameObject - set to prefab! ");

        if (itemHeldPrefab.GetComponent<WorldItem>() != null && itemCategories.HasFlag(ItemCategory.Wieldable))
            Debug.LogError($"{this} {itemHeldPrefab} is a WorldItem and wieldable - remove component!");
#endif
    }
}
