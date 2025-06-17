using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UIData", menuName = "Scriptable Objects/UIData")]
public class UIColorData : ScriptableObject
{
    [Header("UI Colors")] 
    public Color blueTextColor;
    public Color blueButtonBorder;
    public Color blueButtonFace;
    public Color blueButtonBorderHover;
    public Color blueButtonFaceHover;
    
    public Color redTextColor;
    public Color redButtonBorder;
    public Color redButtonFace;
    public Color redButtonBorderHover;
    public Color redButtonFaceHover;
    
    public Color greenTextColor;
    public Color greenButtonBorder;
    public Color greenButtonFace;
    public Color greenButtonBorderHover;
    public Color greenButtonFaceHover;
    
    public Color orangeTextColor;
    public Color orangeButtonBorder;
    public Color orangeButtonFace;
    public Color orangeButtonBorderHover;
    public Color orangeButtonFaceHover;

    [HideInInspector] public Dictionary<SkillType, Color> skillTextColors;

    private void OnEnable()
    {
        skillTextColors = new Dictionary<SkillType, Color>
        {
            { SkillType.Fishing, blueTextColor },
            { SkillType.Forestry, greenTextColor },
            { SkillType.Mining, orangeTextColor },
            { SkillType.Combat, redTextColor }
        };
    }
}
