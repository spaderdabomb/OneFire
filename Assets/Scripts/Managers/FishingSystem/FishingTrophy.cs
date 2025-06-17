using System.Collections;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingTrophy : MonoBehaviour
{
    [SerializeField] private GameObject trophyEnergyEffect;
    [SerializeField] private TextMeshProUGUI trophyText;
    [SerializeField] private Image trophyIcon;
    [SerializeField] private ParticleImage glowEffect;
    [SerializeField] private ParticleImage ringParticleSystem;

    public void Init(FishData fishData)
    {
        string hex = ColorToHex(GameDataManager.Instance.gameData.rarityToLightTextColorDict[fishData.itemRarity]);
        string coloredRarity = $"<color={hex}>{fishData.itemRarity}</color>"; // Gold color

        trophyText.text = $"{coloredRarity} {fishData.displayName}";
        trophyIcon.sprite = UiManager.Instance.trophyTextures[fishData.itemRarity];
        glowEffect.startColor = GameDataManager.Instance.gameData.rarityToLightTextColorDict[fishData.itemRarity];
        ringParticleSystem.startColor = GameDataManager.Instance.gameData.rarityToLightTextColorDict[fishData.itemRarity];
    }
    
    public static string ColorToHex(Color color, bool includeAlpha = false)
    {
        Color32 color32 = color; // Converts float to byte automatically
        return includeAlpha
            ? $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}{color32.a:X2}"
            : $"#{color32.r:X2}{color32.g:X2}{color32.b:X2}";
    }
}
