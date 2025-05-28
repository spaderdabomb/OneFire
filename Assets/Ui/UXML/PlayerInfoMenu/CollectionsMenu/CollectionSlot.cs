using UnityEngine.UIElements;
using UnityEngine;
using JSAM;
using System;

public partial class CollectionSlot : ITabInterface
{
    public VisualElement root;
    public FishData fishData;
    public CollectionSlot(VisualElement root, FishData fishData, int tabIndex)
    {
        AssignQueryResults(root);

        this.root = root;
        this.fishData = fishData;
        tabRoot.tabIndex = tabIndex;
        InitCollectionSlot();
        RegisterCallbacks();
    }

    private void InitCollectionSlot()
    {
        fishIcon.style.backgroundImage = PersistentDataManager.Instance.IsFishTypeCaught(fishData) ? fishData.itemSprite.texture : fishData.uncaughtFishIcon.texture;
        fishLabel.text = fishData.name;

        ItemRarity[] itemRarityArr = (ItemRarity[])Enum.GetValues(typeof(ItemRarity));
        for (int i = 0; i < itemRarityArr.Length; i++)
        {
            string currentFishID = fishData.baseName + itemRarityArr[i];
            rarityLightsContainer[i].style.backgroundImage = PersistentDataManager.Instance.IsFishCaught(currentFishID) ? UiManager.Instance.statusLightLit : 
                                                                                                                          UiManager.Instance.statusLightUnlit;
        }
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
