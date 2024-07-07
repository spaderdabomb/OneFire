using UnityEngine.UIElements;
using UnityEngine;

public partial class DerivedTemplate : BaseTemplate
{   
    public DerivedTemplate(VisualElement root): base(root)
    {
        AssignQueryResults(root);
    }

    public override void SetSlotLabel(string slotLabel)
    {
        base.SetSlotLabel(slotLabel);
        // baseSlotLabel.style.color = Color.white;
    }
}
