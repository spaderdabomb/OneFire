using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

public partial class RaritySlotContainer : ITabInterface
{
    public VisualElement root;
    public FishData fishData;
    public ItemRarity itemRarity;
    public RaritySlotContainer(VisualElement root, FishData fishdata, ItemRarity itemRarity, int index)
    {
        this.root = root;
        this.fishData = fishdata;
        this.itemRarity = itemRarity;

        AssignQueryResults(root);
        tabRoot.tabIndex = index;
        RegisterCallbacks();
        InitRaritySlot();
    }

    private void InitRaritySlot()
    {
        raritySlotLabel.text = itemRarity.ToString();
        rarityColorBg.style.unityBackgroundImageTintColor = GameDataManager.Instance.gameData.rarityToColorDict[itemRarity];
        rarityFishIcon.style.backgroundImage = PersistentDataManager.Instance.IsFishCaught(fishData.baseName) ? fishData.itemSprite.texture : fishData.uncaughtFishIcon.texture;
    }

    public void RegisterCallbacks()
    {
        tabRoot.RegisterValueChangedCallback(TabIndexChanged);
        tabRoot.RegisterCallback<PointerEnterEvent>(OnHover);
    }

    public void UnregisterCallbacks()
    {
        tabRoot.UnregisterValueChangedCallback(TabIndexChanged);
        tabRoot.UnregisterCallback<PointerEnterEvent>(OnHover);
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
