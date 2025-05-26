using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using OneFireUi;
using UnityEngine.Rendering;
using JSAM;

public class StructurePlacer : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private LayerMask placementSurfaceMask;
    [SerializeField] private float forwardOffset = 2f;
    [SerializeField] GameObject structureBuildEffectPrefab;
    [SerializeField] private Material validMaterial;
    [SerializeField] private Material invalidMaterial;

    private GameObject previewStructure = null;
    private Vector3 currentPlacementPosition = Vector3.zero;
    private ItemData currentItemData = null;

    public bool inPlacementMode { get; private set; } = false;
    private bool validBuildState = false;
    private float upwardOffset = 100f; // How high up to start the downward raycast
    private float maxDistance = 1000f; // Maximum raycast distance

    private bool _exitedPlacementModeLastFrame = false;

    private void Awake()
    {
        if (playerCamera == null)
            Debug.LogError($"{playerCamera} not assigned to {this}");

        if (structureBuildEffectPrefab == null)
            Debug.LogError($"{structureBuildEffectPrefab} not assigned to {this}");

        if (invalidMaterial == null)
            Debug.LogError($"{invalidMaterial} not assigned to {this}");

        if (validMaterial == null)
            Debug.LogError($"{validMaterial} not assigned to {this}");
    }

    private void Start()
    {
        RegisterCallbcaks();

        InventorySlot slot = InventoryManager.Instance.PlayerHotbarInventory.GetSelectedSlot();
        SetPlacementMode(slot);
    }

    private void OnDestroy()
    {
        UnregisterCallbacks();
    }

    private void RegisterCallbcaks()
    {
        InputManager.Instance.RegisterCallback("PlaceObject", performedCallback: OnPlaceObject);
        InventoryManager.Instance.OnHotbarItemSelectedChanged += SetPlacementMode;
    }

    private void UnregisterCallbacks()
    {
        InputManager.Instance.UnregisterCallback("PlaceObject");
        InventoryManager.Instance.OnHotbarItemSelectedChanged -= SetPlacementMode;
    }

    private void Update()
    {
        if (_exitedPlacementModeLastFrame)
        {
            InputManager.Instance.SetGameSceneControls();
            _exitedPlacementModeLastFrame = false;
        }

        if (!inPlacementMode)
            return;

        UpdateCurrentPlacementPosition();

        Quaternion rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y - 180f, 0f);
        previewStructure.transform.position = currentPlacementPosition;
        previewStructure.transform.rotation = rotation;
        if (CanPlaceStructure())
            SetValidBuildState();
        else
            SetInvalidBuildState();
    }

    private void UpdateCurrentPlacementPosition()
    {
        RaycastHit hitInfo;
        if (PerformRaycast(out hitInfo))
        {
            currentPlacementPosition = hitInfo.point;
        }
    }

    private bool PerformRaycast(out RaycastHit hitInfo)
    {
        Vector3 cameraForward = playerCamera.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 startPos = playerCamera.transform.position + (cameraForward * forwardOffset);
        startPos.y += upwardOffset;
        Vector3 direction = Vector3.down;

        Debug.DrawRay(startPos, direction, Color.red, 0.05f);

        return Physics.Raycast(startPos, direction, out hitInfo, maxDistance, placementSurfaceMask);
    }

    private void OnPlaceObject(InputAction.CallbackContext context)
    {
        if (!inPlacementMode)
        {
            Debug.LogError("Trying to place object while not in placement mode, adjust action map settings");
            return;
        }

        PlaceStructure();
    }

    public void SetPlacementMode(InventorySlot inventorySlot)
    {
        if (!inventorySlot.selected)
            return;

        if (inventorySlot.currentItemData == null || !inventorySlot.currentItemData.itemCategories.HasFlag(ItemCategory.Structure))
        {
            ExitPreviewMode();
            return;
        }

        if (inPlacementMode)
            return;

        EnterPlacementMode(inventorySlot.currentItemData);
    }

    public void EnterPlacementMode(ItemData itemData)
    {
        InputManager.Instance.SetBuildControls();
        currentItemData = itemData;
        GameObject previewPrefab = currentItemData.GetStructureFromItem().structurePreviewPrefab;
        SetPreviewMode(previewPrefab);
        inPlacementMode = true;
    }

    private void ExitPreviewMode()
    {
        Destroy(previewStructure);
        previewStructure = null;
        UiManager.Instance.uiGameManager.structurePlacementMessage.Hide();

        if (inPlacementMode)
            _exitedPlacementModeLastFrame = true;
        inPlacementMode = false;
    }

    public bool CanPlaceStructure()
    {
        return !previewStructure.GetComponent<PreviewCollisionChecker>().isColliding;
    }


    public void PlaceStructure()
    {
        if (currentItemData == null)
        {
            Debug.LogError("Trying to spawn null item as structure");
            return;
        }

        if (!validBuildState)
        {
            PlaceWhileInvalid();
            return;
        }

        GameObject structurePrefab = currentItemData.GetStructureFromItem().structurePrefab;
        Quaternion rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y - 180f, 0f);
        GameObject spawnedStructure = Instantiate(structurePrefab, currentPlacementPosition, rotation, GameObjectManager.Instance.structureContainer.transform);

        GameObjectManager.Instance.AddWorldStructure(spawnedStructure.GetComponent<WorldStructure>());
        Instantiate(structureBuildEffectPrefab, spawnedStructure.transform);
        AudioManager.PlaySound(MainLibrarySounds.PlaceStructure);

        InventorySlot slot = InventoryManager.Instance.PlayerHotbarInventory.GetSelectedSlot();
        InventoryManager.Instance.PlayerHotbarInventory.RemoveItem(slot);

        ExitPreviewMode();
    }

    private void PlaceWhileInvalid()
    {
        // AudioManager.PlaySound(MainLibrarySounds.InvalidPlacement);
    }

    public void SetCraftingInputState()
    {
        if (inPlacementMode)
            InputManager.Instance.EnableActionMap("UiControls", "ObjectPlacementMap");
        else
            InputManager.Instance.DisableActionMap("UiControls", "ObjectPlacementMap");
    }

    private void SetPreviewMode(GameObject obj)
    {
        UpdateCurrentPlacementPosition();
        Quaternion rotation = Quaternion.Euler(0f, playerCamera.transform.eulerAngles.y - 180f, 0f);

        previewStructure = Instantiate(obj, currentPlacementPosition, rotation, GameObjectManager.Instance.structureContainer.transform);
    }

    private void SetValidBuildState()
    {
        if (validBuildState)
            return;

        UiManager.Instance.uiGameManager.structurePlacementMessage.Hide();
        SetMaterialsRecursively(previewStructure, validMaterial);
        validBuildState = true;
    }

    private void SetInvalidBuildState()
    {
        if (!validBuildState)
            return;

        UiManager.Instance.uiGameManager.structurePlacementMessage.Show();
        SetMaterialsRecursively(previewStructure, invalidMaterial);
        validBuildState = false;
    }

    private void SetMaterialsRecursively(GameObject obj, Material material)
    {
        // Set materials on the current object
        MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material[] materials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = material;
            }
            renderer.sharedMaterials = materials;
        }

        // Recursively set materials on all children
        foreach (Transform child in obj.transform)
        {
            SetMaterialsRecursively(child.gameObject, material);
        }
    }
}
