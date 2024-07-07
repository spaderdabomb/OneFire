using UnityEngine.UIElements;
using UnityEngine;

public partial class BaseTemplate
{   
    public BaseTemplate(VisualElement root)
    {
        AssignQueryResults(root);
    }

    public virtual void SetSlotLabel(string slotLabel)
    {
        baseLabel.text = slotLabel;
    }
}
