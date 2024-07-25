using UnityEngine.UIElements;
using UnityEngine;

namespace OneFireUi
{
    public partial class OptionsMenuUi : IGameMenu
    {
        public VisualElement root;
        public VisualElement rootElement;
        public InventoryMenuUi inventoryMenu;

        private ITabMenu[] tabMenus;
        private ExitButton exitButton;

        MenuType IGameMenu.MenuType { get; set; } = MenuType.Options;

        public OptionsMenuUi(VisualElement root)
        {
            AssignQueryResults(root);

            this.root = root;
            rootElement = optionsUiRoot;

            Init();
        }

        private void Init()
        {
            root.style.display = DisplayStyle.None;
            InitMenus();
            exitButton = new ExitButton(exitButtonUXML);
        }

        private void InitMenus()
        {
            InitInventoryMenu();
            tabMenus = new ITabMenu[] { inventoryMenu };
        }

        private void InitInventoryMenu()
        {
            inventoryMenu = new InventoryMenuUi(inventoryMenuUi);
        }

        public VisualElement GetRootElement()
        {
            return optionsUiRoot;
        }

        public bool IsOpen()
        {
            return root.style.display == DisplayStyle.Flex;
        }

        public void HideMenu()
        {
            root.style.display = DisplayStyle.None;
        }

        public void ShowMenu()
        {
            root.style.display = DisplayStyle.Flex;

            foreach (var menu in tabMenus)
            {
                menu.ShowMenu();
            }
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
}

public interface ITabMenu
{
    void ShowMenu();
}
