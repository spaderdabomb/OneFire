using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using RewardSystem;

public class PlayerStats : MonoBehaviour
{
    [Header("Fishing Stats")]
    [SerializeField] private Stat fishingSpeed = new Stat(1);
    [SerializeField] private Stat fishingPower = new Stat(1);
    [SerializeField] private Stat fishingCreatureSpawnChance = new Stat(0);
    [SerializeField] private Stat fishingRarityLuck = new Stat(0);
    [SerializeField] private Stat fishingLootChance = new Stat(0);

    [Header("Skill Stats")]
    [SerializeField] private Stat forestrySpeed = new Stat(1);
    [SerializeField] private Stat miningSpeed = new Stat(1);

    [Header("Combat Stats")]
    [SerializeField] private Stat health = new Stat(100);
    [SerializeField] private Stat damage = new Stat(35);
    [SerializeField] private Stat defense = new Stat(5);
    [SerializeField] private Stat armor = new Stat(0);
    [SerializeField] private Stat speed = new Stat(1);
    [SerializeField] private Stat criticalChance = new Stat(0);
    [SerializeField] private Stat criticalDamage = new Stat(0);

    [Header("Global Stats")]
    [SerializeField] private Stat experienceMultiplier = new Stat(1);
    [SerializeField] private Stat magicFind = new Stat(0);
    
    private Dictionary<StatType, Stat> statLookup;
    
    // Events for UI updates
    public event System.Action<StatType, float> OnStatChanged;

    private PlayerEquippedItem _playerEquippedItem;

    [SerializeField] private float axeDamageOnTreeMultiplier = 2.5f;
    [SerializeField] private float pickaxeDamageOnVeinMultiplier = 2.5f;

    private void Awake()
    {
        InitializeStatLookup();
        _playerEquippedItem = GetComponent<PlayerEquippedItem>();
    }
    
    private void InitializeStatLookup()
    {
        statLookup = new Dictionary<StatType, Stat>
        {
            { StatType.FishingSpeed, fishingSpeed },
            { StatType.FishingPower, fishingPower },
            { StatType.FishingCreatureSpawnChance, fishingCreatureSpawnChance },
            { StatType.FishingRarityLuck, fishingRarityLuck },
            { StatType.FishingLootChance, fishingLootChance },
            { StatType.ForestrySpeed, forestrySpeed },
            { StatType.MiningSpeed, miningSpeed },
            { StatType.Health, health },
            { StatType.Damage, damage },
            { StatType.Defense, defense },
            { StatType.Armor, armor },
            { StatType.Speed, speed },
            { StatType.CriticalChance, criticalChance },
            { StatType.CriticalDamage, criticalDamage },
            { StatType.ExperienceMultiplier, experienceMultiplier },
            { StatType.MagicFind, magicFind },
        };
    }
    
    public float GetStatValue(StatType statType)
    {
        return statLookup.TryGetValue(statType, out var stat) ? stat.CurrentValue : 0f;
    }
    
    public void AddStatModifier(StatType statType, StatModifier modifier)
    {
        if (statLookup.TryGetValue(statType, out var stat))
        {
            stat.AddModifier(modifier);
            OnStatChanged?.Invoke(statType, stat.CurrentValue);
        }
    }
    
    public void RemoveStatModifier(StatType statType, StatModifier modifier)
    {
        if (statLookup.TryGetValue(statType, out var stat))
        {
            stat.RemoveModifier(modifier);
            OnStatChanged?.Invoke(statType, stat.CurrentValue);
        }
    }
    
    public void IncreaseBaseStat(StatType statType, float amount)
    {
        if (statLookup.TryGetValue(statType, out var stat))
        {
            stat.SetBaseValue(stat.BaseValue + amount);
            OnStatChanged?.Invoke(statType, stat.CurrentValue);
        }
    }

    public void HandleRewardsData(List<BaseRewardData> rewardData)
    {
        foreach (BaseRewardData reward in rewardData)
        {
            switch (reward)
            {
                case ItemRewardData itemReward:
                    Debug.Log($"Item reward: {itemReward.Item}");
                    // Handle item-specific logic
                    break;
            
                case StatBonusRewardData statReward:
                    Debug.Log($"Stat bonus: {statReward.StatType} +{statReward.BonusValue}");
                    IncreaseBaseStat(statReward.StatType, statReward.BonusValue);
                    break;
            
                case RecipeRewardData recipeReward:
                    Debug.Log($"Recipe unlock: {recipeReward.RecipeId}");
                    // Handle recipe-specific logic
                    break;
            
                case CurrencyRewardData currencyReward:
                    Debug.Log($"Currency: +{currencyReward.Amount}");
                    // Handle currency-specific logic
                    break;
            
                default:
                    Debug.LogWarning($"Unknown reward type: {reward.GetType()}");
                    break;
            }
        }
        
    }

    public float CalculateDamage(GameObject targetObject)
    {
        float objectDamage = damage.CurrentValue;

        bool weaponIsNull = _playerEquippedItem.ActiveItemData != null;
        if (targetObject.GetComponent<WorldTree>() != null)
        {
            if (weaponIsNull && _playerEquippedItem.ActiveItemData.itemType == ItemType.Axe)
            {
                objectDamage *= axeDamageOnTreeMultiplier;
            } 
        }
        else if (targetObject.GetComponent<WorldVein>() != null)
        {
            if (weaponIsNull && _playerEquippedItem.ActiveItemData.itemType == ItemType.Pickaxe)
            {
                objectDamage *= pickaxeDamageOnVeinMultiplier;
            }
            else
            {
                objectDamage = 0f;
            }
        }

        return objectDamage;
    }

    public float GetFishingPower()
    {
        return fishingPower.CurrentValue;
    }
}

public enum StatType
{
    [Description("None")] None = 0,
    [Description("Fishing Speed")] FishingSpeed = 1,
    [Description("Fishing Power")] FishingPower = 2,
    [Description("Creature Spawn Chance")] FishingCreatureSpawnChance = 3,
    [Description("Fishing Luck")] FishingRarityLuck = 4,
    [Description("Fishing Loot Chance")] FishingLootChance = 5,
    [Description("Forestry Speed")] ForestrySpeed = 6,
    [Description("Mining Speed")] MiningSpeed = 7,
    [Description("Health")] Health = 8,
    [Description("Damage")] Damage = 9, 
    [Description("Defense")] Defense = 10, 
    [Description("Armor")] Armor = 11,
    [Description("Speed")] Speed = 12,
    [Description("Critical Chance")] CriticalChance = 13, 
    [Description("Critical Damage")] CriticalDamage = 14,
    [Description("Experience Multiplier")] ExperienceMultiplier = 15,
    [Description("Magic Find")] MagicFind = 16,
}
