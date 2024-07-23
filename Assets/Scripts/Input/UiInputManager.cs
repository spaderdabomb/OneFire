using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class UiInputManager : MonoBehaviour, UiControls.IGameSceneMapActions
{
    public UiControls uiControls { get; private set; }
    public InventoryControlsInput inventoryControlsInput;

    private void Awake()
    {
        inventoryControlsInput = GetComponent<InventoryControlsInput>();
    }

    private void OnEnable()
    {
        uiControls = new UiControls();
        uiControls.Enable();
        uiControls.GameSceneMap.Enable();
        uiControls.GameSceneMap.SetCallbacks(this);
    }

    private void OnDisable()
    {
        uiControls.Disable();
        uiControls.GameSceneMap.Disable();
        uiControls.GameSceneMap.RemoveCallbacks(this);
    }
    public void OnToggleOptions(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        UiManager.Instance.uiGameManager.ToggleOptionsMenu();
    }

    public void OnEscPressed(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        UiManager.Instance.uiGameManager.ExitCurrentMenu();
    }

    public void OnToggleCraftingMenu(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        UiManager.Instance.uiGameManager.ToggleCraftingMenu(CraftingManager.Instance.playerCraftingStationData);
    }
}
