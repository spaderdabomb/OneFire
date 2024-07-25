using UnityEngine.UIElements;
using UnityEngine;
using System;
public partial class PlayerInteractionMenu : IGameMenu
{
    public VisualElement root;
    MenuType IGameMenu.MenuType { get; set; } = MenuType.Interact;
    public PlayerInteractionMenu(VisualElement root)
    {
        AssignQueryResults(root);
        this.root = root;

        root.style.display = DisplayStyle.None;
    }

    public bool IsOpen()
    {
        return root.style.display == DisplayStyle.Flex;
    }

    public void HideMenu()
    {
        root.style.display = DisplayStyle.None;
        UiManager.Instance.uiGameManager.OnHideInteractMenu?.Invoke();
    }

    public void ShowMenu()
    {
        root.style.display = DisplayStyle.Flex;
        UiManager.Instance.uiGameManager.OnShowInteractMenu?.Invoke();
    }

    public bool ToggleMenu()
    {
        if (root.style.display == DisplayStyle.None)
        {
            ShowMenu();
            return true;
        }
        else
        {
            HideMenu();
            return false;
        }
    }
}
