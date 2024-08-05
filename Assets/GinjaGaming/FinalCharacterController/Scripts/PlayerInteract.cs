using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GinjaGaming.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
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
            if (objectIndex == -1)
                return;

            GameObject currentInteractingObject = interactingObjects[objectIndex];
            if (currentInteractingObject.GetComponent<WorldItem>() != null)
            {
                currentInteractingObject.GetComponent<WorldItem>().PickUpItem();
            }
            else if (currentInteractingObject.GetComponent<WorldStructure>() != null)
            {
                currentInteractingObject.GetComponent<WorldStructure>().InteractWithStructure();
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
                string interactingObjectPretext = interactingObjects[currentInteractObjectIndex].GetComponent<InteractingObject>().DisplayPretext;
                string interactingObjectName = interactingObjects[currentInteractObjectIndex].GetComponent<InteractingObject>().DisplayString;
                interactPopup.ShowPopup(interactingObjectPretext, interactingObjectName, GameDataManager.Instance.standardInteractTextColor);
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
