using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FishingRod", menuName = "OneFire/Items/FishingRod")]
public class FishingRodData : WieldableItemData
{
    public float rodSpeed = 1f;
    public float baseHookTime = 10f;
    public float missPenaltyTime = 1f;
    public int segmentBonus = 0;
    public int fishingLevelRequired = 1;
}
