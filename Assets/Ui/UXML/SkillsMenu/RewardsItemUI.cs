using UnityEngine.UIElements;
using UnityEngine;
using RewardSystem;

namespace GameUI
{
    public partial class RewardsItemUI
    {   
        public int ContainerIndex { get; set; }
        public VisualElement Root {get; set;}
        public BaseRewardData RewardData { get; set; }
    
        public static string _rowItemEven = "row-item-even";
        public static string _rowItemOdd = "row-item-odd";
        private static string _rowItemLast = "row-item-last";
        private static string _rowItemLabelLast = "row-item-label-last";
    
        public RewardsItemUI(VisualElement root, int index, BaseRewardData baseRewardData)
        {
            Root = root;
            ContainerIndex = index;
            RewardData = baseRewardData;

            AssignQueryResults(root);
            Init();
        }

        private void Init()
        {
            UpdateMaterialContainerSlots();
            UpdateRewardsData();
        }

        public void UpdateRewardsData()
        {
            switch (RewardData)
            {
                case ItemRewardData itemReward:
                    quantityLabel.text = itemReward.Quantity.ToString();
                    rewardLabel.text = itemReward.Item.displayName;
                    rewardIcon.style.backgroundImage = UiManager.Instance.uiTextureLibrary.itemRewardIcon;
                    break;
            
                case StatBonusRewardData statReward:
                    quantityLabel.text = statReward.BonusValue.ToString("F1");
                    rewardLabel.text = statReward.StatType.GetDescription();
                    rewardIcon.style.backgroundImage = UiManager.Instance.uiTextureLibrary.statBoostRewardIcon;
                    break;
            
                case CurrencyRewardData currencyReward:
                    quantityLabel.text = currencyReward.Quantity.ToString();
                    rewardLabel.text = GameDataManager.Instance.gameData.currencyName;
                    rewardIcon.style.backgroundImage = UiManager.Instance.uiTextureLibrary.currentRewardIcon;
                    break;
            
                default:
                    Debug.LogWarning($"Unknown reward type: {RewardData.GetType()}");
                    break;
            }
        }
    
        public void UpdateMaterialContainerSlots()
        {
            if (ContainerIndex % 2 == 0)
                return;

            rewardsContainer.RemoveFromClassList(_rowItemEven);
            rewardsContainer.AddToClassList(_rowItemOdd);
        
            Color currentColor = quantityLabel.resolvedStyle.backgroundColor;
            currentColor.a = 0.65f;
            quantityLabel.style.backgroundColor = new StyleColor(currentColor); 
        }

        public void SetLastItemStyle()
        {
            rewardsContainer.AddToClassList(_rowItemLast);
            rewardLabel.AddToClassList(_rowItemLast);
        }
    
    }    
}

