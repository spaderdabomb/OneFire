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

        GameObject itemHeldObj = PrefabUtility.GetCorrespondingObjectFromSource(itemHeldPrefab);
        GameObject worldItemObj = PrefabUtility.GetCorrespondingObjectFromSource(item3DPrefab);

        if (itemHeldObj != item3DPrefab)
            Debug.Log($"{this} Item held and world item are not from same prefab root ");
#endif
    }
}
