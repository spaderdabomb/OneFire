using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerInteract : MonoBehaviour
{
    private Camera playerCamera;

    public List<GameObject> interactingObjects = new();
    private int currentInteractObjectIndex = -1;

    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
    }

    private int GetBestInteractingObjectIndex()
    {
        float smallestAngle = 999f;
        int currentIndex = -1;

        for (int i = 0; i < interactingObjects.Count; i++)
        {
            GameObject interactingObject = interactingObjects[i];
            Vector3 vectorToItem = interactingObject.transform.position - playerCamera.transform.position;
            float angleFromCameraToItem = Mathf.Abs(Vector3.Angle(vectorToItem, playerCamera.transform.forward));

            if (smallestAngle > angleFromCameraToItem)
            {
                smallestAngle = angleFromCameraToItem;
                currentIndex = i;
            }
        }

        return currentIndex;
    }

    public void AddInteractingObject(GameObject newObject)
    {
        interactingObjects.Add(newObject);
    }

    public void RemoveInteractingObject(GameObject newObject)
    {
        interactingObjects.Remove(newObject);
    }
}
