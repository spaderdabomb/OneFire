using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UIElements;

[DefaultExecutionOrder(2)]
public class CraftingManager : SerializedMonoBehaviour
{
    public static CraftingManager Instance;

    public VisualTreeAsset craftingSlotAsset;
    public VisualTreeAsset craftingContainerAsset;
    public VisualTreeAsset craftingMenuAsset;
    public VisualTreeAsset materialContainerAsset;

    public CraftingStationData playerCraftingStationData;
    public int playerCraftingStationId { get; private set; } = -1;
    public CraftingMenu MainCraftingMenu { get; private set; } = null;
    [SerializeField] public Dictionary<int, CraftingMenu> CraftingMenuDict { get; private set; }

    public bool menuShowing = false;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        UiManager.Instance.uiGameManager.OnHideInteractMenu += CloseCraftingMenu;
    }

    private void OnDisable()
    {
        UiManager.Instance.uiGameManager.OnHideInteractMenu -= CloseCraftingMenu;
    }

    private void Start()
    {
        VisualElement craftingMenuClone = craftingMenuAsset.CloneTree();
        MainCraftingMenu = new CraftingMenu(craftingMenuClone, playerCraftingStationData, "playerCraftingStation_" + playerCraftingStationId);
        CraftingMenuDict = new()
        {
            { -1, MainCraftingMenu }
        };


        for (int i = 0; i < GameObjectManager.Instance.craftingStationList.Count; i++)
        {
            WorldStructure craftingStation = GameObjectManager.Instance.craftingStationList[i];
            CraftingStationData craftingStationData = (CraftingStationData)craftingStation.structureData;
            AddCraftingStation(craftingStationData, i);
        }


     }
    private void Update()
    {
        if (MainCraftingMenu != null)
        {
            MainCraftingMenu.Update();
        }
    }

    public CraftingMenu AddCraftingStation(CraftingStationData craftingStationData, int instanceId)
    {
        VisualElement craftingMenuRoot = craftingMenuAsset.CloneTree();
        string fullId = "CraftingStation_" + craftingStationData.id + "_" + instanceId;
        CraftingMenu craftingMenu = new CraftingMenu(craftingMenuRoot, craftingStationData, fullId);
        CraftingMenuDict.Add(instanceId, craftingMenu);

        return craftingMenu;
    }

    public void ShowCraftingMenu(CraftingStationData craftingStationData, int instanceId)
    {
        menuShowing = true;

        bool menuExists = CraftingMenuDict.ContainsKey(instanceId);
        CraftingMenu selectedMenu = menuExists ? CraftingMenuDict[instanceId] : AddCraftingStation(craftingStationData, instanceId);
        selectedMenu.InitCraftingStation(craftingStationData);

        UiManager.Instance.uiGameManager.PlayerInteractionMenu.root.Clear();
        UiManager.Instance.uiGameManager.PlayerInteractionMenu.root.Add(selectedMenu.root);
        InventoryManager.Instance.PlayerHotbarInventory.HideMenu();
    }

    public void CloseCraftingMenu()
    {
        InventoryManager.Instance.PlayerHotbarInventory.ShowMenu();
        menuShowing = false;
    }
}
