using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1)]
public class GameObjectManager : MonoBehaviour
{
    public static GameObjectManager Instance;

    public GameObject player;
    public Camera playerCamera; 
    public PlayerInteract playerInteract;

    [Header("Containers")]
    public GameObject itemContainer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        InventoryManager.Instance.OnDroppedItem += SpawnItem;
    }

    private void OnDisable()
    {
        InventoryManager.Instance.OnDroppedItem -= SpawnItem;
    }

    public void SpawnItem(ItemData itemData)
    {
        GameObject newItemSpawned = Instantiate(itemData.item3DPrefab, itemContainer.transform);
        ItemSpawned newItemSpanwedInst = newItemSpawned.GetComponent<ItemSpawned>();
        newItemSpanwedInst.InitItemData();
        newItemSpanwedInst.SetStackCount(itemData.stackCount);

        float speedScaleFactor = 2f;
        float angularSpeed = 7f;
        newItemSpawned.transform.position = playerCamera.transform.position;
        Rigidbody newItemRb = newItemSpawned.GetComponent<Rigidbody>();

        newItemRb.angularVelocity = new Vector3(Random.Range(0f, angularSpeed), Random.Range(0f, angularSpeed), Random.Range(0f, angularSpeed));
        newItemRb.velocity = new Vector3(
            speedScaleFactor * Mathf.Sign(playerCamera.transform.forward.x) * Random.Range(0f, Mathf.Abs(playerCamera.transform.forward.x)), 
            Random.Range(0.5f, 2f),
            speedScaleFactor * Mathf.Sign(playerCamera.transform.forward.z) * Random.Range(0f, Mathf.Abs(playerCamera.transform.forward.z))
            );

    }
}