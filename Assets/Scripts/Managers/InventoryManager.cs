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

public class InventoryManager : SerializedMonoBehaviour
{
    public static InventoryManager Instance;

    public VisualTreeAsset inventorySlotAsset;
    public VisualTreeAsset equipmentSlotAsset;

    public VisualElement InventoryContainer { get; private set; }

    [SerializeField] private int playerInventoryRows;
    [SerializeField] private int playerInventoryColumns;
    [SerializeField] private int equipmentInventoryRows;
    [SerializeField] private int equipmentInventoryColumns;

    public Dictionary<ItemType, EquipmentSlotData> equipmentSlotDataDict = new();

    private Vector2 startMousePosition;
    public float timeToShowTooltip = 0.5f;
    [HideInInspector] public BaseInventorySlot CurrentHoverSlot { get; private set; } = null;
    [HideInInspector] public BaseInventorySlot CurrentDraggedSlot { get; private set; } = null;
    [HideInInspector] public bool IsHoveringSlot { get; private set; } = false;
    [HideInInspector] public float TimeSinceHoveringSlot { get; private set; } = 0f;


    public GhostIcon GhostIcon { get; set; }
    public PlayerInventory PlayerInventory { get; private set; } = null;
    public EquipmentInventory EquipmentInventory { get; private set; } = null;
    public PopupMenuInventory popupMenuInventory { get; private set; } = null;

    private List<EquipmentInventorySlot> equipmentSlotsHighlighted = new List<EquipmentInventorySlot>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        VisualElement playerInventoryRoot = UiManager.Instance.optionsMenuUi.inventoryMenu.GetPlayerInventoryRoot();
        VisualElement equipmentInventoryRoot = UiManager.Instance.optionsMenuUi.inventoryMenu.GetEquipmentInventoryRoot();
        PlayerInventory = new PlayerInventory(playerInventoryRoot, playerInventoryRows, playerInventoryColumns);
        EquipmentInventory = new EquipmentInventory(equipmentInventoryRoot, equipmentInventoryRows, equipmentInventoryColumns);
        popupMenuInventory = InitPopupMenu();

        VisualElement ghostIconAsset = UiManager.Instance.ghostIcon.CloneTree();
        GhostIcon = new GhostIcon(ghostIconAsset);
    }

    void Update()
    {
        CheckHoveringTime();
    }

    private PopupMenuInventory InitPopupMenu()
    {
        VisualElement popupMenuAsset = UiManager.Instance.popupMenuInventory.CloneTree();
        PopupMenuInventory newPopupMenuInventory = new PopupMenuInventory(popupMenuAsset);
        UiManager.Instance.optionsMenuUi.root.Add(popupMenuAsset);
        print("Need to add popup menu to inventory");
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
/*        if (CurrentHoverSlot?.currentItemData == null)
            return;

        Vector2 positionDiff = CurrentHoverSlot.parentContainer.root.ChangeCoordinatesTo(popupMenuInventory.root.parent, CurrentHoverSlot.root.layout.position);
        popupMenuInventory.root.style.display = DisplayStyle.Flex;
        popupMenuInventory.root.style.left = positionDiff.x + 70f;
        popupMenuInventory.root.style.top = positionDiff.y + 170f;*/

        // TODO: here

/*        if (!popupMenuInventory.itemDataShowing)
        {
            popupMenuInventory.SetItemData(CurrentHoverSlot.currentItemData);
        }*/
    }

    public void HideInventoryTooltip()
    {
/*        popupMenuInventory.root.style.display = DisplayStyle.None;
        popupMenuInventory.RemoveItemData();*/
    }

    //public void InstantiateItemSpawned(ItemData itemData)
    //{
    //    GameObject newItemSpawned = Instantiate(itemData.item3DPrefab, PrefabManager.Instance.itemContainer.transform);
    //    ItemSpawned newItemSpanwedInst = newItemSpawned.GetComponent<ItemSpawned>();
    //    newItemSpanwedInst.InitItemData();
    //    newItemSpanwedInst.SetStackCount(itemData.stackCount);


    //    newItemSpawned.transform.position = playerCamera.transform.position + playerCamera.transform.forward * 2;
    //    Rigidbody newItemRb = newItemSpawned.GetComponent<Rigidbody>();
    //    newItemRb.velocity = new Vector3(Random.Range(0f, 1f), Random.Range(0.5f, 2f), Random.Range(0f, 1f));
    //    newItemRb.angularVelocity = new Vector3(Random.Range(0f, 10f), Random.Range(0f, 10f), Random.Range(0f, 10f));
    //}

    public void BeginDragHandler(PointerDownEvent evt, BaseInventorySlot draggedInventorySlot)
    {
        // No item exists
        if (draggedInventorySlot.currentItemData == null || evt.button != 0)
            return;

        CurrentDraggedSlot = draggedInventorySlot;
        draggedInventorySlot.parentContainer.currentDraggedInventorySlot = draggedInventorySlot;
        CurrentDraggedSlot.SetTinted();
        CurrentDraggedSlot.SetHighlight();
        SetAllValidSlotHighlights(CurrentDraggedSlot.currentItemData);

        startMousePosition = evt.position;
        GhostIcon.ShowGhostIcon(evt, draggedInventorySlot.currentItemData.itemSprite.texture, draggedInventorySlot.currentItemData.stackCount);

        evt.StopPropagation();
    }

    public void MoveDragHandler(PointerMoveEvent evt)
    {
        print("Moving");

        if (!GhostIcon.root.HasPointerCapture(evt.pointerId))
            return;

        print("Capturing");


        Vector2 displacement = new Vector2(evt.position.x, evt.position.y) - startMousePosition;
        GhostIcon.root.style.left = GhostIcon.StartDragPosition.x + displacement.x;
        GhostIcon.root.style.top = GhostIcon.StartDragPosition.y + displacement.y;

        CurrentHoverSlot = GetHoverSlotFromDrag(evt);
        CurrentHoverSlot?.parentContainer.SetCurrentSlot(CurrentHoverSlot);
        UpdateCurrentGearHoverSlot(evt);
    }

    public void EndDragHandler(PointerUpEvent evt)
    {
        if (!GhostIcon.root.HasPointerCapture(evt.pointerId))
            return;

        if (CurrentHoverSlot != null)
        {
            bool canMoveItem = CurrentHoverSlot.parentContainer.CanMoveItem(CurrentHoverSlot, CurrentDraggedSlot);
            if (canMoveItem)
            {
                CurrentHoverSlot.parentContainer.MoveItem(CurrentHoverSlot, CurrentDraggedSlot);
            }
        }

        GhostIcon.root.ReleasePointer(evt.pointerId);
        evt.StopPropagation();
        GhostIcon.HideGhostIcon();

        CurrentDraggedSlot.ResetTint();
        CurrentDraggedSlot.ResetHighlight();
        ResetAllValidSlotHighlights();
        CurrentDraggedSlot = null;
        PlayerInventory.currentDraggedInventorySlot = null;
    }

    private BaseInventorySlot GetHoverSlotFromDrag(PointerMoveEvent evt)
    {
        EquipmentInventorySlot currentGearSlot = UpdateCurrentGearHoverSlot(evt);
        BaseInventorySlot currentInventorySlot = PlayerInventory.GetCurrentSlotMouseOver(evt);
        PlayerInventory.currentHoverSlot = currentInventorySlot;
        // BaseInventorySlot currentSlot = currentGearSlot != null ? currentGearSlot : currentInventorySlot;
        BaseInventorySlot currentSlot = currentInventorySlot;
        Debug.Log("need to get equipment slot");

        return currentSlot;
    }

    private EquipmentInventorySlot UpdateCurrentGearHoverSlot(PointerMoveEvent evt)
    {
        EquipmentInventorySlot currentGearSlot = null;
        Debug.Log("Need to change UpdateCurrentGearHoverSlot");
/*        foreach (EquipmentInventory currentGearContainer in gearContainerDict.Values)
        {
            GearSlot tempGearSlot = currentGearContainer.GetCurrentSlotMouseOver(evt);
            if (tempGearSlot != null)
                currentGearSlot = tempGearSlot;

            currentGearContainer.SetCurrentSlot(tempGearSlot);
        }*/

        return currentGearSlot;
    }

    public void UpdateCurrentHoverSlot(BaseInventorySlot newHoverSlot, bool isHovering)
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
        Debug.Log("need to highlight gear slots");
/*        foreach (GearContainer gearContainer in gearContainerDict.Values)
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
        }*/
    }

    public void ResetAllValidSlotHighlights()
    {
        Debug.Log("need to reset highlight gear slots");

        /*        foreach (GearSlot gearSlot in gearSlotsHighlighted)
                {
                    gearSlot.ResetHighlight();
                }

                gearSlotsHighlighted.Clear();*/
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

    public void SetAllInventorySlotData()
    {

    }

    public void DropItem(ItemData itemData)
    {

    }
}
