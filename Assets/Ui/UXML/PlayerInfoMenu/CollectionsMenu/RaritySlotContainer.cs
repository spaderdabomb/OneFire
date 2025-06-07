using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

public partial class RaritySlotContainer : ITabInterface
{
    public VisualElement root;
    public FishData SlotFishData;
    public RaritySlotContainer(VisualElement root, FishData fishData, int index)
    {
        this.root = root;
        SlotFishData = fishData;

        AssignQueryResults(root);
        tabRoot.tabIndex = index;
        RegisterCallbacks();
        InitRaritySlot();
    }

    private void InitRaritySlot()
    {
        UpdateRaritySlot(SlotFishData);
    }

    private void UpdateRaritySlot(FishData fishData)
    {
        if (fishData.fishID != SlotFishData.fishID)
            return;
            
        raritySlotLabel.text = fishData.itemRarity.ToString();
        rarityColorBg.style.unityBackgroundImageTintColor = GameDataManager.Instance.gameData.rarityToColorDict[fishData.itemRarity];
        rarityFishIcon.style.backgroundImage = FishingManager.Instance.IsFishCaught(fishData.fishID) ? fishData.itemSprite.texture : fishData.uncaughtFishIcon.texture;
        trophyIcon.style.backgroundImage = FishingManager.Instance.IsFishCaught(fishData.fishID) ? UiManager.Instance.trophyTextures[fishData.itemRarity].texture : UiManager.Instance.trophyTexturesDark[fishData.itemRarity].texture;
    }

    public void RegisterCallbacks()
    {
        tabRoot.RegisterValueChangedCallback(TabIndexChanged);
        tabRoot.RegisterCallback<PointerEnterEvent>(OnHover);
        FishingManager.Instance.OnNewFishCaught += UpdateRaritySlot;
    }

    public void UnregisterCallbacks()
    {
        tabRoot.UnregisterValueChangedCallback(TabIndexChanged);
        tabRoot.UnregisterCallback<PointerEnterEvent>(OnHover);
        FishingManager.Instance.OnNewFishCaught -= UpdateRaritySlot;
    }

    public void TabIndexChanged(ChangeEvent<bool> value)
    {
        UiManager.Instance.uiGameManager.OptionsMenuUi.menuCollections.SetFishRaritySlotIndex(tabRoot.tabIndex);
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
