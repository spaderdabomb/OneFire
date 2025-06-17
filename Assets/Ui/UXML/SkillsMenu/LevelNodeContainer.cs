using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using GameUI;
using RewardSystem;

public partial class LevelNodeContainer
{   
    public VisualElement Root;
    public int LevelIndex;
    public LevelReward LevelReward;

    public static string levelNodeContainerRight = "level-node-container-right";
    public static string levelNodeContainerStandard = "level-node-container-standard";
    public static string levelPathRight = "level-path-right";
    public static string levelPathStandard = "level-path-standard";

    private SkillsRewardsPopup _activeRewardsPopup = null;
    private IVisualElementScheduledItem _scheduledUpdate;
    private IVisualElementScheduledItem _scaleAnimation;
    private SkillsMenu _skillsMenu;
    private float _hoverTime = 0.5f;
    private float _startTime;
    private float _elapsedDuration;
    
    // Animation properties
    private float _animationStartTime;
    private float _animationSpeed = 7f; // Controls how fast the scale oscillates
    private float _minScale = 1f;
    private float _maxScale = 1.3f;
    
    public LevelNodeContainer(VisualElement root, int levelIndex, SkillsMenu skillsMenu, LevelReward levelReward)
    {
        Root = root;
        LevelIndex = levelIndex;
        LevelReward = levelReward;
        _skillsMenu = skillsMenu;
        
        AssignQueryResults(root);
        RegisterCallbacks();
        Init();
    }

    private void Init()
    {
        int activeSkillevel = _skillsMenu.GetActiveSkillLevel();
        levelNodeLabel.text = (LevelIndex + 1).ToString();

        if (LevelIndex + 1 > activeSkillevel - 1)
            SetLevelPathState(false);

        if (LevelIndex + 1 > activeSkillevel)
            SetLevelNodeState(false);
    }

    private void RegisterCallbacks()
    {
        Root.RegisterCallback<PointerEnterEvent>(PointerEnter);
        Root.RegisterCallback<PointerLeaveEvent>(PointerLeave);
        _skillsMenu.OnSkillsMenuHide += HideMenu;
        _skillsMenu.OnSkillsMenuShow += ShowMenu;
        SkillManager.Instance.SubscribeToLevel(_skillsMenu.ActiveSkillType, LevelUp);
    }
    

    private void UnRegisterCallbacks()
    {
        Root.UnregisterCallback<PointerEnterEvent>(PointerEnter);
        Root.UnregisterCallback<PointerLeaveEvent>(PointerLeave);
        _skillsMenu.OnSkillsMenuHide -= HideMenu;
        _skillsMenu.OnSkillsMenuShow -= ShowMenu;
        SkillManager.Instance.UnSubscribeToLevel(_skillsMenu.ActiveSkillType, LevelUp);
    }
    
    #region Scale Animation
    private void StartScaleAnimation()
    {
        StopScaleAnimation();
        _animationStartTime = Time.time;
        _scaleAnimation = levelNode.schedule.Execute(UpdateScale).Every(10);
    }
    
    public void StopScaleAnimation()
    {
        _scaleAnimation?.Pause();
        levelNode.transform.scale = Vector3.one;
    }
    
    private void UpdateScale()
    {
        float elapsedTime = Time.time - _animationStartTime;
        
        float sineValue = Mathf.Sin(elapsedTime * _animationSpeed);
        float scale = Mathf.Lerp(_minScale, _maxScale, (sineValue + 1f) / 2f);
        levelNode.transform.scale = new Vector3(scale, scale, 1f);
    }
    
    #endregion
    
    #region Hover Popup
    
    private void StartScheduledUpdate()
    {
        _scheduledUpdate = Root.schedule.Execute(UpdateHoverTime).Every(100);
    }
    
    public void StopUpdates()
    {
        _scheduledUpdate?.Pause();
    }
    
    private void UpdateHoverTime()
    {
        if (_activeRewardsPopup != null)
            return;
        
        _elapsedDuration = Time.time - _startTime;
        if (_elapsedDuration > _hoverTime)
        {
            VisualElement rewardsPopupElement = UiManager.Instance.skillsRewardsPopup.CloneTree();
            rewardsPopupElement.pickingMode = PickingMode.Ignore;
            _activeRewardsPopup = new SkillsRewardsPopup(rewardsPopupElement, LevelReward, UiManager.Instance.uiGameManager.OptionsMenuUi.root, LevelIndex);
        }
    }
    
    private void PointerEnter(PointerEnterEvent evt)
    {
        _startTime = Time.time;
        StartScheduledUpdate();
    }
    
    private void PointerLeave(PointerLeaveEvent evt)
    {
        RemovePopup();
        StopUpdates();
    }
    
    private void RemovePopup()
    {
        if (_activeRewardsPopup == null)
            return;
        
        _activeRewardsPopup.RemovePopup();
        _activeRewardsPopup = null;
    }
    
    #endregion

    private void LevelUp(SkillType skillType, int level)
    {
        CheckRewardsAvailable();
    }

    public bool CheckRewardsAvailable()
    {
        int collectedIndex = _skillsMenu.GetHighestCollectedIndex();
        if (collectedIndex > LevelIndex || _skillsMenu.GetActiveSkillLevel() < LevelIndex + 1)
        {
            StopScaleAnimation();
            return false;
        }
        
        StartScaleAnimation();
        return true;
    }

    public void SetLevelPathState(bool state)
    {
        levelPath.SetEnabled(state);
    }
    
    public void SetLevelNodeState(bool state)
    {
        levelNode.SetEnabled(state);
    }

    public void ApplyPathRightStyle()
    {
        levelNodeContainer.RemoveFromClassList(levelNodeContainerStandard);
        levelNodeContainer.AddToClassList(levelNodeContainerRight);
        
        levelPath.RemoveFromClassList(levelPathStandard);
        levelPath.AddToClassList(levelPathRight);
        
        BaseSkill activeSkill = SkillManager.Instance.Skills[_skillsMenu.ActiveSkillType];
        if (LevelIndex + 1 > activeSkill.Level)
            levelPath.SetEnabled(false);
    }

    public void HideLevelpath()
    {
        levelPath.style.display = DisplayStyle.None;
    }

    public void ReversePathAndLevelNode()
    {
        VisualElement parent = levelNode.parent;

        parent.Remove(levelNode);
        parent.Remove(levelPath);

        parent.Add(levelPath); 
        parent.Add(levelNode);
    }

    private void ShowMenu()
    {
        CheckRewardsAvailable();
    }

    private void HideMenu()
    {
        StopScaleAnimation();
    }
    
    // Call this when the LevelNodeContainer is being destroyed/disposed
    public void Cleanup()
    {
        StopUpdates();
        StopScaleAnimation();
        RemovePopup();
        UnRegisterCallbacks();
    }
}