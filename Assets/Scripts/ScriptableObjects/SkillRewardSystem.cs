using System.Collections.Generic;
using UnityEngine;

namespace RewardSystem
{
    [CreateAssetMenu(fileName = "SkillRewardData", menuName = "Scriptable Objects/SkillRewardData")]
    public class SkillRewardData : ScriptableObject
    {
        [SerializeField] private SkillType skillType;
        [SerializeField] private LevelReward[] levelRewards;
        
        public LevelReward GetRewardForLevel(int level)
        {
            if (level < 1 || level > levelRewards.Length) return null;
            return levelRewards[level - 1];
        }
    }

    [System.Serializable]
    public class LevelReward
    {
        [SerializeField] private int level;
        [SerializeReference] private List<BaseRewardData> rewards = new List<BaseRewardData>();
        [SerializeField] private string unlockMessage;
        
        public List<BaseRewardData> Rewards => rewards;
        public string UnlockMessage => unlockMessage;
        public int Level => level;
    }

    [System.Serializable]
    public class RewardItem
    {
        [SerializeField] private RewardType type;
        [SerializeField] private string itemId; // For items/recipes
        [SerializeField] private int quantity;
        [SerializeField] private float multiplier; // For stat bonuses
    }

    public enum RewardType
    {
        Item,
        StatBonus,
        Recipe,
        Currency
    }

    [System.Serializable]
    public abstract class BaseRewardData
    {
        [SerializeField] protected RewardType type;
        [SerializeField] protected int quantity = 1;
        
        public RewardType Type => type;
        public int Quantity => quantity;
        
        public abstract void Grant();
        public abstract string GetDescription();
    }

    [System.Serializable]
    public class ItemRewardData : BaseRewardData
    {
        [SerializeField] private ItemData item;
        
        public ItemData Item => item; //
        
        public override void Grant()
        {
            //player.Inventory.AddItem(itemId, quantity);
        }
        
        public override string GetDescription()
        {
            return $"Receive {quantity}x {item}";
        }
    }

    [System.Serializable]
    public class StatBonusRewardData : BaseRewardData
    {
        [SerializeField] private StatType statType;
        [SerializeField] private float bonusValue;
        [SerializeField] private bool isPermanent = true;
        
        public StatType StatType => statType;
        public float BonusValue => bonusValue;
        public bool IsPermanent => isPermanent;
        
        public override void Grant()
        {
            //player.Stats.AddBonus(statType, bonusValue, isPermanent);
        }
        
        public override string GetDescription()
        {
            return $"+{bonusValue} {statType}";
        }
    }

    [System.Serializable]
    public class RecipeRewardData : BaseRewardData
    {
        [SerializeField] private string recipeId;
        
        public string RecipeId => recipeId;
        
        public override void Grant()
        {
            //player.RecipeBook.UnlockRecipe(recipeId);
        }
        
        public override string GetDescription()
        {
            return $"Unlock recipe: {recipeId}";
        }
    }

    [System.Serializable]
    public class CurrencyRewardData : BaseRewardData
    {
        [SerializeField] private int amount;
        
        public int Amount => amount;
        
        public override void Grant()
        {
            // player.Currency.AddCurrency(amount);
        }
        
        public override string GetDescription()
        {
            return $"+{amount}";
        }
    }   
}