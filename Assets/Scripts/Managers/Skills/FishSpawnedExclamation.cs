using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(FaceCamera), typeof(Animator))]
public class FishSpawnedExclamation : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    private void OnEnable()
    {
        FishingManager.Instance.OnFishSpawned += FishSpawned;
        FishingManager.Instance.OnFishHooked += FishHooked;
        FishingManager.Instance.OnStopFishing += StoppedFishing;
    }

    private void OnDisable()
    {
        FishingManager.Instance.OnFishSpawned -= FishSpawned;
        FishingManager.Instance.OnFishHooked -= FishHooked;
        FishingManager.Instance.OnStopFishing -= StoppedFishing;
    }

    private void FishSpawned(FishData fishData, GameObject currentBob)
    {
        transform.position = currentBob.transform.position + offset;
    }

    private void FishHooked(FishData fishData)
    {
        DestroySelf();
    }

    private void StoppedFishing()
    {
        DestroySelf();
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
