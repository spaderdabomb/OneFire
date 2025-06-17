using System;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Sirenix.OdinInspector;
using UnityEngine;
using RewardSystem;

[DefaultExecutionOrder(-2)]
public class SkillManager : SerializedMonoBehaviour, IPersistentData
{
    public static SkillManager Instance;
    
    public SkillData skillData;
    public Dictionary<SkillType, SkillRewardData> skillRewardDataDict;
    [SerializeField] private bool clearData = false;
    
    public event Action<SkillType, float> OnGainedAnyXp;
    public event Action<SkillType, int> OnGainedAnyLevel;
    
    [HideInInspector] public Dictionary<SkillType, BaseSkill> Skills = new();

    private void Awake()
    {
        Instance = this;
        
        Skills[SkillType.Fishing] = new FishingSkill(SkillType.Fishing);
        Skills[SkillType.Forestry] = new ForestrySkill(SkillType.Forestry);
        Skills[SkillType.Mining] = new MiningSkill(SkillType.Mining);
        Skills[SkillType.Combat] = new CombatSkill(SkillType.Combat);
        
        if (clearData)
            ClearData();
    }

    private void OnEnable()
    {
        // throw new NotImplementedException();
    }

    private void Start()
    {

    }

    private void OnDisable()
    {
        foreach (var skill in Skills.Values)
        {
            skill.Dispose();
        }
    }

    public void AddXp(SkillType type, float amount)
    {
        OnGainedAnyXp?.Invoke(type, amount);
    }

    public void LevelUp(SkillType type, int newLevel)
    {
        OnGainedAnyLevel?.Invoke(type, newLevel);
    }
    
    public void NotifyXp(SkillType type, float xp)
    {
        OnGainedAnyXp?.Invoke(type, xp);
    }

    public void NotifyLevel(SkillType type, int level)
    {
        OnGainedAnyLevel?.Invoke(type, level);
    }

    public void SubscribeToXp(SkillType type, Action<SkillType, float> callback)
    {
        OnGainedAnyXp += callback;
    }

    public void SubscribeToLevel(SkillType type, Action<SkillType, int> callback)
    {
        OnGainedAnyLevel += callback;
    }
    
    public void UnSubscribeToLevel(SkillType type, Action<SkillType, int> callback)
    {
        OnGainedAnyLevel -= callback;
    }
    
    
    public void AddToSaveable()
    {
        PersistentDataManager.Instance.AddToPersistentDataList(this);
    }

    public void LoadData()
    {

    }

    public void SaveData()
    {
        
    }

    private void ClearData()
    {
        foreach (var skill in Skills.Values)
        {
            skill.ClearData();
            skill.SaveData();
        }
    }
}
