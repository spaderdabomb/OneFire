using JSAM;
using OneFireUi;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerEquippedItem : SerializedMonoBehaviour
{
    [SerializeField] private Transform armature;
    [SerializeField] private Transform itemContainerR;
    [SerializeField] private Dictionary<ItemType, AnimationClip> itemTypeToAnimationClipDict = new();

    [Header("Weapon Effects")]
    [SerializeField] private ParticleSystem weaponSlash;

    [field: SerializeField] public GameObject ActiveItemObject { get; private set; } = null;
    [field: SerializeField] public ItemData ActiveItemData { get; private set; } = null;

    private void Start()
    {
        RegisterCallbacks();
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        InventoryManager.Instance.OnHotbarItemSelectedChanged += HandleItemSelect;
    }

    private void UnregisterCallbacks()
    {
        InventoryManager.Instance.OnHotbarItemSelectedChanged -= HandleItemSelect;
    }

    public void HandleItemSelect(InventorySlot slot)
    {
        if (!slot.selected)
            return;
;
        if (slot.currentItemData == null || !(slot.currentItemData is WieldableItemData))
        {
            UnEquipItem();
            return;
        }

        EquipItem((WieldableItemData)slot.currentItemData);
    }

    public void EquipItem(WieldableItemData itemData)
    {
        UnEquipItem();
        GameObject itemHeldPrefab = itemData.itemHeldPrefab;
        GameObject spawnedItemHeld = Instantiate(itemHeldPrefab, itemContainerR);

        spawnedItemHeld.transform.position = itemContainerR.position;
        spawnedItemHeld.transform.rotation = itemContainerR.rotation * itemHeldPrefab.transform.rotation;

        ActiveItemObject = spawnedItemHeld;
        ActiveItemData = itemData.CloneItem();
    }

    public void UnEquipItem()
    {
        if (ActiveItemObject == null)
            return;

        Destroy(ActiveItemObject);
        ActiveItemObject = null;
        ActiveItemData = null;
    }

    public AnimationClip GetAttackAnimationFromActiveItem()
    {
        if (ActiveItemData == null)
            return itemTypeToAnimationClipDict[ItemType.None];

        if (ActiveItemData is WieldableItemData)
            return itemTypeToAnimationClipDict[ActiveItemData.itemType];

        return itemTypeToAnimationClipDict[ItemType.None];
    }

    public void SpawnSlashEffect()
    {
        ParticleSystem ps = Instantiate(weaponSlash, ActiveItemObject.transform);
        if (ActiveItemData is WieldableItemData wieldableItemData)
        {
            AudioManager.PlaySound(wieldableItemData.swingSound);
        }
    }
}
