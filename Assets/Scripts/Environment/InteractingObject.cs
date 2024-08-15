using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GinjaGaming.FinalCharacterController;
using JSAM;
using Sirenix.OdinInspector;

public class InteractingObject : MonoBehaviour
{
    [HideInInspector] public PlayerInteract playerInteract;
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Interact") && !playerInteract.interactingObjects.Contains(gameObject))
            playerInteract.AddInteractingObject(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interact") && playerInteract.interactingObjects.Contains(gameObject))
            playerInteract.RemoveInteractingObject(gameObject);
    }
}
