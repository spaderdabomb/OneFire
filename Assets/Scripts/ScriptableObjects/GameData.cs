using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "ZenFisher/GameData/GameData")]
public class GameData : SerializedScriptableObject
{
    public LayerMask structurePreviewCollisionLayerMask;
    
    [Header("General")]
    public float baseSpawnChance = 1f / 30f;
    public Dictionary<ItemRarity, Color> rarityToColorDict;
    public Dictionary<ItemRarity, Color> rarityToSaturatedColorDict;
    
}

