using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;
using UnityEngine.InputSystem;

public class InventoryControlsInput : MonoBehaviour, InventoryControls.IInventoryActionsActions
{
    public InventoryControls InventoryControls { get; private set; }

    private void OnEnable()
    {
        InventoryControls = new InventoryControls();
        InventoryControls.Enable();
        InventoryControls.InventoryActions.Enable();
        InventoryControls.InventoryActions.SetCallbacks(this);
    }

    private void OnDisable()
    {
        InventoryControls.Disable();
        InventoryControls.InventoryActions.Disable();
        InventoryControls.InventoryActions.RemoveCallbacks(this);
    }
    public void OnDropItem(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        InventoryManager.Instance.DropItem();
    }

    public void OnEquipItem(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
    }

    public void OnSplitItemHalf(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        InventoryManager.Instance.PlayerInventory.TrySplitItem(true);
    }

    public void OnSplitItemOne(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;
    }
}
