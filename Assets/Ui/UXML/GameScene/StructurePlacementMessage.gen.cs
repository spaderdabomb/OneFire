// -----------------------
// script auto-generated
// any changes to this file will be lost on next code generation
// com.quickeye.ui-toolkit-plus ver: 3.0.3
// -----------------------
using UnityEngine.UIElements;

partial class StructurePlacementMessage
{
    private VisualElement messageBg;
    private Label messageLabel;
    
    protected void AssignQueryResults(VisualElement root)
    {
        messageBg = root.Q<VisualElement>("MessageBg");
        messageLabel = root.Q<Label>("MessageLabel");
    }
}
