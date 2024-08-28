using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using static ItemData;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "ItemData", menuName = "OneFire/Items/ItemData")]
public class ItemData : SerializedScriptableObject
{
    [Header("Details")]
    public string itemID = Guid.NewGuid().ToString();
    public string baseName;
    public string displayName;
    public string description;
    public string interactDescription;

    public int stackCount;
    public int maxStackCount = 50;
    public int baseSellValue = 1;
    
    [Header("Classification")]
    public ItemType itemType = ItemType.None;
    public ItemRarity itemRarity = ItemRarity.None;
    public ItemCategory itemCategories = ItemCategory.None;

    [Header("Assets")]
    public GameObject item3DPrefab;
    public Sprite itemSprite;
    public GameObject itemHeldPrefab;

    [Header("Stats")]
    public List<IGenericItemStat> itemStatList;
    [HideInInspector] public ItemStats itemStats;

    private void OnValidate()
    {
        ItemRegistry.Register(this);

#if UNITY_EDITOR
        if (itemID == null || itemID == String.Empty)
            itemID = Guid.NewGuid().ToString();

        if (!PrefabUtility.IsPartOfPrefabAsset(item3DPrefab))
            Debug.LogError($"{this} {item3DPrefab} is a GameObject - set to prefab!");

        if (!PrefabUtility.IsPartOfPrefabAsset(itemHeldPrefab))
            Debug.LogError($"{this} {itemHeldPrefab} is a GameObject - set to prefab!");

        if (itemHeldPrefab.GetComponent<WorldItem>() != null && itemCategories.HasFlag(ItemCategory.Wieldable))
            Debug.LogError($"{this} {itemHeldPrefab} is a WorldItem and wieldable - remove component!");

        if (item3DPrefab.GetComponent<WorldItem>() == null)
            Debug.LogError($"{this} {item3DPrefab} does not contain a WorldItem component!");

        if (string.IsNullOrEmpty(itemID) || itemID == Guid.Empty.ToString())
            itemID = Guid.NewGuid().ToString();
#endif
    }

    private void OnEnable()
    {
        itemStats = new();
        foreach (IGenericItemStat genericItemStat in itemStatList)
        {
            itemStats.SetStat(genericItemStat.ItemStat, genericItemStat.GetValue());
        }
    }

    public void SetItemDataToBaseItemData(BaseItemData baseItemData)
    {
        itemID = baseItemData.itemID;
        stackCount = baseItemData.stackCount;
    }

    public interface IGenericItemStat
    {
        ItemStat ItemStat { get; }
        object GetValue();
    }

    [Serializable]
    public class IntValue : IGenericItemStat
    {
        public ItemStat itemStat;
        public int value;

        public ItemStat ItemStat => itemStat;
        public object GetValue() => value;
    }

    [Serializable]
    public class FloatValue : IGenericItemStat
    {
        public ItemStat itemStat;
        public float value;

        public ItemStat ItemStat => itemStat;
        public object GetValue() => value;
    }

    public enum ItemRarity
    {
        None,
        Common,
        Uncommon,
        Rare,
        Special,
        Epic,
        Legendary,
        Mythic
    }

    public enum ItemType
    {
        None = 0,
        // Misc
        Food = 1,
        Potion = 2,
        // Outfit
        Boots = 3,
        Gloves = 4,
        Head = 5,
        Body = 6,
        Legs = 7,
        Cape = 8,
        // Structures
        CraftingStation = 9,
        Chest = 10,
        // Tools
        Axe = 11,
        Pickaxe = 12,
    }

    [Flags]
    public enum ItemCategory
    {
        None = 0,
        Wieldable = 1,
        Fishing = 2,
        Consumable = 4,
        Weapon = 8,
        Outfit = 16,
        Accessory = 32,
        Tackle = 64,
        Structure = 128,
        Tool = 256,
    }
}

public class BaseItemData
{
    public string itemID;
    public int stackCount;

    public BaseItemData(ItemData itemData)
    {
        itemID = itemData.itemID;
        stackCount = itemData.stackCount;
    }
}

public static class ItemExtensions
{
    private static readonly Dictionary<string, ItemData> _items = new Dictionary<string, ItemData>();
    public static ItemData GetItemData(string uniqueID)
    {
        if (_items.Count <= 0)
        {
            var items = Resources.LoadAll<ItemData>("ScriptableObjects/Items");
            foreach ( var item in items )
            {
                _items.Add(item.itemID, item);
            }
        }

        if (_items.TryGetValue(uniqueID, out ItemData itemData))
        {
            return itemData;
        }

        Debug.LogWarning($"Item not found: {uniqueID}");
        return null;
    }

    public static ItemData GetItemDataInstantiated(this ItemData itemData)
    {
        ItemData originalItemData = GetItemData(itemData.itemID);
        if (originalItemData == null)
            return null;

        ItemData spawnedItemData = ScriptableObject.Instantiate(originalItemData);

        return spawnedItemData;
    }

    public static ItemData CloneItemData(this ItemData data)
    {
        ItemData spawnedData = ScriptableObject.Instantiate(data);
        return spawnedData;
    }

    public static StructureData GetStructureFromItem(this ItemData itemData)
    {
        foreach (var structureKVP in StructureRegistry.structureDictionary)
        {
            if (itemData.itemID == structureKVP.Value.itemDataAsset.itemID)
            {
                return structureKVP.Value;
            }
        }

        Debug.LogError($"Unable to find {itemData} in stuctures; is there a mismatch in registries?");
        return null;
    }
}