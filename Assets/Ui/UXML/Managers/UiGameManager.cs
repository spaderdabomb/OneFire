using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;
using static UiManager;

public partial class UiGameManager
{
    public OptionsMenuUi OptionsMenuUi;
    public GameSceneUi GameSceneUi;
    public PlayerHotbarInventory PlayerHotbarInventory;
    public PlayerInteractionMenu PlayerInteractionMenu;

    public VisualElement root;
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
    }

    public VisualElement GetPlayerHotbarInventoryRoot()
    {
        return playerHotbarInventory;
    }

    public void ToggleOptionsMenu()
    {
        if (OptionsMenuUi.root.style.display == DisplayStyle.None)
            ShowOptionsMenu();
        else
            HideOptionsMenu();
    }

    public void ShowOptionsMenu()
    {
        if (PlayerInteractionMenu.root.style.display == DisplayStyle.Flex)
            return;

        OptionsMenuUi.ShowOptionsMenu();
        GameSceneUi.root.style.display = DisplayStyle.None;
        SetPlayerInMenuOptions(MenuType.Options);
    }

    public void HideOptionsMenu()
    {
        OptionsMenuUi.root.style.display = DisplayStyle.None;
        GameSceneUi.root.style.display = DisplayStyle.Flex;
        SetPlayerInMenuOptions(MenuType.GameScene);
    }

    public void ShowInteractMenu()
    {
        PlayerInteractionMenu.root.style.display = DisplayStyle.Flex;
        GameSceneUi.root.style.display = DisplayStyle.None;
        SetPlayerInMenuOptions(MenuType.Interact);
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

    public void ExitCurrentMenu()
    {
        PlayerInteractionMenu.root.style.display = DisplayStyle.None;
        OptionsMenuUi.root.style.display = DisplayStyle.None;
        GameSceneUi.root.style.display = DisplayStyle.Flex;
        SetPlayerInMenuOptions(MenuType.GameScene);
    }

    public void SetPlayerInMenuOptions(MenuType menuType)
    {

        if (menuType == MenuType.Options || menuType == MenuType.Map || menuType == MenuType.Interact)
        {
            InputManager.Instance.SetMenuControls();
            SetCursorStateVisible(true);
        }
        else if (menuType == MenuType.GameScene)
        {
            InputManager.Instance.SetGameSceneControls();
            SetCursorStateVisible(false);
        }
    }
}
