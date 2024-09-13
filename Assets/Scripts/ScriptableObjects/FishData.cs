using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "FishData", menuName = "OneFire/Items/FishData")]
public class FishData : ItemData
{
    float catchExp = 10f;
    float escapeBonus = 1f;

    public override void OnValidate()
    {
        base.OnValidate();

        FishRegistry.Register(this);
    }
}
