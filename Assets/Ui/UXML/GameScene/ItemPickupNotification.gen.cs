// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

namespace Game.Ui
{
    partial class ItemPickupNotification
    {
        private VisualElement itemPickupNotificationBg;
        private VisualElement itemIcon;
        private Label itemNameLabel;
        private Label totalItemsLabel;
    
        protected void AssignQueryResults(VisualElement root)
        {
            itemPickupNotificationBg = root.Q<VisualElement>("ItemPickupNotificationBg");
            itemIcon = root.Q<VisualElement>("ItemIcon");
            itemNameLabel = root.Q<Label>("ItemNameLabel");
            totalItemsLabel = root.Q<Label>("TotalItemsLabel");
        }
    }
}
