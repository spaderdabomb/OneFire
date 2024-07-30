using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StructurePlacer : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;

    private Vector3 currentPlacementPosition = Vector3.zero;

    private void Awake()
    {
        if (playerCamera == null)
            Debug.LogError($"Player camera not assigned to {this}");
    }

    public void PlaceObject()
    {

    }

    public void EnterPlacementMode(GameObject gameObject)
    {

    }
}
