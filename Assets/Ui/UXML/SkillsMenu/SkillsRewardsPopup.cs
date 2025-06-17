using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using RewardSystem;
using UnityEngine.InputSystem;

namespace GameUI
{
    public partial class SkillsRewardsPopup
    {       
        public VisualElement Root {get; set;}
        
        private List<RewardsItemUI> _rewardItems = new();
        private VisualElement _parentElement;
        private LevelReward _levelReward;
        private int _parentIndex;

        private Vector2 _popupOffset = new Vector2(5f, -5f);

        public SkillsRewardsPopup(VisualElement root, LevelReward levelReward, VisualElement parentElement, int parentIndex)
        {
            Root = root;
            _levelReward = levelReward;
            _parentElement = parentElement;
            _parentIndex = parentIndex;
        
            AssignQueryResults(root);
            RegisterCallbacks();
            Init();
        }

        private void Init()
        {
            Root.style.position = Position.Absolute;
            _parentElement.Add(Root);
            UiToolkitUtils.SetPositionToMouse(Root, _popupOffset);
            headerLabel.text = "Level " + (_parentIndex + 1);
            
            for (int i = 0; i < _levelReward.Rewards.Count; i++)
            {
                BaseRewardData rewardData = _levelReward.Rewards[i];
                VisualElement rewardsItemUI = UiManager.Instance.rewardsItemUI.CloneTree();
                rewardsItemUI.pickingMode = PickingMode.Ignore;
                RewardsItemUI rewardsUI = new RewardsItemUI(rewardsItemUI, i, rewardData);

                if (i == _levelReward.Rewards.Count - 1)
                    rewardsUI.SetLastItemStyle();
                
                _rewardItems.Add(rewardsUI);
                vlayout.Add(rewardsItemUI);
            }
        }
    
        private void RegisterCallbacks()
        {
            UiManager.Instance.uiGameManager.OptionsMenuUi.OnPointerMoveOptions += OnPointerMove;
        }

        public void RemovePopup()
        {
            UiManager.Instance.uiGameManager.OptionsMenuUi.OnPointerMoveOptions -= OnPointerMove;
            Root.RemoveFromHierarchy();
        }
    
        private void OnPointerMove(PointerMoveEvent evt)
        {
            UiToolkitUtils.SetPositionToMouse(Root, _popupOffset);
        }
    }    
}

