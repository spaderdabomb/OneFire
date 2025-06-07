using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using QuickEye.UIToolkit;
using UnityEngine.Rendering.Universal;
using System;

public partial class MenuCollections : ITabMenu
{
    public VisualElement root;
    private List<CollectionsBiomeTab> biomeTabs;
    private List<BiomeData> biomeDataList;
    private List<CollectionSlot> currentCollectionSlots;
    private List<RaritySlotContainer> currentFishRaritySlots;
    private int currentBiomeTabIndex = 0;
    
    public int TabIndex { get; }

    private const string biomeTabStyle = "biome-tab";
    public MenuCollections(VisualElement root, int tabIndex)
    {
        this.root = root;
        TabIndex = tabIndex;
        
        AssignQueryResults(root);
        RegisterCallbacks();
        InitMenuCollections();
    }

    private void InitMenuCollections()
    {
        InitBiomeTabs();
    }

    private void RegisterCallbacks()
    {
        FishingManager.Instance.OnNewFishCaught += NewFishCaught;
    }

    private void UnregisterCallbacks()
    {
        FishingManager.Instance.OnNewFishCaught -= NewFishCaught;
    }

    private void InitBiomeTabs()
    {
        biomeTabs = new List<CollectionsBiomeTab>();
        biomeDataList = new List<BiomeData>();
        currentCollectionSlots = new List<CollectionSlot>();
        currentFishRaritySlots = new List<RaritySlotContainer>();
        Dictionary<string, BiomeData> biomeDict = BiomeExtensions.GetAllData();

        int i = 0;
        foreach (var kvp in biomeDict)
        {
            VisualElement collectionBiomeTabClone = UiManager.Instance.collectionsBiomeTab.CloneTree();
            CollectionsBiomeTab collectionBiomeTab = new CollectionsBiomeTab(collectionBiomeTabClone, kvp.Value, i);
            biomeTabGroup.Add(collectionBiomeTabClone);
            biomeTabs.Add(collectionBiomeTab);
            biomeDataList.Add(kvp.Value);
            i++;
        }

        UpdateCollectionProgressUI();
        SetBiomeTabIndex(0);
    }

    public void SetBiomeTabIndex(int index)
    {
        for (int i = 0; i < biomeTabs.Count; i++)
        {
            CollectionsBiomeTab biomeTab = biomeTabs[i];
            biomeTab.SetTabSelectedValue(i == index);
        }

        currentBiomeTabIndex = index;
        ClearBiomeTabs();
        SetBiomeTabUI(index);
    }

    private void ClearBiomeTabs()
    {
        foreach (CollectionSlot collectionSlot in currentCollectionSlots)
        {
            collectionSlot.UnregisterCallbacks();
        }

        currentCollectionSlots.Clear();
        collectionSlotContainer.Clear();
    }

    private void SetBiomeTabUI(int index)
    {
        biomeSubheaderLabel.text = biomeDataList[index].displayName;
        Dictionary<BiomeType, List<FishData>> biomeToFishDict = BiomeExtensions.GetAllFishTypesInBiomes();

        int i = 0;
        foreach (KeyValuePair<BiomeType, List<FishData>> kvp in biomeToFishDict)
        {
            if (kvp.Key == biomeDataList[index].biomeType)
            {
                for (int j = 0; j < kvp.Value.Count; j++)
                {
                    FishData fishData = kvp.Value[j];
                    VisualElement collectionSlotClone = UiManager.Instance.collectionSlot.CloneTree();
                    CollectionSlot newCollectionSlot = new CollectionSlot(collectionSlotClone, fishData, j);
                    collectionSlotContainer.Add(collectionSlotClone);
                    currentCollectionSlots.Add(newCollectionSlot);
                    newCollectionSlot.SetTabSelectedValue(j == 0);
                }
            }
            i++;
        }

        UpdateFishObtainedLabel();
        SetCollectionSlotIndex(0);
    }

    public void SetCollectionSlotIndex(int index)
    {
        for (int i = 0; i < currentCollectionSlots.Count; i++)
        {
            CollectionSlot collectionSlot = currentCollectionSlots[i];
            collectionSlot.SetTabSelectedValue(i == index);
            if (i == index)
            {
                ClearCurrentFishUI();
                SetCurrentFishUI(collectionSlot.SlotFishData);
            }
        }

        SetFishRaritySlotIndex(0);
    }

    private void ClearCurrentFishUI()
    {
        // Clear fish rarity UI
        foreach (RaritySlotContainer raritySlotContainer in currentFishRaritySlots)
        {
            raritySlotContainer.UnregisterCallbacks();
        }

        currentFishRaritySlots.Clear();
        raritiesSlotLayout.Clear();
    }

    private void SetCurrentFishUI(FishData fishData)
    {
        currentFisherLabel.text = fishData.displayName;
        fishDescriptionLabel.text = fishData.description;
        rarityFishIcon.style.backgroundImage = FishingManager.Instance.IsFishCaught(fishData.fishID) ? fishData.itemSprite.texture : fishData.uncaughtFishIcon.texture;
        fishInfoColorBg.style.unityBackgroundImageTintColor = GameDataManager.Instance.gameData.rarityToColorDict[fishData.itemRarity];

        // Init Fish Rarity UI
        int i = 0;
        foreach (ItemRarity itemRarity in Enum.GetValues(typeof(ItemRarity)))
        {
            VisualElement fishRaritySlot = UiManager.Instance.raritySlotContainer.CloneTree();
            FishData newFishData = fishData.CreateNew(itemRarity);
            RaritySlotContainer raritySlotContainer = new RaritySlotContainer(fishRaritySlot, newFishData, i);
            currentFishRaritySlots.Add(raritySlotContainer);
            raritiesSlotLayout.Add(fishRaritySlot);
            i++;
        }
    }

    private void SetFishInfoUI(FishData fishData)
    {
        if (FishingManager.Instance.FishDateCaughtDict.TryGetValue(fishData.fishID, out DateTime fishCaughtDate) &&
            FishingManager.Instance.FishTotalCaughtDict.TryGetValue(fishData.fishID, out int fishTotal) &&
            FishingManager.Instance.FishCaughtBestWeightDict.TryGetValue(fishData.fishID, out float fishWeight))
        {
            infoLabelLeft1.text = "First Caught";
            infoLabelRight1.text = fishCaughtDate.ToString();
            infoLabelLeft2.text = "Total Caught";
            infoLabelRight2.text = fishTotal.ToString();
            infoLabelLeft3.text = "Best Weight";
            infoLabelRight3.text = fishWeight.ToString("0.00");
            rarityFishIcon.style.backgroundImage = fishData.itemSprite.texture;
            fishInfoColorBg.style.unityBackgroundImageTintColor = GameDataManager.Instance.gameData.rarityToColorDict[fishData.itemRarity];
        }
        else
        {
            infoLabelLeft1.text = "First Caught";
            infoLabelRight1.text = "-";
            infoLabelLeft2.text = "Total Caught";
            infoLabelRight2.text = "0";
            infoLabelLeft3.text = "Best Weight";
            infoLabelRight3.text = "-";
            rarityFishIcon.style.backgroundImage = fishData.uncaughtFishIcon.texture;
            fishInfoColorBg.style.unityBackgroundImageTintColor = GameDataManager.Instance.gameData.rarityToColorDict[fishData.itemRarity];
        }
    }


    public void SetFishRaritySlotIndex(int index)
    {
        for (int i = 0; i < currentFishRaritySlots.Count; i++)
        {
            RaritySlotContainer raritySlot = currentFishRaritySlots[i];
            raritySlot.SetTabSelectedValue(i == index);
            if (i == index)
            {
                SetFishInfoUI(raritySlot.SlotFishData);
            }
        }
    }

    private void UpdateCollectionProgressUI()
    {
        int fishObtained = FishingManager.Instance.GetTotalFishCollected();
        int totalFish = FishDataExtensions.GetTotalUniqueFish();
        
        float percentComplete = 100 * ((float)fishObtained / (float)totalFish);
        collectionProgressBar.value = percentComplete;
        collectionProgressBar.title = fishObtained.ToString() + "/" + totalFish.ToString();
    }

    private void UpdateFishObtainedLabel()
    {
        BiomeData currentBiomeData = biomeTabs[currentBiomeTabIndex].biomeData;
        Dictionary<BiomeType, List<FishData>> biomeToFishDict = BiomeExtensions.GetAllFishTypesInBiomes();
        List<FishData> biomeFishData = BiomeExtensions.GetAllFishTypesInBiomes()[currentBiomeData.biomeType];
        int numFishInBiome = biomeFishData.Count * Enum.GetValues(typeof(ItemRarity)).Length;
        int numFishCaughtInBiome = FishingManager.Instance.GetFishCollectedInBiome(currentBiomeData.biomeType);
        biomeObtainedLabel.text = "Obtained:  " + numFishCaughtInBiome + "/" + numFishInBiome;
    }

    public void NewFishCaught(FishData fishData)
    {
        UpdateCollectionProgressUI();
    }
    
    public void ShowMenu()
    {
        root.style.display = DisplayStyle.Flex;
    }
    
    public void HideMenu()
    {
        root.style.display = DisplayStyle.None;
    }
    
    public void OnTabChanged(int newIndex)
    {
        if (TabIndex == newIndex)
            ShowMenu();
        else
            HideMenu();
    }
}
