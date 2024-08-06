using UnityEngine;
using System.Collections.Generic;

public class CollisionChecker : MonoBehaviour
{
    public LayerMask collisionLayers;
    public bool isColliding = false;
    private HashSet<Collider> collidingObjects = new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & collisionLayers) != 0)
        {
            collidingObjects.Add(other);
            isColliding = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & collisionLayers) != 0)
        {
            collidingObjects.Remove(other);
            isColliding = collidingObjects.Count > 0;
        }
    }
}