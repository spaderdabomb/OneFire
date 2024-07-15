using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemData;
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

public class InventoryManager : SerializedMonoBehaviour
{
    public static InventoryManager Instance;

    public event Action<ItemData> OnDroppedItem;
    public void DroppedItem(ItemData itemData) => OnDroppedItem?.Invoke(itemData);

    public VisualTreeAsset inventorySlotAsset;
    public VisualElement InventoryContainer { get; private set; }

    [SerializeField] private int playerHotbarSlots;
    [SerializeField] private int playerInventorySlots;
    [SerializeField] private int equipmentInventorySlots;

    [SerializeField] private VisualTreeAsset chestInventoryAsset;

    public Dictionary<ItemType, EquipmentSlotData> equipmentSlotDataDict = new();

    private Vector2 startMousePosition;
    public float timeToShowTooltip = 0.5f;
    [field: SerializeField] public InventorySlot CurrentHoverSlot { get; private set; } = null;
    [field: SerializeField] public InventorySlot DragEndSlot { get; private set; } = null;
    [field: SerializeField] public InventorySlot DragStartSlot { get; private set; } = null;
    public bool IsHoveringSlot { get; private set; } = false;
    public bool IsDragging { get; private set; } = false;
    public float TimeSinceHoveringSlot { get; private set; } = 0f;

    public GhostIcon GhostIcon { get; set; }
    public PlayerInventory PlayerInventory { get; private set; } = null;
    public EquipmentInventory EquipmentInventory { get; private set; } = null;
    public PlayerHotbarInventory PlayerHotbarInventory { get; private set; } = null;
    [SerializeField] public Dictionary<string, ChestInventory> ChestInventoryDict { get; private set; } = null;
    public PopupMenuInventory popupMenuInventory { get; private set; } = null;

    private List<EquipmentSlot> equipmentSlotsHighlighted = new List<EquipmentSlot>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        VisualElement playerInventoryRoot = UiManager.Instance.uiGameManager.OptionsMenuUi.inventoryMenu.GetPlayerInventoryRoot();
        VisualElement equipmentInventoryRoot = UiManager.Instance.uiGameManager.OptionsMenuUi.inventoryMenu.GetEquipmentInventoryRoot();
        VisualElement hotbarInventoryRoot = UiManager.Instance.uiGameManager.GetPlayerHotbarInventoryRoot();

        PlayerInventory = new PlayerInventory(playerInventoryRoot, playerInventorySlots, "PlayerInventory");
        EquipmentInventory = new EquipmentInventory(equipmentInventoryRoot, equipmentInventorySlots, "EquipmentInventory");
        PlayerHotbarInventory = new PlayerHotbarInventory(hotbarInventoryRoot, playerHotbarSlots, "PlayerHotbarInventory");

        ChestInventoryDict = new();

        popupMenuInventory = InitPopupMenu();

        VisualElement ghostIconAsset = UiManager.Instance.ghostIcon.CloneTree();
        GhostIcon = new GhostIcon(ghostIconAsset);
    }

    void Update()
    {
        CheckHoveringTime();
    }

    public void OpenChestInventory(ChestData chestData)
    {
        UiManager.Instance.uiGameManager.ShowInteractMenu();

        foreach (KeyValuePair<string, ChestInventory> kvp in ChestInventoryDict)
        {
            if (chestData.id == kvp.Key)
            {
                kvp.Value.ShowInventory();
                return;
            }
        }

        MakeChestInventory(chestData);
    }

    private void MakeChestInventory(ChestData chestData)
    {
        
        VisualElement chestElement = chestInventoryAsset.CloneTree();
        ChestInventory chestInventory = new ChestInventory(chestElement, chestData.numSlots, chestData.id);
        ChestInventoryDict.Add(chestData.id, chestInventory);
        UiManager.Instance.uiGameManager.PlayerInteractionMenu.root.Add(chestElement);
    }

    private PopupMenuInventory InitPopupMenu()
    {
        VisualElement popupMenuAsset = UiManager.Instance.popupMenuInventory.CloneTree();
        PopupMenuInventory newPopupMenuInventory = new PopupMenuInventory(popupMenuAsset);
        UiManager.Instance.uiGameManager.OptionsMenuUi.root.Add(popupMenuAsset);
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

        Vector2 positionDiff = CurrentHoverSlot.parentContainer.root.ChangeCoordinatesTo(popupMenuInventory.root.parent, CurrentHoverSlot.root.layout.position);
        popupMenuInventory.root.style.display = DisplayStyle.Flex;
        popupMenuInventory.root.style.left = positionDiff.x + 70f;
        popupMenuInventory.root.style.top = positionDiff.y + 170f;

        if (!popupMenuInventory.itemDataShowing)
        {
            popupMenuInventory.SetItemData(CurrentHoverSlot.currentItemData);
        }
    }

    public void HideInventoryTooltip()
    {
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
        DragStartSlot.SetTinted();
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

        DragStartSlot.ResetTint();
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
        object currentObj = hoverElement.GetFirstAncestorOfType<TemplateContainer>().userData;

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

    public void TrySplitItem(bool splitHalf)
    {
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

    public int TryAddItem(ItemData itemData)
    {
        int itemsRemaining = PlayerHotbarInventory.TryAddItem(itemData);

        // Added some items to hotbar
        if (itemsRemaining > 0)
        {
            ItemData playerInventoryItemData = itemData.CloneItemData();
            playerInventoryItemData.stackCount = itemsRemaining;

            itemsRemaining = PlayerInventory.TryAddItem(playerInventoryItemData);
        }

        // No items added to hotbar

        return itemsRemaining;
    }

    public void SetAllInventorySlotData()
    {

    }
}
