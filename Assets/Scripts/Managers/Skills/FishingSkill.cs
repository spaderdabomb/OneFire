using UnityEngine;

public class FishingSkill : BaseSkill
{
    public FishingSkill(SkillType skillType) : base(skillType)
    {
        RegisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        FishingManager.Instance.OnFishCaught += FishCaught;
    }

    private void UnregisterCallbacks()
    {
        FishingManager.Instance.OnFishCaught -= FishCaught;
    }

    private void FishCaught(FishData fishData)
    {
        AddXp(fishData.catchExp);
    }
    
    public override void Dispose()
    {
        UnregisterCallbacks();
    }
}
