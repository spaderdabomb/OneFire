// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace OneFireUi
{
    partial class GameSceneUi
    {
        private VisualElement gameSceneRoot;
        private VisualElement bottomContainer;
        private TemplateContainer structurePlacementMessageRoot;
        private TemplateContainer itemPickupContainerRoot;
    
        protected void AssignQueryResults(VisualElement root)
        {
            gameSceneRoot = root.Q<VisualElement>("GameSceneRoot");
            bottomContainer = root.Q<VisualElement>("BottomContainer");
            structurePlacementMessageRoot = root.Q<TemplateContainer>("StructurePlacementMessageRoot");
            itemPickupContainerRoot = root.Q<TemplateContainer>("ItemPickupContainerRoot");
        }
    }
}
