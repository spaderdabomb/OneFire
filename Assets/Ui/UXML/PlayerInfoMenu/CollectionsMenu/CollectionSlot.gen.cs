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
    private VisualElement rarityLightsContainer;
    private VisualElement lightCommon;
    private VisualElement lightUncommon;
    private VisualElement lightRare;
    private VisualElement lightEpic;
    private VisualElement lightLegendary;
    private VisualElement lightMythic;
    
    protected void AssignQueryResults(VisualElement root)
    {
        tabRoot = root.Q<QuickEye.UIToolkit.Tab>("tabRoot");
        fishIcon = root.Q<VisualElement>("FishIcon");
        fishLabel = root.Q<Label>("FishLabel");
        rarityLightsContainer = root.Q<VisualElement>("RarityLightsContainer");
        lightCommon = root.Q<VisualElement>("LightCommon");
        lightUncommon = root.Q<VisualElement>("LightUncommon");
        lightRare = root.Q<VisualElement>("LightRare");
        lightEpic = root.Q<VisualElement>("LightEpic");
        lightLegendary = root.Q<VisualElement>("LightLegendary");
        lightMythic = root.Q<VisualElement>("LightMythic");
    }
}
