using OneFireUi;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquippedItem : SerializedMonoBehaviour
{
    [SerializeField] private Transform armature;
    [SerializeField] private Transform itemContainerR;
    [SerializeField] private Dictionary<ItemData.ItemType, AnimationClip> itemTypeToAnimationClipDict = new();
    public GameObject ActiveItemObject { get; private set; } = null;
    public ItemData ActiveItemData { get; private set; } = null;

    private void OnEnable()
    {
        InventoryManager.Instance.OnHotbarItemSelectedChanged += HandleItemSelect;
    }

    private void OnDisable()
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
        ActiveItemData = itemData.CloneItemData();
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
            return itemTypeToAnimationClipDict[ItemData.ItemType.None];

        if (ActiveItemData.itemCategories.HasFlag(ItemData.ItemCategory.Wieldable))
            return itemTypeToAnimationClipDict[ActiveItemData.itemType];

        return itemTypeToAnimationClipDict[ItemData.ItemType.None];
    }
}
