using System;
using UnityEngine;

public abstract class BaseSkill : IPersistentData, IDisposable
{
    public SkillType SkillType;
    public int Level { get; protected set; } = 1;
    public float TotalXp { get; protected set; }

    public event Action<SkillType, float> OnXpGained;
    public event Action<SkillType, int> OnLevelUp;
    
    public float CurrentLevelXp
    {
        get
        {
            int levelIndex = Mathf.Max(0, Level - 1);
            return TotalXp - SkillManager.Instance.skillData.levelXpTable[levelIndex];
        }
    }

    private protected BaseSkill(SkillType skillType)
    {
        this.SkillType = skillType;
        LoadData();
    }

    public virtual void AddXp(float amount)
    {
        TotalXp += amount;

        while (Level < SkillManager.Instance.skillData.levelXpTable.Count && TotalXp >= SkillManager.Instance.skillData.levelXpTable[Level])
        {
            Level++;
            SkillManager.Instance.NotifyLevel(SkillType, Level);
            OnLevelUp?.Invoke(SkillType, Level);
        }
        
        SkillManager.Instance.NotifyXp(SkillType, amount);
        OnXpGained?.Invoke(SkillType, amount);
    }
    
    public void SubscribeToXp(SkillType type, Action<SkillType, float> callback)
    {
        OnXpGained += callback;
    }

    public void SubscribeToLevel(SkillType type, Action<SkillType, int> callback)
    {
        OnLevelUp += callback;
    }

    public virtual void Dispose()
    {
        
    }

    public void AddToSaveable()
    {
        PersistentDataManager.Instance.AddToPersistentDataList(this);
    }

    public void LoadData()
    {
        TotalXp = ES3.Load(nameof(TotalXp), defaultValue: TotalXp);
        Level = ES3.Load(nameof(Level), defaultValue: Level);
    }

    public virtual void SaveData()
    {
        ES3.Save(nameof(TotalXp), TotalXp);
        ES3.Save(nameof(Level), Level);
    }

    public virtual void ClearData()
    {
        TotalXp = 0;
        Level = 1;
    }
}