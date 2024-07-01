using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipmentSlotData", menuName = "OneFire/Equipment/EquipmentSlotData")]
public class EquipmentSlotData : SerializedScriptableObject
{
    public string displayName;
    public Texture2D backingIcon;
}

