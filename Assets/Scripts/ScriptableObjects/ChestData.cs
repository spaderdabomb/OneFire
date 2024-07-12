using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChestData", menuName = "OneFire/Structures/ChestData")]
public class ChestData : StructureData
{
    [Header("Chest Values")]
    public int numSlots;
}
