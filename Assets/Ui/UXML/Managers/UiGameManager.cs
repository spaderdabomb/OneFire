using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using System;

public partial class UiGameManager
{
    public OptionsMenuUi OptionsMenuUi { get; private set; }
    public GameSceneUi GameSceneUi { get; private set; }
    public PlayerHotbarInventory PlayerHotbarInventory { get; private set; }
    public PlayerInteractionMenu PlayerInteractionMenu { get; private set; }
    public List<IGameMenu> GameMenus { get; private set; }
    public Dictionary<MenuType, IGameMenu> GameMenuDict { get; private set; }

    public VisualElement root;
    public Action OnShowInteractMenu;
    public Action OnHideInteractMenu;
    public UiGameManager(VisualElement root)
    {
        AssignQueryResults(root);
        this.root = root;
        Init();
    }

    private void Init()
    {
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

    public VisualElement GetPlayerHotbarInventoryRoot()
    {
        return playerHotbarInventory;
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

    public void ToggleCraftingMenu(CraftingStationData craftingStationData)
    {
        bool menuOpen = ToggleInteractMenu();
        if (menuOpen)
            CraftingManager.Instance.ShowCraftingMenu(craftingStationData);
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
}

public interface IGameMenu
{
    public MenuType MenuType { get; set; }
    public bool IsOpen();
    public void HideMenu();
    public void ShowMenu();
    public bool ToggleMenu();
}
