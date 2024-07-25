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
    public CraftingMenu PlayerCraftingMenu { get; private set; } = null;
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
        PlayerCraftingMenu = new CraftingMenu(craftingMenuClone, playerCraftingStationData);
    }
    private void Update()
    {
        if (PlayerCraftingMenu != null)
        {
            PlayerCraftingMenu.Update();
        }
    }

    public void ShowCraftingMenu(CraftingStationData craftingStationData)
    {
        menuShowing = true;
/*        VisualElement craftingMenuClone = craftingMenuAsset.CloneTree();
        PlayerCraftingMenu = new CraftingMenu(craftingMenuClone, craftingStationData);*/

        UiManager.Instance.uiGameManager.PlayerInteractionMenu.root.Clear();
        UiManager.Instance.uiGameManager.PlayerInteractionMenu.root.Add(PlayerCraftingMenu.root);
    }

    public void CloseCraftingMenu()
    {
/*        PlayerCraftingMenu.root.parent.Remove(PlayerCraftingMenu.root);
        PlayerCraftingMenu = null;*/
        menuShowing = false;
    }
}
