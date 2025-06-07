// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class CollectionSlot
{
    private QuickEye.UIToolkit.Tab tabRoot;
    private VisualElement fishIcon;
    private Label fishLabel;
    private VisualElement rightContainer;
    private Label rarityTierLabel;
    private VisualElement trophyIcon;
    
    protected void AssignQueryResults(VisualElement root)
    {
        tabRoot = root.Q<QuickEye.UIToolkit.Tab>("tabRoot");
        fishIcon = root.Q<VisualElement>("FishIcon");
        fishLabel = root.Q<Label>("FishLabel");
        rightContainer = root.Q<VisualElement>("RightContainer");
        rarityTierLabel = root.Q<Label>("RarityTierLabel");
        trophyIcon = root.Q<VisualElement>("TrophyIcon");
    }
}
