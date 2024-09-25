using GinjaGaming.FinalCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance;

    public GameObject fishingBobPrefab;
    public GameObject _currentFishingBob = null;

    [SerializeField] float _bobAngularSpeed = 7f;

    public event Action OnFishCaught;
    public event Action OnStopFishing;

    private void Awake()
    {
        Instance = this;
    }

    public void BobHitWater()
    {

    }

    public void FishCaught()
    {

    }

    public void CastRod(float castPower)
    {
        _currentFishingBob = Instantiate(fishingBobPrefab, GameObjectManager.Instance.effectsContainer.transform);
        GameObject currentFishingRod = GameObjectManager.Instance.playerEquippedItem.ActiveItemObject;

        FishingRodHeld fishingRodHeld;
        if (currentFishingRod.TryGetComponent(out fishingRodHeld))
        {
            _currentFishingBob.transform.position = fishingRodHeld.bobSpawnLocation.transform.position;
            float fishingPower = GameObjectManager.Instance.playerStats.GetFishingPower();

            Rigidbody bobRb = _currentFishingBob.GetComponent<Rigidbody>();

            Camera playerCamera = GameObjectManager.Instance.playerCamera;
            float deltaFromHorizontal = 90f - Vector3.Angle(playerCamera.transform.forward, Vector3.up);
            float angleFromLookDirection = Mathf.Clamp(-45f - deltaFromHorizontal, -60f, -30f);

            Vector3 originalDirection = Vector3.ProjectOnPlane(playerCamera.transform.forward, Vector3.up);
            originalDirection.Normalize();

            Vector3 rightInXZPlane = Vector3.Cross(Vector3.up, originalDirection);
            Quaternion rotation = Quaternion.AngleAxis(angleFromLookDirection, rightInXZPlane);
            Vector3 newDirection = rotation * originalDirection;

            bobRb.angularVelocity = new Vector3(Random.Range(0f, _bobAngularSpeed), Random.Range(0f, _bobAngularSpeed), Random.Range(0f, _bobAngularSpeed));
            bobRb.velocity = newDirection * fishingPower;
        }
        else
        {
            Debug.Log("Casted rod but active item is not a fishing rod");
        }
    }

    public void StopFishing()
    {
        GameObjectManager.Instance.playerState.SetPlayerActionState(PlayerActionState.None);
        GameObjectManager.Instance.playerState.SetPlayerFishingState(PlayerFishingState.None);
        OnStopFishing?.Invoke();
    }
}
