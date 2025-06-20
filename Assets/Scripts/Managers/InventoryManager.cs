using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using OneFireUi;
using Random = UnityEngine.Random;
using OneFireUI;
using UnityEngine.EventSystems;
using Sirenix.OdinInspector;
using static UnityEngine.Rendering.DebugUI;
using static UnityEditor.Rendering.FilterWindow;
using JSAM;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using GinjaGaming.FinalCharacterController;

public class InventoryManager : SerializedMonoBehaviour
{
    public static InventoryManager Instance;

    [HideInInspector] public Action<InventorySlot> OnHotbarItemSelectedChanged { get; set; }

    public event Action<ItemData> OnDroppedItem;
    public event Action<ItemData, bool> OnAddedItem;
    public event Action OnInventoryFull;
    public void DroppedItem(ItemData itemData) => OnDroppedItem?.Invoke(itemData);

    public VisualTreeAsset inventorySlotAsset;
    public VisualElement InventoryContainer { get; private set; }
    public ChestInventory lastActiveChest { get; private set; } = null;

    [SerializeField] private int playerHotbarSlots;
    [SerializeField] private int playerInventorySlots;
    [SerializeField] private int equipmentInventorySlots;

    [SerializeField] private VisualTreeAsset chestInventoryAsset;
    [SerializeField] private Vector2 popupOffset = Vector2.zero;

    public Dictionary<ItemType, EquipmentSlotData> equipmentSlotDataDict = new();

    private Vector2 startMousePosition;
    public float timeToShowTooltip = 0.5f;
    [field: SerializeField]public InventorySlot CurrentHoverSlot { get; private set; } = null;
    [field: SerializeField]public InventorySlot DragEndSlot { get; private set; } = null;
    [field: SerializeField]public InventorySlot DragStartSlot { get; private set; } = null;
    public bool IsHoveringSlot { get; private set; } = false;
    public bool IsDragging { get; private set; } = false;
    public float TimeSinceHoveringSlot { get; private set; } = 0f;

    public GhostIcon GhostIcon { get; set; }
    public PlayerInventory PlayerInventory { get; private set; } = null;
    public EquipmentInventory EquipmentInventory { get; private set; } = null;
    public PlayerHotbarInventory PlayerHotbarInventory { get; private set; } = null;
    [SerializeField] public Dictionary<int, ChestInventory> ChestInventoryDict { get; private set; } = null;
    public PopupMenuInventory popupMenuInventory { get; private set; } = null;

    private List<EquipmentSlot> equipmentSlotsHighlighted = new List<EquipmentSlot>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RegisterCallbacks();

        VisualElement playerInventoryRoot = UiManager.Instance.uiGameManager.OptionsMenuUi.inventoryMenu.GetPlayerInventoryRoot();
        VisualElement equipmentInventoryRoot = UiManager.Instance.uiGameManager.OptionsMenuUi.inventoryMenu.GetEquipmentInventoryRoot();
        VisualElement hotbarInventoryRoot = UiManager.Instance.uiGameManager.GetPlayerHotbarInventoryRoot();

        PlayerInventory = new PlayerInventory(playerInventoryRoot, playerInventorySlots, "PlayerInventory", false);
        EquipmentInventory = new EquipmentInventory(equipmentInventoryRoot, equipmentInventorySlots, "EquipmentInventory", false);
        PlayerHotbarInventory = new PlayerHotbarInventory(hotbarInventoryRoot, playerHotbarSlots, "PlayerHotbarInventory", true);

        ChestInventoryDict = new();

        popupMenuInventory = InitPopupMenu();

        VisualElement ghostIconAsset = UiManager.Instance.ghostIcon.CloneTree();
        GhostIcon = new GhostIcon(ghostIconAsset);
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        FishingManager.Instance.OnFishCaught += AddOrSpawnItem;

        InputManager.Instance.RegisterCallback("DropItem", OnDropItem);
        InputManager.Instance.RegisterCallback("SplitItemHalf", OnTrySplitItem);
        InputManager.Instance.RegisterCallback("QuickMove", performedCallback: OnQuickMove);

        InputManager.Instance.RegisterCallback("SelectSlot0", performedCallback: OnSelectSlot0);
        InputManager.Instance.RegisterCallback("SelectSlot1", performedCallback: OnSelectSlot1);
        InputManager.Instance.RegisterCallback("SelectSlot2", performedCallback: OnSelectSlot2);
        InputManager.Instance.RegisterCallback("SelectSlot3", performedCallback: OnSelectSlot3);
        InputManager.Instance.RegisterCallback("SelectSlot4", performedCallback: OnSelectSlot4);
        InputManager.Instance.RegisterCallback("SelectSlot5", performedCallback: OnSelectSlot5);
        InputManager.Instance.RegisterCallback("SelectSlot6", performedCallback: OnSelectSlot6);
        InputManager.Instance.RegisterCallback("SelectSlot7", performedCallback: OnSelectSlot7);
    }

    private void UnregisterCallbacks()
    {
        FishingManager.Instance.OnFishCaught -= AddOrSpawnItem;

        InputManager.Instance.UnregisterCallback("DropItem");
        InputManager.Instance.UnregisterCallback("SplitItemHalf");
        InputManager.Instance.UnregisterCallback("QuickMove");

        InputManager.Instance.UnregisterCallback("SelectSlot0");
        InputManager.Instance.UnregisterCallback("SelectSlot1");
        InputManager.Instance.UnregisterCallback("SelectSlot2");
        InputManager.Instance.UnregisterCallback("SelectSlot3");
        InputManager.Instance.UnregisterCallback("SelectSlot4");
        InputManager.Instance.UnregisterCallback("SelectSlot5");
        InputManager.Instance.UnregisterCallback("SelectSlot6");
        InputManager.Instance.UnregisterCallback("SelectSlot7");
    }

    void Update()
    {
        CheckHoveringTime();
    }

    public void OpenChestInventory(ChestData chestData, int instanceId)
    {
        UiManager.Instance.uiGameManager.ShowInteractMenu();

        foreach (KeyValuePair<int, ChestInventory> kvp in ChestInventoryDict)
        {
            if (instanceId == kvp.Key)
            {
                lastActiveChest = kvp.Value;
                kvp.Value.ShowInventory();
                return;
            }
        }

        MakeChestInventory(chestData, instanceId);
    }

    private void MakeChestInventory(ChestData chestData, int instanceId)
    {
        
        VisualElement chestElement = chestInventoryAsset.CloneTree();
        string fullId = chestData.itemDataAsset.baseName + "_" + chestData.id + "_" + instanceId;
        ChestInventory chestInventory = new ChestInventory(chestElement, chestData.numSlots, fullId);
        ChestInventoryDict.Add(instanceId, chestInventory);
        lastActiveChest = chestInventory;
    }

    private PopupMenuInventory InitPopupMenu()
    {
        VisualElement popupMenuAsset = UiManager.Instance.popupMenuInventory.CloneTree();
        PopupMenuInventory newPopupMenuInventory = new PopupMenuInventory(popupMenuAsset);
        PlayerHotbarInventory.root.Add(popupMenuAsset);
        newPopupMenuInventory.root.style.position = Position.Absolute;
        newPopupMenuInventory.root.style.display = DisplayStyle.Flex;

        return newPopupMenuInventory;
    }

    private void CheckHoveringTime()
    {
        if (IsHoveringSlot)
        {
            if (TimeSinceHoveringSlot > timeToShowTooltip)
                ShowInventoryTooltip();
            else
                HideInventoryTooltip();

            TimeSinceHoveringSlot += Time.deltaTime;
        }
        else
        {
            TimeSinceHoveringSlot = 0f;
            HideInventoryTooltip();
        }
    }

    public void ShowInventoryTooltip()
    {
        if (CurrentHoverSlot?.currentItemData == null)
            return;

        popupMenuInventory.root.style.display = DisplayStyle.Flex;
        popupMenuInventory.root.style.left = CurrentHoverSlot.mousePosition.x + popupOffset.x;
        popupMenuInventory.root.style.top = CurrentHoverSlot.mousePosition.y + popupOffset.y;

        if (!popupMenuInventory.itemDataShowing)
        {
            popupMenuInventory.SetItemData(CurrentHoverSlot.currentItemData);
        }
    }

    public void HideInventoryTooltip()
    {
        if (popupMenuInventory == null)
            return;

        popupMenuInventory.root.style.display = DisplayStyle.None;
        popupMenuInventory.RemoveItemData();
    }

    public void BeginDragHandler(PointerDownEvent evt, InventorySlot draggedInventorySlot)
    {
        // No item exists
        if (draggedInventorySlot.currentItemData == null || evt.button != 0)
            return;

        IsDragging = true;
        DragEndSlot = draggedInventorySlot;
        DragStartSlot = draggedInventorySlot;
        DragStartSlot.SetHoverStyle();
        DragStartSlot.SetHighlight();

        draggedInventorySlot.parentContainer.currentDraggedInventorySlot = draggedInventorySlot;
        EquipmentInventory.SetAllValidSlotHighlights(DragStartSlot.currentItemData);

        startMousePosition = evt.position;
        GhostIcon.ShowGhostIcon(evt, draggedInventorySlot.currentItemData.itemSprite.texture, draggedInventorySlot.currentItemData.stackCount);

        evt.StopPropagation();
    }

    public void MoveDragHandler(PointerMoveEvent evt)
    {
        if (!GhostIcon.root.HasPointerCapture(evt.pointerId))
            return;

        Vector2 displacement = new Vector2(evt.position.x, evt.position.y) - startMousePosition;
        GhostIcon.root.style.left = GhostIcon.StartDragPosition.x + displacement.x;
        GhostIcon.root.style.top = GhostIcon.StartDragPosition.y + displacement.y;
    }

    public void EndDragHandler(PointerUpEvent evt)
    {
        if (!GhostIcon.root.HasPointerCapture(evt.pointerId))
            return;

        DragEndSlot = GetHoverSlotFromDrag(evt);
        DragEndSlot?.parentContainer.SetCurrentHoverSlot(DragEndSlot);

        if (DragEndSlot != null)
        {
            bool canMoveItem = DragEndSlot.parentContainer.CanMoveItem(DragEndSlot, DragStartSlot);
            if (canMoveItem)
            {
                DragEndSlot.parentContainer.MoveItem(DragEndSlot, DragStartSlot);
            }
        }

        if (IsDroppingItem(evt))
            DropItem();

        GhostIcon.root.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
        GhostIcon.HideGhostIcon();

        DragStartSlot.ResetStyle();
        DragStartSlot.ResetHighlight();
        EquipmentInventory.ResetAllValidSlotHighlights();
        DragStartSlot = null;
        DragEndSlot = null;
        PlayerInventory.currentDraggedInventorySlot = null;
        IsDragging = false;
    }

    VisualElement FindElementBehind(VisualElement currentElement, Vector2 position)
    {
        VisualElement elementBehind = null;
        if (currentElement.panel != null)
        {
            GhostIcon.SetPickingModeIgnore();
            elementBehind = UiManager.Instance.uiGameManager.OptionsMenuUi.root.panel.Pick(position);
            GhostIcon.SetPickingModePosition();
        }

        return elementBehind;
    }

    private InventorySlot GetHoverSlotFromDrag(PointerUpEvent evt)
    {
        var hoverElement = FindElementBehind(GhostIcon.root, evt.position);
        object currentObj = hoverElement?.GetFirstAncestorOfType<TemplateContainer>().userData;

        return currentObj is InventorySlot ? (InventorySlot)currentObj : null;
    }

    public void UpdateCurrentHoverSlot(InventorySlot newHoverSlot, bool isHovering)
    {
        if (isHovering)
        {
            CurrentHoverSlot = newHoverSlot;
            IsHoveringSlot = true;
        }
        else
        {
            CurrentHoverSlot = null;
            IsHoveringSlot = false;
            TimeSinceHoveringSlot = 0f;
        }
    }

    private bool IsDroppingItem(PointerUpEvent evt)
    {
        var hoverElement = FindElementBehind(GhostIcon.root, evt.position);
        bool pointerOverDropButton = PlayerInventory.IsPointerOverDropButton(evt);
        bool hoveringDropArea = hoverElement == UiManager.Instance.uiGameManager.OptionsMenuUi.GetRootElement();

        return hoveringDropArea || pointerOverDropButton; ;
    }

    public void DropItem()
    {
        InventorySlot tempSlot = IsDragging ? DragStartSlot : CurrentHoverSlot;
        if (!tempSlot.ContainsItem())
            return;

        DroppedItem(tempSlot.currentItemData);
        tempSlot.parentContainer.RemoveItem(tempSlot);
        AudioManager.PlaySound(MainLibrarySounds.ItemDrop);
    }

    private void OnTrySplitItem(InputAction.CallbackContext context)
    {
        TrySplitItem(true);
    }

    public void TrySplitItem(bool splitHalf)
    {
        if (CurrentHoverSlot == null)
            return;
        
        CurrentHoverSlot.parentContainer.TrySplitItem(splitHalf);
    }

    public BaseItemData[] GetAllInventorySlotData()
    {
        BaseItemData[] baseItemDataArray = new BaseItemData[PlayerInventory.inventorySlots.Count];
        for (int i = 0; i < PlayerInventory.inventorySlots.Count; i++)
        {
            if (PlayerInventory.inventorySlots[i].currentItemData != null)
            {
                BaseItemData baseItemData = new BaseItemData(PlayerInventory.inventorySlots[i].currentItemData);
                baseItemDataArray[i] = baseItemData;
            }
        }

        return baseItemDataArray;
    }

    public void AddOrSpawnItem(ItemData itemData)
    {
        AddOrSpawnItem(itemData, true);
    }

    public void AddOrSpawnItem(ItemData itemData, bool showNotification = true)
    {
        ItemData clonedItemData = itemData.CloneItem();
        int itemsRemaining = TryAddItem(clonedItemData, showNotification);
        if (itemsRemaining > 0)
        {
            ItemData spawnedItemData = clonedItemData.CloneItem();
            GameObjectManager.Instance.SpawnItem(spawnedItemData);
        }
    }

    // For trying to add an existing item to inventory (such as ground item)
    // Returns items remaining, must be handled separately
    public int TryAddItem(ItemData itemData, bool showNotification = true)
    {
        int startingItems = itemData.stackCount;
        int itemsRemaining = PlayerHotbarInventory.TryAddItem(itemData);

        // Add remaining items to inventory
        if (itemsRemaining > 0)
        {
            ItemData playerInventoryItemData = itemData.CloneItem();
            playerInventoryItemData.stackCount = itemsRemaining;

            itemsRemaining = PlayerInventory.TryAddItem(playerInventoryItemData);
        }

        // Inventory not full
        if (startingItems != itemsRemaining)
        {
            ItemData itemDataClone = itemData.CloneItem();
            itemDataClone.stackCount = startingItems - itemsRemaining;
            OnAddedItem?.Invoke(itemDataClone, showNotification);
        }
        // Inventory full
        else
        {
            AudioManager.PlaySound(MainLibrarySounds.ItemPickup);
            OnInventoryFull.Invoke();
        }


        return itemsRemaining;
    }

    public ChestInventory GetActiveChest()
    {
        if (UiManager.Instance.uiGameManager.PlayerInteractionMenu.IsOpen())
        {
            return lastActiveChest;
        }

        return null;
    }

    public void SetAllInventorySlotData()
    {

    }

    public void ConsumeItem(ItemData itemData, int itemQuantity)
    {
        int itemsRemaining = ConsumeItemFromInventory(itemData, itemQuantity, PlayerHotbarInventory);
        if (itemsRemaining > 0)
        {
            itemsRemaining = ConsumeItemFromInventory(itemData, itemsRemaining, PlayerInventory);
        }

        if (itemsRemaining > 0)
        {
            foreach (var chest in ChestInventoryDict)
            {
                ChestInventory chestInventory = chest.Value;
                itemsRemaining = ConsumeItemFromInventory(itemData, itemsRemaining, chestInventory);
            }
        }

        if (itemsRemaining > 0)
        {
            Debug.LogError($"Could not consume {itemQuantity} {itemData} in all inventories, crafting issue occurred");
        }
    }

    public int ConsumeItemFromInventory(ItemData itemData, int itemQuantity, BaseInventory inventory)
    {
        int itemsRemaining = inventory.SubtractItemFromInventory(itemData, itemQuantity);

        return itemsRemaining;
    }

    public void DepositAll(BaseInventory fromInventory, BaseInventory toInventory)
    {
        for (int i = 0; i < fromInventory.inventorySlots.Count; i++)
        {
            InventorySlot fromSlot = fromInventory.inventorySlots[i];
            if (!fromSlot.ContainsItem()) 
                continue;

            int itemsRemaining = toInventory.TryAddItem(fromSlot.currentItemData);
            if (itemsRemaining > 0)
                fromSlot.currentItemData.stackCount = itemsRemaining;
            else
                fromSlot.RemoveItemFromSlot();
        }
    }

    public void DepositSame(BaseInventory fromInventory, BaseInventory toInventory)
    {
        for (int i = 0; i < fromInventory.inventorySlots.Count; i++)
        {
            InventorySlot fromSlot = fromInventory.inventorySlots[i];
            if (!fromSlot.ContainsItem())
                continue;

            // Check if the item exists in the toInventory
            if (toInventory.ContainsItem(fromSlot.currentItemData))
            {
                int itemsRemaining = toInventory.TryAddItem(fromSlot.currentItemData);
                if (itemsRemaining > 0)
                    fromSlot.currentItemData.stackCount = itemsRemaining;
                else
                    fromSlot.RemoveItemFromSlot();
            }
        }
    }

    #region Input callbacks

    private void OnDropItem(InputAction.CallbackContext context)
    {
        DropItem();
    }
    private void OnQuickMove(InputAction.CallbackContext context)
    {
        print("Quick moving");


        InventorySlot tempSlot = IsDragging ? DragStartSlot : CurrentHoverSlot;
        if (!tempSlot.ContainsItem())
            return;

        ItemData itemDataClone = tempSlot.currentItemData.CloneItem();
        int startingStackCount = itemDataClone.stackCount;
        Type inventoryType = tempSlot.parentContainer.GetType();

        if (inventoryType == typeof(PlayerInventory) && GetActiveChest() != null)
        {
            int itemsRemaining = lastActiveChest.TryAddItem(itemDataClone);
            tempSlot.SubtractItemFromSlot(itemDataClone, startingStackCount - itemsRemaining);
        }
        else if (inventoryType == typeof(PlayerInventory) && tempSlot.currentItemData.itemCategories.HasFlag(ItemCategory.Outfit))
        {
            int itemsRemaining = EquipmentInventory.TryAddItem(itemDataClone);
            tempSlot.SubtractItemFromSlot(itemDataClone, startingStackCount - itemsRemaining);
        }
        else if (inventoryType == typeof(PlayerInventory))
        {
            int itemsRemaining = PlayerHotbarInventory.TryAddItem(itemDataClone);
            tempSlot.SubtractItemFromSlot(itemDataClone, startingStackCount - itemsRemaining);
        }
        else
        {
            int itemsRemaining = PlayerInventory.TryAddItem(itemDataClone);
            tempSlot.SubtractItemFromSlot(itemDataClone, startingStackCount - itemsRemaining);
        }
    }

    private void OnSelectSlot(InputAction.CallbackContext context, int slotIndex)
    {
        if (GameObjectManager.Instance.playerState.CurrentPlayerActionState == PlayerActionState.Attacking)
            return;

        PlayerHotbarInventory.SetSelectedIndex(slotIndex);
    }

    private void OnSelectSlot7(InputAction.CallbackContext context)
    {
        OnSelectSlot(context, 7);
    }

    private void OnSelectSlot6(InputAction.CallbackContext context)
    {
        OnSelectSlot(context, 6);
    }

    private void OnSelectSlot5(InputAction.CallbackContext context)
    {
        OnSelectSlot(context, 5);
    }

    private void OnSelectSlot4(InputAction.CallbackContext context)
    {
        OnSelectSlot(context, 4);
    }

    private void OnSelectSlot3(InputAction.CallbackContext context)
    {
        OnSelectSlot(context, 3);
    }

    private void OnSelectSlot2(InputAction.CallbackContext context)
    {
        OnSelectSlot(context, 2);
    }

    private void OnSelectSlot1(InputAction.CallbackContext context)
    {
        OnSelectSlot(context, 1);
    }

    private void OnSelectSlot0(InputAction.CallbackContext context)
    {
        OnSelectSlot(context, 0);
    }
    #endregion

    #region Get item quantities
    public int GetNumItemInInventory(ItemData itemData, BaseInventory inventory)
    {
        int totalItems = 0;

        foreach (var slot in inventory.inventorySlots)
        {
            if (!slot.ContainsItem()) continue;
            if (slot.currentItemData.itemID == itemData.itemID)
            {
                totalItems += slot.currentItemData.stackCount;
            }
        }

        return totalItems;
    }

    public int GetNumItemInChests(ItemData itemData)
    {
        int totalItems = 0;

        foreach (var chest in ChestInventoryDict)
        {
            ChestInventory chestInventory = chest.Value;
            totalItems += GetNumItemInInventory(itemData, chestInventory);
        }

        return totalItems;
    }

    public int GetNumItemOwned(ItemData itemData)
    {
        int totalItems = GetNumItemInInventory(itemData, PlayerInventory);
        totalItems += GetNumItemInInventory(itemData, PlayerHotbarInventory);
        totalItems += GetNumItemInChests(itemData);

        return totalItems;
    }
    #endregion
}
