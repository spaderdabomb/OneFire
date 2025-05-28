// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class RaritySlotContainer
{
    private VisualElement raritySlotContainer;
    private QuickEye.UIToolkit.Tab tabRoot;
    private VisualElement rarityColorBg;
    private VisualElement rarityFishIcon;
    private Label raritySlotLabel;
    
    protected void AssignQueryResults(VisualElement root)
    {
        raritySlotContainer = root.Q<VisualElement>("RaritySlotContainer");
        tabRoot = root.Q<QuickEye.UIToolkit.Tab>("tabRoot");
        rarityColorBg = root.Q<VisualElement>("RarityColorBg");
        rarityFishIcon = root.Q<VisualElement>("RarityFishIcon");
        raritySlotLabel = root.Q<Label>("RaritySlotLabel");
    }
}
