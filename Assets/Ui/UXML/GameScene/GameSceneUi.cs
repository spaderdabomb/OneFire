using UnityEngine.UIElements;
using UnityEngine;

namespace OneFireUi
{
    public partial class GameSceneUi
    {
        public VisualElement root;
        public VisualElement rootElement;
        public GameSceneUi(VisualElement root)
        {
            AssignQueryResults(root);

            this.root = root;
            rootElement = gameSceneRoot;
        }
    }
}
