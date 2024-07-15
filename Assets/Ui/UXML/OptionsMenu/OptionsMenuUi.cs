using UnityEngine.UIElements;
using UnityEngine;

namespace OneFireUi
{
    public partial class OptionsMenuUi
    {
        public VisualElement root;
        public VisualElement rootElement;
        public InventoryMenuUi inventoryMenu;

        private ITabMenu[] tabMenus;
        private ExitButton exitButton;
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

        public void ShowOptionsMenu()
        {
            root.style.display = DisplayStyle.Flex;

            foreach (var menu in tabMenus)
            {
                menu.ShowMenu();
            }
        }
    }

    public interface ITabMenu
    {
        void ShowMenu();
    }
}
