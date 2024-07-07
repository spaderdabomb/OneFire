// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class DerivedTemplate
{
    private Label derivedLabel;
    private TemplateContainer baseTemplate;
    
    protected void AssignQueryResults(VisualElement root)
    {
        derivedLabel = root.Q<Label>("DerivedLabel");
        baseTemplate = root.Q<TemplateContainer>("BaseTemplate");
    }
}
