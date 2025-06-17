using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using System.ComponentModel;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : SerializedScriptableObject
{
    public LayerMask structurePreviewCollisionLayerMask;

    [Header("General")] 
    public string currencyName = "Zennies";
    public float baseSpawnChance = 1f / 30f;
    public Color whiteTextColor = Color.white;
    public Dictionary<ItemRarity, Color> rarityToColorDict;
    public Dictionary<ItemRarity, Color> rarityToSaturatedColorDict;
    public Dictionary<ItemRarity, Color> rarityToLightTextColorDict;

    public Dictionary<ItemRarity, TrophyRank> rarityToTrophyRankDict;
}

public enum TrophyRank
{
    [Description("Amateur")]
    Amatuer = 0,

    [Description("Beginner")]
    Beginner = 1,

    [Description("Adept")]
    Adept = 2,

    [Description("Expert")]
    Expert = 3,

    [Description("Master")]
    Master = 4,

    [Description("Zen Master")]
    ZenMaster = 5
}

