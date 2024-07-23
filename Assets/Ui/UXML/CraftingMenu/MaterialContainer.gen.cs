// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class MaterialContainer
{
    private VisualElement materialContainer;
    private VisualElement materialIcon;
    private Label materialLabel;
    private Label materialCountLabel;
    
    protected void AssignQueryResults(VisualElement root)
    {
        materialContainer = root.Q<VisualElement>("MaterialContainer");
        materialIcon = root.Q<VisualElement>("MaterialIcon");
        materialLabel = root.Q<Label>("MaterialLabel");
        materialCountLabel = root.Q<Label>("MaterialCountLabel");
    }
}
