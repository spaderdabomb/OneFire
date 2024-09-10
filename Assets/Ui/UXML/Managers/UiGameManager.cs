using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using System;
using UnityEngine.InputSystem;
using Game.Ui;
using DamageNumbersPro;

[DefaultExecutionOrder(-1)]
public partial class UiGameManager : MonoBehaviour
{
    public StructurePlacementMessage structurePlacementMessage;
    public ItemPickupContainer itemPickupContainer;
    public GameObject gameCanvas;

    public OptionsMenuUi OptionsMenuUi { get; private set; }
    public GameSceneUi GameSceneUi { get; private set; }
    public PlayerInteractionMenu PlayerInteractionMenu { get; private set; }
    public List<IGameMenu> GameMenus { get; private set; }
    public Dictionary<MenuType, IGameMenu> GameMenuDict { get; private set; }

    [HideInInspector] public VisualElement root;
    public Action OnShowInteractMenu;
    public Action OnHideInteractMenu;

    public void Init(VisualElement root)
    {
        AssignQueryResults(root);
        this.root = root;

        RegisterCallbacks();

        SetCursorStateVisible(false);

        OptionsMenuUi = new OptionsMenuUi(optionsMenuUi);
        GameSceneUi = new GameSceneUi(gameSceneUi);
        PlayerInteractionMenu = new PlayerInteractionMenu(playerInteractionMenu);

        GameMenus = new List<IGameMenu>() { OptionsMenuUi, PlayerInteractionMenu };
        GameMenuDict = new Dictionary<MenuType, IGameMenu>
        {
            { MenuType.Options, OptionsMenuUi },
            { MenuType.Interact, PlayerInteractionMenu },
        };
    }

    private void RegisterCallbacks()
    {
        InputManager.Instance.RegisterCallback("ToggleOptions", OnToggleOptions);
        InputManager.Instance.RegisterCallback("EscPressed", OnEscPressed);
        InputManager.Instance.RegisterCallback("ToggleCraftingMenu", OnToggleCraftingMenu);

    }

    private void UnregisterCallbacks()
    {
        InputManager.Instance.UnregisterCallback("OptionsToggled");
        InputManager.Instance.UnregisterCallback("EscPressed");
        InputManager.Instance.UnregisterCallback("ToggleCraftingMenu");
    }

    public VisualElement GetPlayerHotbarInventoryRoot()
    {
        return playerHotbarInventory;
    }

    private void OnToggleOptions(InputAction.CallbackContext context)
    {
        ToggleOptionsMenu();
    }

    public void ToggleOptionsMenu()
    {
        if (!IsMenuOpen() && !OptionsMenuUi.IsOpen())
            ShowOptionsMenu();
        else
            ExitCurrentMenu();
    }

    public void ShowOptionsMenu()
    {
        OptionsMenuUi.ShowMenu();
        GameSceneUi.root.style.display = DisplayStyle.None;
        SetInputSettings(MenuType.Options);
    }

    private void OnToggleCraftingMenu(InputAction.CallbackContext context)
    {
        ToggleCraftingMenu(CraftingManager.Instance.playerCraftingStationData, CraftingManager.Instance.playerCraftingStationId);
    }

    public void ToggleCraftingMenu(CraftingStationData craftingStationData, int instanceId)
    {
        bool menuOpen = ToggleInteractMenu();
        if (menuOpen)
            CraftingManager.Instance.ShowCraftingMenu(craftingStationData, instanceId);
    }

    public bool ToggleInteractMenu()
    {
        if (!IsMenuOpen() && !PlayerInteractionMenu.IsOpen())
        {
            ShowInteractMenu();
            return true;
        }
        else
        {
            ExitCurrentMenu();
            return false;
        }
    }

    public void ShowInteractMenu()
    {
        PlayerInteractionMenu.ShowMenu();

        GameSceneUi.root.style.display = DisplayStyle.None;
        SetInputSettings(MenuType.Interact);
    }

    public bool IsMenuOpen()
    {
        foreach (var menu in GameMenus)
        {
            if (menu.IsOpen())
                return true;
        }

        return false;
    }

    public IGameMenu GetMenuOpen()
    {
        foreach (var menu in GameMenus)
        {
            if (menu.IsOpen())
                return menu;
        }

        return null;
    }

    public void OnEscPressed(InputAction.CallbackContext context)
    {
        ExitCurrentMenu();
    }

    public void ExitCurrentMenu()
    {
        PlayerInteractionMenu.HideMenu();
        OptionsMenuUi.HideMenu();

        GameSceneUi.root.style.display = DisplayStyle.Flex;
        SetInputSettings(MenuType.GameScene);
    }

    public void SetCursorStateVisible(bool newState)
    {
        if (newState)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None;
            UnityEngine.Cursor.visible = true;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
    }

    public void SetInputSettings(MenuType menuType)
    {
        if (menuType == MenuType.GameScene)
        {
            InputManager.Instance.SetGameSceneControls();
            SetCursorStateVisible(false);
        }
        else
        {
            InputManager.Instance.SetMenuControls();
            SetCursorStateVisible(true);
        }
    }

    public void SpawnDamageNumberGUI(GameObject damageNumberObj, float value, Vector3 worldPosition)
    {
        GameObject spanwedDamageNumber = Instantiate(damageNumberObj, gameCanvas.transform);
        Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
        spanwedDamageNumber.transform.position = screenPoint;
        spanwedDamageNumber.GetComponent<DamageNumberGUI>().number = Mathf.Floor(value);
    }

    public void SpawnDamageNumberMesh(GameObject damageNumberObj, float value, Vector3 worldPosition)
    {
        GameObject spanwedDamageNumber = Instantiate(damageNumberObj, gameCanvas.transform);
        spanwedDamageNumber.transform.position = worldPosition;
        spanwedDamageNumber.GetComponent<DamageNumberMesh>().number = Mathf.Floor(value);
    }
}

public interface IGameMenu
{
    public MenuType MenuType { get; set; }
    public bool IsOpen();
    public void HideMenu();
    public void ShowMenu();
    public bool ToggleMenu();
}
