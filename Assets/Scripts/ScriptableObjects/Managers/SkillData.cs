using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    public int skillCap = 50;
    
    public List<int> levelXpTable = new List<int>
    {
        0, 100, 400, 900, 1600, 2500, 3600, 4900, 6400, 8100, 10000,
        12100, 14400, 16900, 19600, 22500, 25600, 28900, 32400, 36100, 40000,
        44100, 48400, 52900, 57600, 62500, 67600, 72900, 78400, 84100, 90000,
        96100, 102400, 108900, 115600, 122500, 129600, 136900, 144400, 152100, 160000,
        168100, 176400, 184900, 193600, 202500, 211600, 220900, 230400, 240100, 250000,
        260100, 270400, 280900, 291600, 302500, 313600, 324900, 336400, 348100, 360000,
        372100, 384400, 396900, 409600, 422500, 435600, 448900, 462400, 476100, 490000,
        504100, 518400, 532900, 547600, 562500, 577600, 592900, 608400, 624100, 640000,
        656100, 672400, 688900, 705600, 722500, 739600, 756900, 774400, 792100, 810000,
        828100, 846400, 864900, 883600, 902500, 921600, 940900, 960400, 980100, 1000000
    };
    
    public int GetLevelFromXp(float totalXp)
    {
        for (int i = levelXpTable.Count - 1; i >= 0; i--)
        {
            if (totalXp >= levelXpTable[i])
                return Mathf.Min(i + 1, skillCap);
        }
        return 1; // Default to level 1 if below all thresholds
    }

    public float GetXpToNextLevel(float totalXp)
    {
        int level = GetLevelFromXp(totalXp);
    
        if (level >= skillCap)
            return 0f; // Maxed out
    
        float currentLevelThreshold = levelXpTable[level - 1];
        float nextLevelThreshold = levelXpTable[level];
    
        return nextLevelThreshold - totalXp;
    }
}

public enum SkillType
{
    Fishing = 0,
    Forestry = 1,
    Mining = 2,
    Combat = 3,
}
