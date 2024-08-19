using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float BaseDamage { get; private set; } = 35f;
    public float BaseSpeed { get; private set; } = 1f;
    public float BaseStamina { get; private set; } = 1f;

    public int LumberjackingLevel { get; private set; } = 1;
    public int MiningLevel { get; private set; } = 1;
    public int FishingLevel { get; private set; } = 1;

    public int LumberjackingXp { get; private set; } = 1;
    public int MiningXp { get; private set; } = 1;
    public int FishingXp { get; private set; } = 1;

    public float CalculateDamage(GameObject targetObject)
    {
        return BaseDamage;
    }

}
