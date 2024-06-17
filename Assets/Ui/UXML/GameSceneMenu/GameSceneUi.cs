using UnityEngine.UIElements;
using UnityEngine;

namespace OneFireUi
{
    public partial class GameSceneUi
    {
        public VisualElement root;
        public GameSceneUi(VisualElement root)
        {
            this.root = root;

            AssignQueryResults(root);
        }
    }
}
