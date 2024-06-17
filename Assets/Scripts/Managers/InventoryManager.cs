using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ItemData;
using UnityEngine.UIElements;
using OneFireUi;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

 /*   public VisualTreeAsset inventoryAsset;
    public VisualTreeAsset inventorySlotAsset;
    public VisualTreeAsset gearContainerAsset;
    public VisualTreeAsset gearSlotAsset;

    private int inventoryRows;
    private int inventoryCols;
    private Vector2 startMousePosition;
    private Vector2 startElementPosition;
    public float timeToShowTooltip = 0.5f;
    [HideInInspector] public BaseInventorySlotUi CurrentHoverSlot { get; set; } = null;
    [HideInInspector] public BaseInventorySlotUi CurrentDraggedSlot { get; set; } = null;
    [HideInInspector] public bool IsHoveringSlot { get; set; } = false;
    [HideInInspector] public float TimeSinceHoveringSlot { get; set; } = 0f;


    private Player player;
    public Inventory inventory { get; private set; } = null;
    public PopupMenuInventory popupMenuInventory { get; private set; } = null;
    public VisualElement ghostIcon { get; set; }

    public Dictionary<GearContainerType, GearContainer> gearContainerDict = new();
    public Dictionary<GearContainerType, Dictionary<ItemType, GearSlotData>> gearContainerToDataDicts;

    public Dictionary<ItemType, GearSlotData> handsSlotsDataDict = new();
    public Dictionary<ItemType, GearSlotData> tackleSlotsDataDict = new();
    public Dictionary<ItemType, GearSlotData> outfitSlotsDataDict = new();
    public Dictionary<ItemType, GearSlotData> accessoriesSlotsDataDict = new();

    private List<GearSlot> gearSlotsHighlighted = new List<GearSlot>();

    private void Awake()
    {
        Instance = this;
        player = GameManager.Instance.player.GetComponent<Player>();
        inventoryRows = GameManager.Instance.gameData.inventoryRows;
        inventoryCols = GameManager.Instance.gameData.inventoryCols;

        gearContainerToDataDicts = new()
        {
            { GearContainerType.Hands, handsSlotsDataDict },
            { GearContainerType.Tackle, tackleSlotsDataDict },
            { GearContainerType.Outfit, outfitSlotsDataDict },
            { GearContainerType.Accessories, accessoriesSlotsDataDict }
        };
    }

    private void OnDisable()
    {
        ghostIcon.UnregisterCallback<PointerMoveEvent>(MoveDragHandler);
        ghostIcon.UnregisterCallback<PointerUpEvent>(EndDragHandler);
    }

    void Start()
    {
        VisualElement inventoryClone = inventoryAsset.CloneTree();
        inventory = new Inventory(inventoryClone, inventoryRows, inventoryCols);
        ghostIcon = UIGameManager.Instance.uiGameScene.GetGhostIconRef();
        ghostIcon.RegisterCallback<PointerMoveEvent>(MoveDragHandler);
        ghostIcon.RegisterCallback<PointerUpEvent>(EndDragHandler);

        popupMenuInventory = InitPopupMenu();
        UIGameManager.Instance.uiGameScene.AddInventoryToPlayerInfo(inventoryClone);

        foreach (GearContainerType gearContainerType in Enum.GetValues(typeof(GearContainerType)))
        {
            VisualElement gearContainerClone = gearContainerAsset.CloneTree();
            gearContainerDict.Add(gearContainerType, new GearContainer(gearContainerClone, 1, GameManager.Instance.gameData.gearCols, gearContainerType));
            UIGameManager.Instance.uiGameScene.AddElementToGearContainer(gearContainerClone);
        }
    }

    private PopupMenuInventory InitPopupMenu()
    {
        VisualElement popupMenuAsset = UIGameManager.Instance.popupMenuInventory.CloneTree();
        PopupMenuInventory newPopupMenuInventory = new PopupMenuInventory(popupMenuAsset);
        UIGameManager.Instance.uiGameScene.root.Add(popupMenuAsset);
        newPopupMenuInventory.root.style.position = Position.Absolute;
        newPopupMenuInventory.root.style.display = DisplayStyle.Flex;

        return newPopupMenuInventory;
    }

    // Update is called once per frame
    void Update()
    {
        CheckHoveringTime();
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


    public ItemData InstantiateItem(ItemData itemData)
    {
        ItemData newItemData = Instantiate(itemData);

        return newItemData;
    }

    public void InstantiateItemSpawned(ItemData itemData)
    {
        GameObject newItemSpawned = Instantiate(itemData.item3DPrefab, GameManager.Instance.itemContainer.transform);
        ItemSpawned newItemSpanwedInst = newItemSpawned.GetComponent<ItemSpawned>();
        newItemSpanwedInst.InitItemData();
        newItemSpanwedInst.SetStackCount(itemData.stackCount);

        Transform playerTransform = player.transform;
        Camera playerCamera = GameManager.Instance.playerMovement.playerCamera;

        newItemSpawned.transform.position = playerCamera.transform.position + playerTransform.forward * 2;
        Rigidbody newItemRb = newItemSpawned.GetComponent<Rigidbody>();
        newItemRb.velocity = new Vector3(Random.Range(0f, 1f), Random.Range(0.5f, 2f), Random.Range(0f, 1f));
        newItemRb.angularVelocity = new Vector3(Random.Range(0f, 10f), Random.Range(0f, 10f), Random.Range(0f, 10f));
    }

    public void BeginDragHandler(PointerDownEvent evt, InventorySlot draggedInventorySlot)
    {
        // No item exists
        if (draggedInventorySlot.currentItemData == null || evt.button != 0)
            return;

        CurrentDraggedSlot = draggedInventorySlot;
        draggedInventorySlot.parentContainer.currentDraggedInventorySlot = draggedInventorySlot;
        CurrentDraggedSlot.SetTinted();
        CurrentDraggedSlot.SetHighlight();
        SetAllValidSlotHighlights(CurrentDraggedSlot.currentItemData);

        ShowGhostIcon(evt, draggedInventorySlot.currentItemData.itemSprite.texture, draggedInventorySlot.currentItemData.stackCount);

        evt.StopPropagation();
    }

    public void MoveDragHandler(PointerMoveEvent evt)
    {
        if (!ghostIcon.HasPointerCapture(evt.pointerId))
            return;

        Vector2 displacement = new Vector2(evt.position.x, evt.position.y) - startMousePosition;
        ghostIcon.style.left = startElementPosition.x + displacement.x;
        ghostIcon.style.top = startElementPosition.y + displacement.y;

        CurrentHoverSlot = GetHoverSlotFromDrag(evt);
        CurrentHoverSlot?.parentContainer.SetCurrentSlot(CurrentHoverSlot);
        UpdateCurrentGearHoverSlot(evt);
    }

    public void EndDragHandler(PointerUpEvent evt)
    {
        if (!ghostIcon.HasPointerCapture(evt.pointerId))
            return;

        if (CurrentHoverSlot != null)
        {
            // print(CurrentHoverSlot.parentContainer);
            bool canMoveItem = CurrentHoverSlot.parentContainer.CanMoveItem(CurrentHoverSlot, CurrentDraggedSlot);
            if (canMoveItem)
            {
                CurrentHoverSlot.parentContainer.MoveItem(CurrentHoverSlot, CurrentDraggedSlot);
            }
        }

        ghostIcon.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
        ghostIcon.style.left = 0f;
        ghostIcon.style.top = 0f;
        ghostIcon.style.visibility = Visibility.Hidden;

        CurrentDraggedSlot.ResetTint();
        CurrentDraggedSlot.ResetHighlight();
        ResetAllValidSlotHighlights();
        CurrentDraggedSlot = null;
        inventory.currentDraggedInventorySlot = null;
    }

    private void ShowGhostIcon(PointerDownEvent evt, Texture2D bgTexture, int stackCount)
    {
        Label ghostIconLabel = UIGameManager.Instance.uiGameScene.GetGhostIconLabelRef();
        ghostIcon.style.position = Position.Absolute;
        ghostIcon.style.visibility = Visibility.Visible;
        ghostIcon.style.backgroundImage = bgTexture;
        ghostIconLabel.text = stackCount.ToString();

        startMousePosition = evt.position;
        float positionLeft = ghostIcon.WorldToLocal(evt.position).x - ghostIcon.resolvedStyle.width / 2;
        float positionTop = ghostIcon.WorldToLocal(evt.position).y - ghostIcon.resolvedStyle.height / 2;
        ghostIcon.style.left = positionLeft;
        ghostIcon.style.top = positionTop;
        startElementPosition = new Vector2(positionLeft, positionTop);

        ghostIcon.CapturePointer(evt.pointerId);
    }

    private InventorySlot GetHoverSlotFromDrag(PointerMoveEvent evt)
    {
        GearSlot currentGearSlot = UpdateCurrentGearHoverSlot(evt);
        InventorySlot currentInventorySlot = inventory.GetCurrentSlotMouseOver(evt);
        inventory.currentHoverSlot = currentInventorySlot;
        InventorySlot currentSlot = currentGearSlot != null ? currentGearSlot : currentInventorySlot;

        return currentSlot;
    }

    private GearSlot UpdateCurrentGearHoverSlot(PointerMoveEvent evt)
    {
        GearSlot currentGearSlot = null;
        foreach (GearContainer currentGearContainer in gearContainerDict.Values)
        {
            GearSlot tempGearSlot = currentGearContainer.GetCurrentSlotMouseOver(evt);
            if (tempGearSlot != null)
                currentGearSlot = tempGearSlot;

            currentGearContainer.SetCurrentSlot(tempGearSlot);
        }

        return currentGearSlot;
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

    public bool IsValidHandsSlotItem(ItemData itemData)
    {
        return (itemData.itemCategories == ItemCategory.Wieldable) ||
               (itemData.itemCategories == ItemCategory.Consumable);
    }

    public void SetAllValidSlotHighlights(ItemData itemData)
    {
        foreach (GearContainer gearContainer in gearContainerDict.Values)
        {
            foreach (GearSlot gearSlot in gearContainer.gearSlots)
            {
                bool isHandsContainer = gearSlot.gearContainer.gearContainerType == GearContainerType.Hands;
                if (gearSlot.itemType == itemData.itemType || (isHandsContainer && IsValidHandsSlotItem(itemData)))
                {
                    gearSlot.SetHighlight();
                    gearSlotsHighlighted.Add(gearSlot);
                }
            }
        }
    }

    public void ResetAllValidSlotHighlights()
    {
        foreach (GearSlot gearSlot in gearSlotsHighlighted)
        {
            gearSlot.ResetHighlight();
        }

        gearSlotsHighlighted.Clear();
    }

    public BaseItemData[] GetAllInventorySlotData()
    {
        BaseItemData[] baseItemDataArray = new BaseItemData[inventory.inventorySlots.Count];
        for (int i = 0; i < inventory.inventorySlots.Count; i++)
        {
            if (inventory.inventorySlots[i].currentItemData != null)
            {
                BaseItemData baseItemData = new BaseItemData(inventory.inventorySlots[i].currentItemData);
                baseItemDataArray[i] = baseItemData;
            }
        }

        return baseItemDataArray;
    }

    public BaseItemData[] GetGearSlotData(GearContainerType gearContainerType)
    {
        GearContainer currentGearContainer = gearContainerDict[gearContainerType];
        BaseItemData[] baseItemDataArray = new BaseItemData[currentGearContainer.inventorySlots.Count];
        for (int i = 0; i < currentGearContainer.inventorySlots.Count; i++)
        {
            if (currentGearContainer.inventorySlots[i].currentItemData != null)
            {
                BaseItemData baseItemData = new BaseItemData(currentGearContainer.inventorySlots[i].currentItemData);
                baseItemDataArray[i] = baseItemData;
            }
        }

        return baseItemDataArray;
    }

    public void SetAllInventorySlotData()
    {

    }*/
}
