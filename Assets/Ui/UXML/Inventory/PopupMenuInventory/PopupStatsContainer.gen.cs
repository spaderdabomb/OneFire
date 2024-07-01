// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class PopupStatsContainer
{
    private VisualElement popupStatsContainer;
    private VisualElement statsContainerBg;
    private Label statDisplayNameLabel;
    private Label statValueLabel;
    
    protected void AssignQueryResults(VisualElement root)
    {
        popupStatsContainer = root.Q<VisualElement>("PopupStatsContainer");
        statsContainerBg = root.Q<VisualElement>("statsContainerBg");
        statDisplayNameLabel = root.Q<Label>("statDisplayNameLabel");
        statValueLabel = root.Q<Label>("statValueLabel");
    }
}
