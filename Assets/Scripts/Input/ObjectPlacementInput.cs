using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectPlacementInput : MonoBehaviour, UiControls.IObjectPlacementMapActions
{
    [SerializeField] private StructurePlacer structurePlacer;

    private void Awake()
    {
        if (structurePlacer == null)
            Debug.LogError($"structurePlacer is null on {this}");
    }

    public void OnPlaceObject(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        print("Placing object");
        structurePlacer.PlaceObject();
    }
}
