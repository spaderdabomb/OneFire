using UnityEngine.UIElements;
using UnityEngine;
using JSAM;

public partial class ExitButton
{   
    public ExitButton(VisualElement root)
    {
        AssignQueryResults(root);
        RegisterCallbacks();
    }

    private void RegisterCallbacks()
    {
        exitButton.clickable.clicked += ExitClicked;
    }

    private void UnregisterCallbacks()
    {
        exitButton.clickable.clicked -= ExitClicked;
    }

    private void ExitClicked()
    {
        AudioManager.PlaySound(MainLibrarySounds.ConfirmTick);
        UiManager.Instance.uiGameManager.ExitCurrentMenu();
    }
}
