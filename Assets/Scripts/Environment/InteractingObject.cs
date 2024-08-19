using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GinjaGaming.FinalCharacterController;
using JSAM;
using Sirenix.OdinInspector;

[DefaultExecutionOrder(-1)]
public class InteractingObject : MonoBehaviour
{
    [HideInInspector] public PlayerInteract playerInteract;

    [field: SerializeField] public bool ShowPopup { get; set; } = true;
    public string DisplayString { get; set; } = string.Empty;
    public string DisplayPretext { get; set; } = string.Empty;

    private void Awake()
    {
        playerInteract = GameObjectManager.Instance.playerInteract;

        if (playerInteract == null)
            Debug.LogError($"Player interact {playerInteract} is null");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interact") 
            && !playerInteract.interactingObjects.Contains(gameObject) 
            && !playerInteract.damageableObjects.Contains(gameObject))
        {
            if (GetComponent<DamageableObject>() != null)
                playerInteract.AddDamageableObject(gameObject);
            else
                playerInteract.AddInteractingObject(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interact")
            && (playerInteract.interactingObjects.Contains(gameObject) || playerInteract.damageableObjects.Contains(gameObject)))
        {
            if (GetComponent<DamageableObject>() != null)
                playerInteract.RemoveDamageableObject(gameObject);
            else
                playerInteract.RemoveInteractingObject(gameObject);
        }
    }
}
