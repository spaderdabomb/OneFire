using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

namespace GinjaGaming.FinalCharacterController
{
    [DefaultExecutionOrder(-1)]
    public class PlayerInteract : MonoBehaviour
    {
        public List<GameObject> interactingObjects;
        public List<GameObject> damageableObjects;
        private int currentInteractObjectIndex = -1;
        private int currentDamageableObjectIndex = -1;
        private InteractPopup interactPopup;

        private void OnEnable()
        {
            InputManager.Instance.playerActionsInput.onInteract += PickUpItem;
            InputManager.Instance.playerActionsInput.OnPunchHit += Damage;
        }

        private void OnDisable()
        {
            InputManager.Instance.playerActionsInput.onInteract -= PickUpItem;
            InputManager.Instance.playerActionsInput.OnPunchHit -= Damage;
        }

        private void Start()
        {
            interactingObjects = new List<GameObject>();
            damageableObjects = new List<GameObject>();
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

        private void Damage()
        {
            int objectIndex = GetBestDamageableObjectIndex();
            if (objectIndex == -1)
                return;

            GameObject currentInteractingObject = damageableObjects[objectIndex];
            if (currentInteractingObject.GetComponent<DamageableObject>() != null)
            {
                currentInteractingObject.GetComponent<DamageableObject>().Damage();
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

        public void AddDamageableObject(GameObject newObject)
        {
            damageableObjects.Add(newObject);
        }

        public void RemoveDamageableObject(GameObject newObject)
        {
            damageableObjects.Remove(newObject);
        }

        private void UpdateInteractUI()
        {
            if (interactingObjects.Count > 0)
            {
                currentInteractObjectIndex = GetBestInteractingObjectIndex();
                InteractingObject interactingObject = interactingObjects[currentInteractObjectIndex].GetComponent<InteractingObject>();
                if (interactingObject.ShowPopup)
                    interactPopup.ShowPopup(interactingObject.DisplayPretext, interactingObject.DisplayString, GameDataManager.Instance.standardInteractTextColor);
            }
            else if (interactPopup.gameObject.activeSelf)
            {
                interactPopup.HidePopup();
            }
        }

        private int GetBestInteractingObjectIndex()
        {
            return GetBestObjectIndex(interactingObjects);
        }

        private int GetBestDamageableObjectIndex()
        {
            return GetBestObjectIndex(damageableObjects);
        }

        private int GetBestObjectIndex(List<GameObject> objectList)
        {
            float smallestAngle = 999f;
            int currentIndex = -1;

            for (int i = 0; i < objectList.Count; i++)
            {
                GameObject interactingObject = objectList[i];
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
