using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [field: SerializeField] public float BaseDamage { get; private set; } = 35f;
    [field: SerializeField] public float BaseSpeed { get; private set; } = 1f;
    [field: SerializeField] public float BaseStamina { get; private set; } = 1f;
    [field: SerializeField] public float BaseFishingPower { get; private set; } = 1f;

    public int LumberjackingLevel { get; private set; } = 1;
    public int MiningLevel { get; private set; } = 1;
    public int FishingLevel { get; private set; } = 1;

    public int LumberjackingXp { get; private set; } = 1;
    public int MiningXp { get; private set; } = 1;
    public int FishingXp { get; private set; } = 1;

    private PlayerEquippedItem _playerEquippedItem;

    [SerializeField] private float _axeDamageOnTreeMultiplier = 2.5f;
    [SerializeField] private float _pickaxeDamageOnVeinMultiplier = 2.5f;

    private void Awake()
    {
        _playerEquippedItem = GetComponent<PlayerEquippedItem>();
    }

    public float CalculateDamage(GameObject targetObject)
    {
        float objectDamage = BaseDamage;

        bool weaponIsNull = _playerEquippedItem.ActiveItemData != null;
        if (targetObject.GetComponent<WorldTree>() != null)
        {
            if (weaponIsNull && _playerEquippedItem.ActiveItemData.itemType == ItemData.ItemType.Axe)
            {
                objectDamage *= _axeDamageOnTreeMultiplier;
            } 
        }
        else if (targetObject.GetComponent<WorldVein>() != null)
        {
            if (weaponIsNull && _playerEquippedItem.ActiveItemData.itemType == ItemData.ItemType.Pickaxe)
            {
                objectDamage *= _pickaxeDamageOnVeinMultiplier;
            }
            else
            {
                objectDamage = 0f;
            }
        }

        return objectDamage;
    }

    public float GetFishingPower()
    {
        float fishingPower = BaseFishingPower * Mathf.Log10(10f + FishingLevel);

        return fishingPower;
    }

}
