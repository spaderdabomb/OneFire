using UnityEngine.UIElements;
using UnityEngine;

namespace OneFireUi
{
    public partial class GameSceneUi
    {
        public VisualElement root;
        public VisualElement rootElement;
        public MenuType MenuType { get; private set; } = MenuType.GameScene;
        public GameSceneUi(VisualElement root)
        {
            AssignQueryResults(root);

            this.root = root;
            rootElement = gameSceneRoot;
        }
        public void HideMenu()
        {
            root.style.display = DisplayStyle.None;
        }

        public void ShowMenu()
        {
            root.style.display = DisplayStyle.Flex;
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
