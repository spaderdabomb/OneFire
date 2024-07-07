using UnityEngine.UIElements;
using UnityEngine;
using OneFireUi;

public partial class UiGameManager
{
    public OptionsMenuUi OptionsMenuUi;
    public UiGameManager(VisualElement root)
    {
        AssignQueryResults(root);
    }

    private void Init(VisualElement root)
    {
        OptionsMenuUi = new OptionsMenuUi(root);
    }
}
