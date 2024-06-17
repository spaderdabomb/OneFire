using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractingObject : MonoBehaviour
{
    private PlayerInteract playerInteract;
    protected abstract string DisplayString { get; set; }
    protected virtual void Start()
    {
        playerInteract = PrefabManager.Instance.player.GetComponent<PlayerInteract>();
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
