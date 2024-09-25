using System;
using UnityEngine;

public class TriggerEnterAction : MonoBehaviour
{
    [SerializeField] private int layerToDetect;

    public Action onTriggerEntered;

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null && other.gameObject.layer == layerToDetect)
        {
            onTriggerEntered?.Invoke();
        }
    }

    public void SetAction(Action action)
    {
        onTriggerEntered = action;
    }
}
