// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace GameUI
{
    partial class RewardsItemUI
    {
        private VisualElement rewardsContainer;
        private VisualElement rewardIcon;
        private Label rewardLabel;
        private Label quantityLabel;
    
        protected void AssignQueryResults(VisualElement root)
        {
            rewardsContainer = root.Q<VisualElement>("RewardsContainer");
            rewardIcon = root.Q<VisualElement>("RewardIcon");
            rewardLabel = root.Q<Label>("RewardLabel");
            quantityLabel = root.Q<Label>("QuantityLabel");
        }
    }
}
