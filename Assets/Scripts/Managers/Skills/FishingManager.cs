using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;

    public Action OnFishCaught;

    private void Awake()
    {
        Instance = this;
    }

    public void BobHitWater()
    {

    }

    public void FishCaught()
    {

    }
}
