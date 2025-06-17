using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "UITextures", menuName = "Scriptable Objects/UITextures")]
public class UITextureLibrary : SerializedScriptableObject
{
    [Header("Skill Icons")] 
    public Dictionary<SkillType, Sprite> skillIcons;

    [Header("Reward Type Icons")] 
    public Texture2D statBoostRewardIcon;
    public Texture2D itemRewardIcon;
    public Texture2D currentRewardIcon;
    
    [Header("Quest Icons")] 
    public Texture2D questStatusIconDark;
    public Texture2D questStatusIconLight;
    public Texture2D statusLightLit;
    public Texture2D statusLightUnlit;
}
