using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LakeData", menuName = "OneFire/Environment/LakeData")]
public class LakeData : ScriptableObject
{
    public List<FishData> fishInLake;
}
