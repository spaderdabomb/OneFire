using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using GinjaGaming.FinalCharacterController;
using JSAM;

public abstract class InteractingObject : MonoBehaviour
{
    protected PlayerInteract playerInteract;
    protected abstract string DisplayString { get; set; }

    protected virtual void Start()
    {
        playerInteract = GameObjectManager.Instance.playerInteract;
    }

    public string GetDisplayString()
    {
        return DisplayString;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interact"))
        {
            playerInteract.AddInteractingObject(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Interact"))
        {
            playerInteract.RemoveInteractingObject(gameObject);
        }
    }
}
