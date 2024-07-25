using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Linq;
public partial class CraftingMenu
{
    public CraftingSlotContainer CraftingSlotContainer { get; private set; }
    public CraftingStationData craftingStationData;
    public VisualElement root;

    public CraftingStationData playerCraftingStationData;
    public List<MaterialContainer> materialContainers;

    private float totalCraftTimeRemaining = 0f;
    private float singleCraftTimeRemaining = 0f;
    private int selectedIndex = -1;
    private bool isCrafting = false;

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

    public CraftingMenu(VisualElement root, CraftingStationData craftingStationData)
    {
        AssignQueryResults(root);

        this.root = root;
        this.craftingStationData = craftingStationData;

        Init();
        RegisterCallbacks();
        ShowMenu();
    }

    private void Init()
    {
        materialContainers = new();
        CraftingSlotContainer = new CraftingSlotContainer(craftingSlotContainer, craftingStationData, this);
    }

    private void RegisterCallbacks()
    {
        craftButton.clickable.clicked += () => StartCraftingItem(NumItemsToCraft);
        plusButton.clickable.clicked += IncrementNumItemsToCraft;
        minusButton.clickable.clicked += DecrementNumItemsToCraft;
        maxButton.clickable.clicked += SetMaxItemsToCraft;
        cancelCraftButton.clickable.clicked += CancelCrafting;
    }

    private void UnregisterCallbacks()
    {
        craftButton.clickable.clicked -= () => StartCraftingItem(NumItemsToCraft);
        plusButton.clickable.clicked -= IncrementNumItemsToCraft;
        minusButton.clickable.clicked -= DecrementNumItemsToCraft;
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
            MaterialContainer materialContainer = new MaterialContainer(materialContainerClone, kvp.Key, kvp.Value);
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
}
