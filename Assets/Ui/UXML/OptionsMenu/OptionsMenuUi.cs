using System.Collections.Generic;
using QuickEye.UIToolkit;
using UnityEngine.UIElements;
using UnityEngine;

namespace OneFireUi
{
    public partial class OptionsMenuUi : IGameMenu
    {
        public VisualElement root;
        public VisualElement rootElement;
        public InventoryMenuUi inventoryMenu;
        public MenuCollections menuCollections;

        public List<Tab> playerInfoTabs = new();
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
            
            foreach (Tab tab in optionsTabGroup.contentContainer.Children())
            {
                playerInfoTabs.Add(tab);
                tab.RegisterValueChangedCallback(evt => OnTabIndexChanged(evt, tab));
            }
        }

        private void InitMenus()
        {
            inventoryMenu = new InventoryMenuUi(inventoryMenuUi, 0);
            menuCollections = new MenuCollections(menuCollectionsUi, 1);
            
            tabMenus = new ITabMenu[] { inventoryMenu, menuCollections };
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
            
            HideMenu();
            return false;
        }
        
        void OnTabIndexChanged(ChangeEvent<bool> evt, VisualElement tab)
        {
            Debug.Log(tab);
            Debug.Log(tab.tabIndex);
            foreach (var tabMenu in tabMenus)
            {
                tabMenu.OnTabChanged(tab.tabIndex);
            }
        }
    }
}

public interface ITabMenu
{
    void ShowMenu();
    void HideMenu();
    void OnTabChanged(int newIndex);
    int TabIndex { get; }
}
