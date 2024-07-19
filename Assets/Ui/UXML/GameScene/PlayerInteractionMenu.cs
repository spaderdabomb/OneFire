using UnityEngine.UIElements;
using UnityEngine;

public partial class PlayerInteractionMenu
{
    public VisualElement root;
    public PlayerInteractionMenu(VisualElement root)
    {
        AssignQueryResults(root);
        this.root = root;

        root.style.display = DisplayStyle.None;
    }
}
