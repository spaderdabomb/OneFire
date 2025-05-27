using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggersInTrigger : MonoBehaviour
{
    public List<Collider> triggerList;
    public event Action<Collider> OnTriggerInTriggerEntered;
    private WorldItem worldItem;

    private void Awake()
    {
        triggerList = new List<Collider>();
        worldItem = GetComponent<WorldItem>();
    }

    private void OnEnable()
    {
        WorldItem.OnWorldItemDestroyed += RemoveGameobjectFromList;
    }

    private void OnDisable()
    {
        WorldItem.OnWorldItemDestroyed -= RemoveGameobjectFromList;
    }

    public void RemoveGameobjectFromList(GameObject gameObj)
    {
        int removeIndex = -1;
        for (int i = 0; i < triggerList.Count; i++)
        {
            if (triggerList[i].gameObject == gameObj)
            {
                removeIndex = i;
                break;
            }
        }

        if (removeIndex != -1)
        {
            triggerList.RemoveAt(removeIndex);
        }
    }

    public Collider IsGameObjectInTriggerList(GameObject gameObj)
    {
        int removeIndex = -1;
        for (int i = 0; i < triggerList.Count; i++)
        {
            if (triggerList[i].gameObject == gameObj)
            {
                removeIndex = i;
                break;
            }
        }

        return removeIndex != -1 ? triggerList[removeIndex] : null;
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (!triggerList.Contains(trigger) && trigger.isTrigger && trigger.gameObject.GetComponent<WorldItem>() != null)
        {
            triggerList.Add(trigger);
            OnTriggerInTriggerEntered?.Invoke(trigger);
        }
    }

    void OnTriggerExit(Collider trigger)
    {
        if (triggerList.Contains(trigger))
        {
            triggerList.Remove(trigger);
        }
    }

    public void RemoveItem(Collider trigger)
    {
        triggerList.Remove(trigger);
    }
}
