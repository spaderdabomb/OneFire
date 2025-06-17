// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace GameUI
{
    partial class SkillsMenu
    {
        private VisualElement skillsMenuRoot;
        private Label menuHeaderLabel;
        private QuickEye.UIToolkit.TabGroup skillsTabGroup;
        private QuickEye.UIToolkit.Tab fishingTab;
        private QuickEye.UIToolkit.Tab forestryTab;
        private QuickEye.UIToolkit.Tab miningTab;
        private QuickEye.UIToolkit.Tab combat;
        private VisualElement horizontalSkillContainer;
        private VisualElement leftSkillContainer;
        private VisualElement leftTopContainer;
        private VisualElement activeSkillIcon;
        private Label activeLevelLabel;
        private Label activeSkillLabel;
        private Button collectRewardsButton;
        private VisualElement leftBottomContainer;
        private Label skillExpLabel;
        private ProgressBar activeSkillXpBar;
        private Label skillExpValueLabel;
        private VisualElement rightSkillContainer;
        private VisualElement levelNodeContainer;
    
        protected void AssignQueryResults(VisualElement root)
        {
            skillsMenuRoot = root.Q<VisualElement>("SkillsMenuRoot");
            menuHeaderLabel = root.Q<Label>("MenuHeaderLabel");
            skillsTabGroup = root.Q<QuickEye.UIToolkit.TabGroup>("SkillsTabGroup");
            fishingTab = root.Q<QuickEye.UIToolkit.Tab>("FishingTab");
            forestryTab = root.Q<QuickEye.UIToolkit.Tab>("ForestryTab");
            miningTab = root.Q<QuickEye.UIToolkit.Tab>("MiningTab");
            combat = root.Q<QuickEye.UIToolkit.Tab>("Combat");
            horizontalSkillContainer = root.Q<VisualElement>("HorizontalSkillContainer");
            leftSkillContainer = root.Q<VisualElement>("LeftSkillContainer");
            leftTopContainer = root.Q<VisualElement>("LeftTopContainer");
            activeSkillIcon = root.Q<VisualElement>("ActiveSkillIcon");
            activeLevelLabel = root.Q<Label>("ActiveLevelLabel");
            activeSkillLabel = root.Q<Label>("ActiveSkillLabel");
            collectRewardsButton = root.Q<Button>("CollectRewardsButton");
            leftBottomContainer = root.Q<VisualElement>("LeftBottomContainer");
            skillExpLabel = root.Q<Label>("SkillExpLabel");
            activeSkillXpBar = root.Q<ProgressBar>("ActiveSkillXpBar");
            skillExpValueLabel = root.Q<Label>("SkillExpValueLabel");
            rightSkillContainer = root.Q<VisualElement>("RightSkillContainer");
            levelNodeContainer = root.Q<VisualElement>("LevelNodeContainer");
        }
    }
}
