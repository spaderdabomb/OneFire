using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;
using System;
using JSAM;
using static UnityEditor.Experimental.GraphView.GraphView;
using VHierarchy.Libs;
using UnityEditor;

[RequireComponent(typeof(InteractingObject), typeof(Rigidbody), typeof(BoxCollider))]
[RequireComponent(typeof(Outline), typeof(SphereCollider), typeof(TriggersInTrigger))]
public class WorldItem : MonoBehaviour
{
    public ItemData itemDataAsset;
    [ReadOnly] public ItemData itemData;

    public InteractingObject InteractingObject {  get; private set; }
    public Action<string> StackCountChanged;
    public static Action<GameObject> OnWorldItemDestroyed; 
    public bool Combining { get; set; } = false;
    private WorldItem combineTargetWorldItem = null;

    private TriggersInTrigger triggersInTrigger;
    private const float combineRadius = 1f;
    private const float combineDestroyDist = 0.1f; 
    private float canCombineTimer = 1f;

    private string _interactDescription = "Pick up";

    private void Awake()
    {
        InteractingObject = GetComponent<InteractingObject>();
        triggersInTrigger = GetComponent<TriggersInTrigger>();
    }

    private void OnEnable()
    {
        triggersInTrigger.OnTriggerInTriggerEntered += CombineWorldItems;
    }

    private void OnDisable()
    {
        triggersInTrigger.OnTriggerInTriggerEntered -= CombineWorldItems;
    }

    private void Start()
    {
        if (itemData == null)
            InitItemData(Instantiate(itemDataAsset));
    }

    private void Update()
    {
        canCombineTimer -= Time.deltaTime;
        if (canCombineTimer <= 0)
        {
            List<Collider> triggerList = new List<Collider>();
            foreach (var c in triggersInTrigger.triggerList)
            {
                triggerList.Add(c);
            }

            foreach (var c in triggerList)
            {
                CombineWorldItems(c);
            }
        }

        if (Combining && combineTargetWorldItem != null)
        {
            Vector3 newPosition = Vector3.Lerp(transform.position, combineTargetWorldItem.transform.position, 14f*Time.deltaTime);
            transform.position = newPosition;
            if (Vector3.Distance(transform.position, combineTargetWorldItem.transform.position) < combineDestroyDist)
            {
                int newStackCount = itemData.stackCount + combineTargetWorldItem.itemData.stackCount;
                combineTargetWorldItem.SetStackCount(newStackCount);

                Destroy(gameObject);
                combineTargetWorldItem = null;
            }
        }
        else
        {
            Combining = false;
        }
    }

    public void InitItemData(ItemData newItemData)
    {
        itemData = newItemData;
        SetStackCount(itemData.stackCount);
    }

    public void SetStackCount(int newStackCount)
    {
        itemData.stackCount = newStackCount;
        SetDisplay();
    }

    public void SetDisplay()
    {
        InteractingObject.DisplayPretext = _interactDescription;
        InteractingObject.DisplayString = itemData.displayName + " x" + itemData.stackCount.ToString();
        StackCountChanged?.Invoke(InteractingObject.DisplayString);
    }

    public void CombineWorldItems(Collider combineCollider)
    {
        if (itemData.maxStackCount <= 1 || combineCollider == null || canCombineTimer > 0f)
            return;

        WorldItem combineWorldItem;
        if (!combineCollider.gameObject.TryGetComponent(out combineWorldItem))
            return;

        if (combineWorldItem.itemData.itemID != itemData.itemID || Combining || combineWorldItem.canCombineTimer > 0f || combineWorldItem.Combining || canCombineTimer > combineWorldItem.canCombineTimer)
            return;

        int newStackCount = itemData.stackCount + combineWorldItem.itemData.stackCount;
        if (newStackCount > itemData.maxStackCount)
            return;

        combineWorldItem.Combining = true;
        combineWorldItem.combineTargetWorldItem = this;
        combineWorldItem.GetComponent<BoxCollider>().enabled = false;
        combineWorldItem.GetComponent<Rigidbody>().isKinematic = true;

        OnWorldItemDestroyed?.Invoke(combineWorldItem.gameObject);
        GameObjectManager.Instance.playerInteract.RemoveInteractingObject(combineWorldItem.gameObject);
    }

    public void PickUpItem()
    {
        ItemData clonedItemData = itemData.CloneItem();
        int itemsRemaining = InventoryManager.Instance.TryAddItem(clonedItemData);

        if (itemsRemaining == itemData.stackCount)
        {
            AudioManager.PlaySound(MainLibrarySounds.InventoryFull);
        }

        // Inventory is full so set stack count to remaining items
        if (itemsRemaining < itemData.stackCount)
        {
            SetStackCount(itemsRemaining);
        }

        if (itemsRemaining == 0)
        {
            InteractingObject.playerInteract.RemoveInteractingObject(gameObject);
            Destroy(gameObject);
        }
    }

    public void OnDestroy()
    {
        OnWorldItemDestroyed?.Invoke(gameObject);
        InteractingObject.playerInteract.RemoveInteractingObject(gameObject);
    }

    public void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void OnValidate()
    {
#if UNITY_EDITOR

        float maxScale = Mathf.Max(transform.localScale.x, Mathf.Max(transform.localScale.y, transform.localScale.z));
        GetComponent<SphereCollider>().radius = combineRadius * 1 / maxScale;

        if (itemDataAsset == null)
        {
            Debug.Log($"No item data asset assigned to {this} - assign in inspector");
        }

        if (GetComponent<BoxCollider>() != null && GetComponent<MeshCollider>() != null)
        {
            Debug.Log($"{this} has both a BoxCollider and a MeshCollider - are you sure you want both?");
        }

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb.isKinematic)
        {
            Debug.Log($"{this} is kinematic checked - automatically changing");
            rb.isKinematic = false;
        }

        if (rb.collisionDetectionMode != CollisionDetectionMode.Continuous)
        {
            Debug.Log($"{this} collision detection is not set to continuous - automatically changing ");
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        if (gameObject.layer != LayerMask.NameToLayer("Item"))
        {
            Debug.Log($"{this} does not have a default layer assigned, assigning to 'item'");
            gameObject.layer = LayerMask.NameToLayer("Item");
        }

        foreach (Transform child in transform)
        {
            SetLayerRecursively(child.gameObject, LayerMask.NameToLayer("Item"));
        }

        // Combine trigger
        if (GetComponent<SphereCollider>().isTrigger == false)
        {
            Debug.Log($"{this} sphere collider trigger set to false - changing to true");
            GetComponent<SphereCollider>().isTrigger = true;
        }

        // Outline
        if (GetComponent<Outline>().OutlineMode != Outline.Mode.OutlineVisible)
        {
            Debug.Log($"{this} outline mode not set to visible - automatically changing");
            GetComponent<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
        }

        if (GetComponent<Outline>().OutlineWidth != 4f)
        {
            Debug.Log($"{this} outline width not set to {4f} - automatically changing");
            GetComponent<Outline>().OutlineWidth = 4f;
        }

        if (GetComponent<Outline>().OutlineColor != new Color(1, 1, 1, 0.3f))
        {
            Debug.Log($"{this} outline color not set to {new Color(1, 1, 1, 0.3f)} - automatically changing");
            GetComponent<Outline>().OutlineColor = new Color(1, 1, 1, 0.3f);
        }
    }
#endif
}
