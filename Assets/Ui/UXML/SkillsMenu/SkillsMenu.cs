using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;

namespace GameUI
{
    public partial class SkillsMenu : ITabMenu, IPersistentData
    {   
        public List<LevelNodeContainer> LevelNodes = new List<LevelNodeContainer>();
        public VisualElement Root;
        public int TabIndex { get; }
        public SkillType ActiveSkillType = SkillType.Fishing;

        private List<QuickEye.UIToolkit.Tab> _skillsTabs = new();
        private int _pathGoesRightInterval = 5;
        private int _lastLevel;
        private int _activeSkillTabIndex;

        private static string levelContainerEven = "level-container-even";
        private static string levelContainerOdd = "level-container-odd";
        
        private Dictionary<SkillType, bool[]> _skillRewardsCollectedDict = new ();
        private Dictionary<SkillType, int> _skillToTabIndexDict = new ();
        private Dictionary<SkillType, string> _collectRewardsStyleDict = new ();
        public event Action OnSkillsMenuHide;
        public event Action OnSkillsMenuShow;
        
        public SkillsMenu(VisualElement root, int tabIndex)
        {
            Root = root;
            TabIndex = tabIndex;
            _lastLevel = SkillManager.Instance.skillData.skillCap;
            
            AssignQueryResults(root);
            LoadData();
            RegisterCallbacks();
            Init();
        }

        private void Init()
        {
            levelNodeContainer.Clear();

            InitDicts();
            InitRewardsButton();
            UpdateLevelLabels();
            InitLevelNodes();
            InitTabs();
        }

        private void RegisterCallbacks()
        {
            SkillManager.Instance.OnGainedAnyLevel += LevelGained;
            SkillManager.Instance.OnGainedAnyXp += XpGained;
        }

        private void UnregisterCallbacks()
        {
            SkillManager.Instance.OnGainedAnyLevel -= LevelGained;
            SkillManager.Instance.OnGainedAnyXp -= XpGained;
        }

        private void InitTabs()
        {
            int i = 0;
            foreach (QuickEye.UIToolkit.Tab tab in skillsTabGroup.contentContainer.Children())
            {
                _skillsTabs.Add(tab);
                tab.RegisterValueChangedCallback(evt => OnSkillTabChanged(tab.tabIndex));

                // Automatically map SkillType enum values to tabs based on order
                if (i < Enum.GetValues(typeof(SkillType)).Length)
                {
                    SkillType skillType = (SkillType)Enum.GetValues(typeof(SkillType)).GetValue(i);
                    _skillToTabIndexDict[skillType] = i;
                }

                i++;
            }
        }
        
        void OnSkillTabChanged(int tabIndex)
        {
            _activeSkillTabIndex = tabIndex;
            ActiveSkillType = (SkillType)Enum.GetValues(typeof(SkillType)).GetValue(_activeSkillTabIndex);

            UpdateLevelNodes();
            UpdateLevelLabels();
            UpdateRewardsButton();
            UpdateRewardsButtonStyle();
            UpdateRewardsAvailable();
        }

        private void InitDicts()
        {
            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                _skillRewardsCollectedDict[skillType] = new bool[SkillManager.Instance.skillData.skillCap];
            }

            _collectRewardsStyleDict[SkillType.Fishing] = "collect-rewards-button-fishing";
            _collectRewardsStyleDict[SkillType.Forestry] = "collect-rewards-button-forestry";
            _collectRewardsStyleDict[SkillType.Mining] = "collect-rewards-button-mining";
            _collectRewardsStyleDict[SkillType.Combat] = "collect-rewards-button-combat";
        }

        private void InitRewardsButton()
        {
            collectRewardsButton.clickable.clicked += CollectRewards;
            
            UpdateRewardsButton();
            UpdateRewardsButtonStyle();
        }
        
        private void UpdateRewardsButton()
        {
            collectRewardsButton.SetEnabled(RewardsAvailable());
        }

        private void UpdateRewardsButtonStyle()
        {
            foreach (var styleClass in _collectRewardsStyleDict.Values)
            {
                collectRewardsButton.RemoveFromClassList(styleClass);
            }

            collectRewardsButton.AddToClassList(_collectRewardsStyleDict[ActiveSkillType]);
        }

        private void UpdateRewardsAvailable()
        {
            foreach (var levelNode in LevelNodes)
            {
                bool rewardsAvailable = levelNode.CheckRewardsAvailable();
                if (!rewardsAvailable)
                    break;
            }
        }

        private bool RewardsAvailable()
        {
            int currentRewardsIndex = GetHighestCollectedIndex();
            return SkillManager.Instance.Skills[ActiveSkillType].Level > currentRewardsIndex;
        }

        private void UpdateLevelNodes()
        {
            for (int i = 0; i < LevelNodes.Count; i++)
            {
                LevelNodeContainer levelNode = LevelNodes[i];
                levelNode.Cleanup();
            }
            
            LevelNodes.Clear();
            levelNodeContainer.Clear();
            
            InitLevelNodes();
        }

        private void InitLevelNodes()
        {
            VisualElement activeColumnContainer = null;
            for (int i = 0; i < SkillManager.Instance.skillData.skillCap; i++)
            {
                VisualElement levelNodeElement = UiManager.Instance.levelNodeContainer.CloneTree();
                LevelNodeContainer levelNodeContainer = new LevelNodeContainer(levelNodeElement, i, this, SkillManager.Instance.skillRewardDataDict[ActiveSkillType].GetRewardForLevel(i+1));
                
                // Apply styles based on node index
                if (i % (_pathGoesRightInterval * 2) == 0)
                    activeColumnContainer = CreateColumnNodeContainer(levelContainerEven);
                else if (i % (_pathGoesRightInterval * 2) == 5)
                    activeColumnContainer = CreateColumnNodeContainer(levelContainerOdd);
                
                // Apply style when path turns
                if ((i + 1) % _pathGoesRightInterval == 0)
                    levelNodeContainer.ApplyPathRightStyle();
                
                // Hide path on last element
                if (i + 1 == SkillManager.Instance.skillData.skillCap)
                    levelNodeContainer.HideLevelpath();
                
                // Reverse node and path if on odd column, going up
                if (i % (_pathGoesRightInterval * 2) >= 5 && i % (_pathGoesRightInterval * 2) < 9)
                    levelNodeContainer.ReversePathAndLevelNode();

                activeColumnContainer.Add(levelNodeElement);
                LevelNodes.Add(levelNodeContainer);
            }
        }
        
        private void CollectRewards()
        {
            int currentRewardsIndex = GetHighestCollectedIndex();
            while (SkillManager.Instance.Skills[ActiveSkillType].Level > currentRewardsIndex)
            {
                RewardSystem.LevelReward rewardData = SkillManager.Instance.skillRewardDataDict[ActiveSkillType].GetRewardForLevel(currentRewardsIndex + 1); 
                GameObjectManager.Instance.playerStats.HandleRewardsData(rewardData.Rewards);
                _skillRewardsCollectedDict[ActiveSkillType][currentRewardsIndex] = true;
                LevelNodes[currentRewardsIndex].CheckRewardsAvailable();
                currentRewardsIndex++;
            }
            
            UpdateRewardsButton();
        }
        
        public int GetHighestCollectedIndex()
        {
            if (!_skillRewardsCollectedDict.TryGetValue(ActiveSkillType, out var list) || list == null || list.Length == 0)
                return 0;

            for (int i = 0; i < list.Length; i++)
            {
                if (!list[i])
                    return i;
            }

            return 0;
        }

        private VisualElement CreateColumnNodeContainer(string className)
        {
            VisualElement columnContainer = new VisualElement();
            columnContainer.AddToClassList(className);
            levelNodeContainer.Add(columnContainer);

            return columnContainer;
        }
        
        private void LevelGained(SkillType skillType, int level)
        {
            if (skillType != ActiveSkillType)
                return;

            UpdateRewardsButton();
            UpdateLevelNode(level);
        }
        
        private void XpGained(SkillType skillType, float xp)
        {
            if (skillType != ActiveSkillType)
                return;
            
            UpdateLevelLabels();
        }

        public int GetActiveSkillLevel()
        {
            BaseSkill activeSkill = SkillManager.Instance.Skills[ActiveSkillType];
            return activeSkill.Level;
        }

        private void UpdateLevelNode(int level)
        {
            int index = level - 1;
            if (index <= 0)
                return;
            
            LevelNodes[index - 1].SetLevelPathState(true);
            LevelNodes[index].SetLevelNodeState(true);
        }

        private void UpdateLevelLabels()
        {
            BaseSkill activeSkill = SkillManager.Instance.Skills[ActiveSkillType];
            float targetLevelExp = SkillManager.Instance.skillData.levelXpTable[activeSkill.Level];
            
            activeSkillIcon.style.backgroundImage = UiManager.Instance.uiTextureLibrary.skillIcons[activeSkill.SkillType].texture;
            
            activeSkillLabel.text = activeSkill.SkillType.ToString();
            activeSkillLabel.style.color = UiManager.Instance.uiColorData.skillTextColors[activeSkill.SkillType];
            activeLevelLabel.text = "Level " + activeSkill.Level;
            
            skillExpValueLabel.text = activeSkill.CurrentLevelXp.ToString("F0") + "/" + targetLevelExp.ToString("F0");
            activeSkillXpBar.value = (activeSkill.CurrentLevelXp / targetLevelExp) * 100;
        }
        
        public void ShowMenu()
        {
            Root.style.display = DisplayStyle.Flex;
            OnSkillsMenuShow?.Invoke();
        }
        
        public void HideMenu()
        {
            Root.style.display = DisplayStyle.None;
            OnSkillsMenuHide?.Invoke();
        }
        
        public void OnTabChanged(int newIndex)
        {
            if (TabIndex == newIndex)
                ShowMenu();
            else
                HideMenu();
        }
        
        public void AddToSaveable()
        {
            PersistentDataManager.Instance.AddToPersistentDataList(this);
        }

        public void LoadData()
        {
            ES3.Load(nameof(_skillRewardsCollectedDict), defaultValue: new Dictionary<SkillType, List<bool>>());
        }

        public void SaveData()
        {
            ES3.Save(nameof(_skillRewardsCollectedDict), _skillRewardsCollectedDict);
        }
    }    
}

