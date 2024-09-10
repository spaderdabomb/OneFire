using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

[CreateAssetMenu(fileName = "ItemData", menuName = "OneFire/Items/WeaponData")]
public class WeaponData : ItemData
{
    [Header("Audio")]
    [SerializeField] public SoundFileObject swingSound;

}
