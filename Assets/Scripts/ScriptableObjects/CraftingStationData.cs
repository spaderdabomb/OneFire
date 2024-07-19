using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftingStationData", menuName = "OneFire/Crafting/CraftingStationData")]
public class CraftingStationData : StructureData
{
    [Header("Classification")]
    public RecipeData[] recipesAvailable;

}