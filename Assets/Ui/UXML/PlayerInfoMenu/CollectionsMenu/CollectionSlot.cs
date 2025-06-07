using UnityEngine.UIElements;
using UnityEngine;
using JSAM;
using System;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;

public partial class CollectionSlot : ITabInterface
{
    public VisualElement root;
    public FishData SlotFishData;
    public CollectionSlot(VisualElement root, FishData fishData, int tabIndex)
    {
        AssignQueryResults(root);

        this.root = root;
        SlotFishData = fishData;
        tabRoot.tabIndex = tabIndex;
        InitCollectionSlot();
        RegisterCallbacks();
    }

    private void InitCollectionSlot()
    {
        UpdateCollectionSlotUI(SlotFishData);
    }

    private void UpdateCollectionSlotUI(FishData fishData)
    {
        if (fishData.itemID != SlotFishData.itemID) // important that it is itemID here since it's just by fish type
            return;
        
        fishIcon.style.backgroundImage = FishingManager.Instance.IsFishTypeCaught(fishData) ? fishData.itemSprite.texture : fishData.uncaughtFishIcon.texture;
        fishLabel.text = fishData.displayName;
        
        ItemRarity[] itemRarityArr = (ItemRarity[])Enum.GetValues(typeof(ItemRarity));
        int highestIndex = -1;
        for (int i = 0; i < itemRarityArr.Length; i++)
        {
            string currentFishID = fishData.baseName + itemRarityArr[i];
            if (!FishingManager.Instance.IsFishCaught(currentFishID))
                break;

            highestIndex = i;
        }
        
        trophyIcon.style.backgroundImage = highestIndex == -1 ? UiManager.Instance.trophyTexturesDark[ItemRarity.Common].texture : UiManager.Instance.trophyTextures[itemRarityArr[highestIndex]].texture;
        rarityTierLabel.text = highestIndex == -1 ? "" : GameDataManager.Instance.gameData.rarityToTrophyRankDict[itemRarityArr[highestIndex]].GetDescription();
        rarityTierLabel.style.color = highestIndex == -1 ? GameDataManager.Instance.gameData.whiteTextColor : GameDataManager.Instance.gameData.rarityToLightTextColorDict[itemRarityArr[highestIndex]];
    }

    public void RegisterCallbacks()
    {
        tabRoot.RegisterValueChangedCallback(TabIndexChanged);
        tabRoot.RegisterCallback<PointerEnterEvent>(OnHover);
        FishingManager.Instance.OnNewFishCaught += UpdateCollectionSlotUI;
    }

    public void UnregisterCallbacks()
    {
        tabRoot.UnregisterValueChangedCallback(TabIndexChanged);
        tabRoot.UnregisterCallback<PointerEnterEvent>(OnHover);
        FishingManager.Instance.OnNewFishCaught -= UpdateCollectionSlotUI;
    }

    public void TabIndexChanged(ChangeEvent<bool> value)
    {
        UiManager.Instance.uiGameManager.OptionsMenuUi.menuCollections.SetCollectionSlotIndex(tabRoot.tabIndex);
        AudioManager.PlaySound(MainLibrarySounds.ConfirmTick);
    }

    public void SetTabSelectedValue(bool value)
    {
        tabRoot.SetValueWithoutNotify(value);
    }

    public void OnHover(PointerEnterEvent evt)
    {
        AudioManager.PlaySound(MainLibrarySounds.WoodenTick);
    }
}
