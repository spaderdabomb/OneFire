using System;
using System.Collections.Generic;
using QuickEye.UIToolkit;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.EventSystems;
using Tab = QuickEye.UIToolkit.Tab;
using GameUI;

namespace OneFireUi
{
    public partial class OptionsMenuUi : IGameMenu
    {
        public VisualElement root;
        public VisualElement rootElement;
        public InventoryMenuUi inventoryMenu;
        public MenuCollections menuCollections;
        public SkillsMenu skillsMenu;

        public List<Tab> playerInfoTabs = new();
        public int CurrentTabIndex { get; private set; } = 0;
        private ITabMenu[] tabMenus;
        private ExitButton exitButton;

        MenuType IGameMenu.MenuType { get; set; } = MenuType.Options;

        public event Action<PointerMoveEvent> OnPointerMoveOptions;
        public event Action OnOptionsHide;

        public OptionsMenuUi(VisualElement root)
        {
            AssignQueryResults(root);

            this.root = root;
            rootElement = optionsUiRoot;
            
            RegisterCallbacks();
            Init();
        }

        private void RegisterCallbacks()
        {
            root.RegisterCallback<PointerMoveEvent>(OnPointerMove);
        }

        private void UnRegisterCallbacks()
        {
            
        }

        private void OnPointerMove(PointerMoveEvent eventData)
        {
            OnPointerMoveOptions?.Invoke(eventData);
        }

        private void Init()
        {
            root.style.display = DisplayStyle.None;
            InitMenus();
            exitButton = new ExitButton(exitButtonUXML);
            
            foreach (Tab tab in optionsTabGroup.contentContainer.Children())
            {
                playerInfoTabs.Add(tab);
                tab.RegisterValueChangedCallback(evt => OnTabIndexChanged(tab.tabIndex));
            }
        }

        private void InitMenus()
        {
            inventoryMenu = new InventoryMenuUi(inventoryMenuUi, 0);
            menuCollections = new MenuCollections(menuCollectionsUi, 1);
            skillsMenu = new SkillsMenu(skillsMenuUi, 2);
            
            tabMenus = new ITabMenu[] { inventoryMenu, menuCollections, skillsMenu };
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
            
            OnTabIndexChanged(CurrentTabIndex);
        }

        public bool ToggleMenu()
        {
            if (root.style.display == DisplayStyle.None)
            {
                ShowMenu();
                return true;
            }
            
            HideMenu();
            OnOptionsHide?.Invoke();
            return false;
        }
        
        void OnTabIndexChanged(int tabIndex)
        {
            CurrentTabIndex = tabIndex;
            foreach (var tabMenu in tabMenus)
            {
                tabMenu.OnTabChanged(tabIndex);
            }
        }

        public void AddElementToRoot(VisualElement element)
        {
            root.Add(element);
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
