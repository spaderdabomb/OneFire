using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GinjaGaming.FinalCharacterController
{
    public class PlayerInteract : MonoBehaviour
    {
        public List<GameObject> interactingObjects;
        private int currentInteractObjectIndex = -1;
        private InteractPopup interactPopup;

        private void OnEnable()
        {
            InputManager.Instance.playerActionsInput.onInteract += PickUpItem;
        }

        private void OnDisable()
        {
            InputManager.Instance.playerActionsInput.onInteract -= PickUpItem;
        }

        private void Start()
        {
            interactingObjects = new List<GameObject>();
            interactPopup = UiManager.Instance.interactPopup;
        }

        private void Update()
        {
            UpdateInteractUI();
        }

        private void PickUpItem()
        {
            int objectIndex = GetBestInteractingObjectIndex();
            GameObject currentInteractingObject = interactingObjects[objectIndex];
            if (currentInteractingObject.GetComponent<ItemSpawned>() != null)
            {
                currentInteractingObject.GetComponent<ItemSpawned>().PickUpItem();
            }
        }

        public void AddInteractingObject(GameObject newObject)
        {
            interactingObjects.Add(newObject);
        }

        public void RemoveInteractingObject(GameObject newObject)
        {
            interactingObjects.Remove(newObject);
        }

        private void UpdateInteractUI()
        {
            if (interactingObjects.Count > 0)
            {
                currentInteractObjectIndex = GetBestInteractingObjectIndex();
                string interactingObjectStr = interactingObjects[currentInteractObjectIndex].GetComponent<InteractingObject>().GetDisplayString();
                interactPopup.ShowPopup(interactingObjectStr, GameDataManager.Instance.standardInteractTextColor);
            }
            else if (interactPopup.gameObject.activeSelf)
            {
                interactPopup.HidePopup();
            }
        }

        private int GetBestInteractingObjectIndex()
        {
            float smallestAngle = 999f;
            int currentIndex = -1;

            for (int i = 0; i < interactingObjects.Count; i++)
            {
                GameObject interactingObject = interactingObjects[i];
                Vector3 vectorToItem = interactingObject.transform.position - transform.position;
                float angleFromCameraToItem = Mathf.Abs(Vector3.Angle(vectorToItem, transform.forward));

                if (smallestAngle > angleFromCameraToItem)
                {
                    smallestAngle = angleFromCameraToItem;
                    currentIndex = i;
                }
            }

            return currentIndex;
        }
    }
}
