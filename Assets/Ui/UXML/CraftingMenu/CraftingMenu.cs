using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
public partial class CraftingMenu : IPersistentData
{
    public CraftingSlotContainer CraftingSlotContainer { get; private set; }
    public CraftingStationData craftingStationData;
    public VisualElement root;

    public List<MaterialContainer> materialContainers;

    public string fullId { get; private set; }
    public bool isCrafting { get; private set; } = false;

    private float totalCraftTimeRemaining = 0f;
    private float singleCraftTimeRemaining = 0f;
    private int selectedIndex = -1;
    private bool loaded = false;

    private int _itemsRemainingToCraft = 0;
    private int _numItemsToCraft;
    public int NumItemsToCraft
    {
        get { return _numItemsToCraft; }
        set 
        {
            if (value < 1)
            {
                _numItemsToCraft = 1;
                return;
            }

            int maxItemsCraftable = GetMaxItemsCraftable(GetCurrentRecipe());
            if (value > maxItemsCraftable && !isCrafting)
            {
                int maxItems = GetMaxItemsCraftable(GetCurrentRecipe());
                _numItemsToCraft = maxItems > 1 ? maxItems : 1;
                return;
            }
            else if (value > (_numItemsToCraft + maxItemsCraftable) && isCrafting)
            {
                int maxItems = _numItemsToCraft + maxItemsCraftable;
                _numItemsToCraft = maxItems > 1 ? maxItems : 1;
                return;
            }
            else
            {
                _numItemsToCraft = value;
            }
        }
    }

    public CraftingMenu(VisualElement root, CraftingStationData craftingStationData, string fullId)
    {
        AssignQueryResults(root);

        this.root = root;
        this.craftingStationData = craftingStationData;
        this.fullId = fullId;

        RegisterCallbacks();
        ShowMenu();
    }

    public void InitCraftingStation()
    {
        materialContainers = new();
        CraftingSlotContainer = new CraftingSlotContainer(craftingSlotContainer, craftingStationData, this);

        if (!loaded)
        {
            AddToSaveable();
            LoadData();
        }
    }

    private void RegisterCallbacks()
    {
        craftButton.clickable.clicked += () => StartCraftingItem(NumItemsToCraft);
        plusButton.clickable.clicked += IncrementNumItemsToCraft;
        minusButton.clickable.clicked += DecrementNumItemsToCraft;
        minButton.clickable.clicked += SetMinItemsToCraft;
        maxButton.clickable.clicked += SetMaxItemsToCraft;
        cancelCraftButton.clickable.clicked += CancelCrafting;
    }

    private void UnregisterCallbacks()
    {
        craftButton.clickable.clicked -= () => StartCraftingItem(NumItemsToCraft);
        plusButton.clickable.clicked -= IncrementNumItemsToCraft;
        minusButton.clickable.clicked -= DecrementNumItemsToCraft;
        minButton.clickable.clicked -= SetMinItemsToCraft;
        maxButton.clickable.clicked -= SetMaxItemsToCraft;
        cancelCraftButton.clickable.clicked -= CancelCrafting;
    }

    public void Update()
    {
        if (isCrafting)
        {
            UpdateSingleCraftTime();
            UpdateCraftProgress();

            if (singleCraftTimeRemaining <= 0)
                UpdateTotalCraftTime();
        }
    }

    private void UpdateCraftProgress()
    {
        RecipeData recipeData = GetCurrentRecipe();
        float progressValue = 100f*Mathf.Min((recipeData.timeToCraft - singleCraftTimeRemaining) / recipeData.timeToCraft, 1f);
        progressBar.value = progressValue;
    }

    private void UpdateSingleCraftTime()
    {
        singleCraftTimeRemaining -= Time.deltaTime;
        singleTimeQuantityLabel.text = ConvertSecondsToString(singleCraftTimeRemaining);

        if (singleCraftTimeRemaining <= 0)
            FinishCraftingItem();
    }

    private void UpdateTotalCraftTime()
    {
        totalCraftTimeRemaining = NumItemsToCraft * GetCurrentRecipe().timeToCraft;
        timeQuantityLabel.text = ConvertSecondsToString(totalCraftTimeRemaining);
    }

    private void UpdateRequiredMaterials()
    {
        foreach (var materialContainer in materialContainers)
        {
            materialContainer.UpdateLabels(_itemsRemainingToCraft, NumItemsToCraft);
        }
    }

    private void ShowMenu()
    {
        rightContainer.style.display = DisplayStyle.None;
    }

    private int GetRecipeIdxFromItem(ItemData itemData)
    {
        for (int i = 0; i < craftingStationData.recipesAvailable.Length; i++)
        {
            RecipeData recipe = craftingStationData.recipesAvailable[i];
            if (recipe.itemResult.itemID == itemData.itemID)
            {
                return i;
            }
        }

        Debug.LogError($"Unable to find {itemData} in recipe list, data mismatch");
        return -1;
    }

    public void RecipeSelected(int recipeIndex)
    {
        ClearRecipe();
        selectedIndex = recipeIndex;
        NumItemsToCraft = 1;
        _itemsRemainingToCraft = 0;
        UpdateCraftQuantityLabels();

        craftButton.style.display = DisplayStyle.Flex;
        rightContainer.style.display = DisplayStyle.Flex;

        CraftingSlot craftingSlot = CraftingSlotContainer.CraftingSlots[recipeIndex];
        RecipeData recipeData = craftingSlot.recipeData;

        singleCraftTimeRemaining = recipeData.timeToCraft;

        slotPreviewIcon.style.backgroundImage = recipeData.itemResult.itemSprite.texture;
        slotPreviewLabel.text = recipeData.itemResult.displayName;

        foreach (var kvp in recipeData.recipe)
        {
            VisualElement materialContainerClone = CraftingManager.Instance.materialContainerAsset.CloneTree();
            MaterialContainer materialContainer = new MaterialContainer(materialContainerClone, kvp.Key, kvp.Value, this);
            materialContainers.Add(materialContainer);
            requiredMaterialsContainer.Add(materialContainerClone);
        }

        ownedQuantityLabel.text = InventoryManager.Instance.GetNumItemOwned(recipeData.itemResult).ToString();
        UpdateTotalCraftTime();
        SetCraftButtonState();
    }

    private void SetCraftButtonState()
    {
        RecipeData recipeData = GetCurrentRecipe();
        bool canCraftRecipe = CanCraftRecipe(recipeData);

        if (canCraftRecipe)
            craftButton.SetEnabled(true);
        else
            craftButton.SetEnabled(false);
    }

    public void ClearRecipe()
    {
        requiredMaterialsContainer.Clear();
        materialContainers.Clear();
    }

    public bool CanCraftRecipe(RecipeData recipeData)
    {
        foreach (var kvp in recipeData.recipe)
        {
            ItemData itemData = kvp.Key;
            int itemQuantity = kvp.Value;
            int ownedItems = InventoryManager.Instance.GetNumItemOwned(itemData);
            if (ownedItems < itemQuantity)
                return false;
        }

        return true;
    }

    private void StartCraftingItem(int itemsToConsume)
    {
        _itemsRemainingToCraft = NumItemsToCraft;

        craftButton.style.display = DisplayStyle.None;
        craftProgressContainer.style.display = DisplayStyle.Flex;


        RecipeData recipeData = GetCurrentRecipe();
        foreach (var itemData in recipeData.recipe)
        {
            InventoryManager.Instance.ConsumeItem(itemData.Key, itemData.Value * itemsToConsume);
        }

        UpdateRequiredMaterials();
        isCrafting = true;
    }

    public void ResumeCraftingItem(SaveableCraftingStation saveableCraftingStation)
    {
        ItemData clonedItemData = ItemExtensions.GetItemData(saveableCraftingStation.itemInProgress.itemID);
        int recipeIndex = GetRecipeIdxFromItem(clonedItemData);
        RecipeSelected(recipeIndex);

        NumItemsToCraft = saveableCraftingStation.itemsRemaining;
        _itemsRemainingToCraft = NumItemsToCraft;
        singleCraftTimeRemaining = saveableCraftingStation.timeRemaining;

        craftButton.style.display = DisplayStyle.None;
        craftProgressContainer.style.display = DisplayStyle.Flex;

        UpdateRequiredMaterials();
        isCrafting = true;
    }

    private void FinishCraftingItem()
    {
        RecipeData recipeData = GetCurrentRecipe();
        AddOrSpawnItem(recipeData.itemResult);

        NumItemsToCraft -= 1;
        _itemsRemainingToCraft -= 1;

        EndCraftingInterface();
    }

    private void AddOrSpawnItem(ItemData itemData)
    {
        ItemData clonedItemData = itemData.CloneItemData();
        int itemsRemaining = InventoryManager.Instance.TryAddItem(clonedItemData);
        if (itemsRemaining > 0)
        {
            ItemData spawnedItemData = clonedItemData.CloneItemData();
            GameObjectManager.Instance.SpawnItem(spawnedItemData);
        }
    }

    private void EndCraftingInterface()
    {
        if (_itemsRemainingToCraft <= 0)
        {
            craftButton.style.display = DisplayStyle.Flex;
            craftProgressContainer.style.display = DisplayStyle.None;
            isCrafting = false;
        }

        RecipeData recipeData = GetCurrentRecipe();
        singleCraftTimeRemaining = recipeData.timeToCraft;

        ownedQuantityLabel.text = InventoryManager.Instance.GetNumItemOwned(recipeData.itemResult).ToString();
        SetCraftButtonState();
        UpdateRequiredMaterials();
        UpdateCraftQuantityLabels();
        UpdateTotalCraftTime();
    }

    private void ReturnMaterials(int recipeQuantity)
    {
        foreach (var materialContainer in materialContainers)
        {
            ItemData clonedItemData = materialContainer.ItemData.CloneItemData();
            clonedItemData.stackCount = materialContainer.ItemQuantity * recipeQuantity;
            AddOrSpawnItem(clonedItemData);
        }
    }

    private void IncrementNumItemsToCraft()
    {
        int currentNumItemsToCraft = NumItemsToCraft;
        NumItemsToCraft++;
        UpdateRequiredMaterials();
        UpdateCraftQuantityLabels();

        if (isCrafting && NumItemsToCraft > currentNumItemsToCraft)
            StartCraftingItem(1);
    }

    private void DecrementNumItemsToCraft()
    {
        int currentNumItemsToCraft = NumItemsToCraft;
        NumItemsToCraft--;
        UpdateRequiredMaterials();
        UpdateCraftQuantityLabels();

        if (isCrafting && NumItemsToCraft < currentNumItemsToCraft)
        {
            _itemsRemainingToCraft -= 1;
            ReturnMaterials(1);
        }
    }

    public int GetMaxItemsCraftable(RecipeData recipeData)
    {
        List<int> itemMultiples = new List<int>();
        foreach (var kvp in recipeData.recipe)
        {
            ItemData itemData = kvp.Key;
            int itemQuantity = kvp.Value;
            int ownedItems = InventoryManager.Instance.GetNumItemOwned(itemData);
            int ownedMultiple = (int)Mathf.Floor(ownedItems / itemQuantity);
            itemMultiples.Add(ownedMultiple);
        }

        return itemMultiples.Min();
    }

    private void SetMaxItemsToCraft()
    {
        RecipeData recipeData = GetCurrentRecipe(); 
        NumItemsToCraft = GetMaxItemsCraftable(recipeData);
        UpdateRequiredMaterials();
        UpdateCraftQuantityLabels();
    }

    private void SetMinItemsToCraft()
    {
        NumItemsToCraft = 1;
        UpdateRequiredMaterials();
        UpdateCraftQuantityLabels();
    }

    private void UpdateCraftQuantityLabels()
    {
        craftAmountLabel.text = NumItemsToCraft.ToString();
        UpdateTotalCraftTime();
    }

    private RecipeData GetCurrentRecipe()
    {
        return CraftingSlotContainer.CraftingSlots[selectedIndex].recipeData;
    }

    public void CancelCrafting()
    {
        ReturnMaterials(_itemsRemainingToCraft);

        NumItemsToCraft = 1;
        _itemsRemainingToCraft = 0;

        EndCraftingInterface();
    }

    public string ConvertSecondsToString(float seconds)
    {
        if (seconds <= 60)
        {
            return Mathf.Ceil(seconds).ToString("F0") + "s";
        }
        else if (seconds < 3600)
        {
            float minutes = seconds / 60f;
            return Mathf.Ceil(minutes).ToString("F0") + "m";
        }
        else
        {
            float hours = seconds / 3600f;
            return Mathf.Ceil(hours).ToString("F0") + "h";
        }
    }

    public void AddToSaveable()
    {
        PersistentDataManager.Instance.AddToPersistentDataList(this);
    }

    public void LoadData()
    {
        SaveableCraftingStation defaultCraftingStation = new SaveableCraftingStation(fullId, null, 0, 0f);
        SaveableCraftingStation saveableCraftingStation = ES3.Load(fullId, defaultValue: defaultCraftingStation);
        if (saveableCraftingStation.itemInProgress != null)
        {
            ResumeCraftingItem(saveableCraftingStation);
        }

        loaded = true;
    }

    public void SaveData()
    {
        BaseItemData baseItemData = isCrafting ? new BaseItemData(GetCurrentRecipe().itemResult) : null;
        SaveableCraftingStation saveableCraftingStation = new SaveableCraftingStation(fullId, baseItemData, _itemsRemainingToCraft, singleCraftTimeRemaining);
        ES3.Save(fullId, saveableCraftingStation);
    }
}

public class SaveableCraftingStation
{
    public string fullId;
    public BaseItemData itemInProgress;
    public int itemsRemaining;
    public float timeRemaining;

    public SaveableCraftingStation(string fullId, BaseItemData itemInProgress, int itemsRemaining, float timeRemaining)
    {
        this.fullId = fullId;
        this.itemInProgress = itemInProgress;
        this.itemsRemaining = itemsRemaining;
        this.timeRemaining = timeRemaining;
    }
}
