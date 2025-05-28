using GinjaGaming.FinalCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using OneFireUi;
using Sirenix.OdinInspector;
using JSAM;

public class FishingManager : SerializedMonoBehaviour
{
    public static FishingManager Instance;

    [Header("Fishing Prefabs")]
    public GameObject fishingBobPrefab;
    public GameObject fishSpawnedExclamation;

    [Header("Fishing Parameters")]
    [SerializeField] public float baseTimeToCatchFish = 10f;

    [Header("Bob Parameters")]
    [SerializeField] private float bobAngularSpeed = 7f;
    [SerializeField] private float bobShowDelayTime = 0.2f;

    [Header("Effects")]
    public ParticleSystem bobSplashEffect;
    public ParticleSystem fishSpawnedEffect;
    public ParticleSystem fishOnHookEffect;
    public GameObject successBurstYellowEffect;

    [Header("UI")]
    public GameObject fishHookedProgressBarPrefab;
    public GameObject fishCaughtProgressBarPrefab;

    [Header("WorldObjects")]
    public Transform currentFishingPositionTransform;

    [HideInInspector] public GameObject fishCaughtProgressBar = null;
    [HideInInspector] public GameObject fishHookedProgressBar = null;

    private ParticleSystem _currentFishOnHookEffect;

    private float _timeBobInWater = 0f;
    private float _nextSpawnTime = 0f;

    public Dictionary<ItemRarity, float> rarityWeights = new Dictionary<ItemRarity, float>
    {
        { ItemRarity.Common, 1.0f },
        { ItemRarity.Uncommon, 5.0f },
        { ItemRarity.Rare, 20.0f },
        { ItemRarity.Epic, 100.0f },
        { ItemRarity.Legendary, 500.0f },
        { ItemRarity.Mythic, 1000.0f }
    };

    public event Action<WorldLake> OnBobHitWater;
    public event Action<FishData> OnFishCaught;
    public event Action<FishData> OnNewFishCaught;
    public event Action<FishData, GameObject> OnFishSpawned;
    public event Action<FishData> OnFishHooked;
    public event Action OnStopFishing;

    public FishData CurrentFish { get; private set; } = null;

    private GameObject _currentFishingBob = null;
    private GameObject _currentSpawnExclamation = null;
    private WorldLake _currentWorldLake = null;
    private Vector3 _currentFishPosition = Vector3.zero;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        InventoryManager.Instance.OnHotbarItemSelectedChanged += StopFishingIfFishing;
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnHotbarItemSelectedChanged -= StopFishingIfFishing;
    }

    private void Update()
    {
        if (GameObjectManager.Instance.playerState.CurrentPlayerActionState != PlayerActionState.Fishing)
            return;

        if (GameObjectManager.Instance.playerState.CurrentPlayerFishingState == PlayerFishingState.BobInWater)
        {
            _timeBobInWater += Time.deltaTime;
            if (_timeBobInWater > _nextSpawnTime)
                FishSpawned();
        }
    }

    private float GetRandomCatchTime()
    {
        float averageTimeToCatchFish = baseTimeToCatchFish;

        return -Mathf.Log(1f - UnityEngine.Random.value) * averageTimeToCatchFish;
    }

    public void BobHitWater(WorldLake worldLake)
    {
        _timeBobInWater = 0f;
        _nextSpawnTime = GetRandomCatchTime();
        _currentWorldLake = worldLake;
        _currentFishPosition = new Vector3(_currentFishingBob.transform.position.x, worldLake.transform.position.y, _currentFishingBob.transform.position.z);
        currentFishingPositionTransform.position = _currentFishPosition;
        currentFishingPositionTransform.gameObject.SetActive(true);

        Instantiate(bobSplashEffect, _currentFishPosition, Quaternion.identity, GameObjectManager.Instance.effectsContainer.transform);

        GameObjectManager.Instance.playerState.SetPlayerFishingState(PlayerFishingState.BobInWater);
        AudioManager.PlaySound(MainLibrarySounds.BobSplashNormal);

        OnBobHitWater?.Invoke(_currentWorldLake);
    }

    public void HookFish()
    {
        GameObjectManager.Instance.playerState.SetPlayerFishingState(PlayerFishingState.FishHooked);
        GameObjectManager.Instance.playerAnimation.SetReelingTrigger();

        AudioManager.PlaySound(MainLibrarySounds.FishHooked);
        AudioManager.PlaySound(MainLibrarySounds.ReelFishHooked);

        _currentFishingBob.GetComponent<FishingBob>().Hide();
        _currentFishOnHookEffect = Instantiate(fishOnHookEffect, _currentFishPosition, fishOnHookEffect.transform.rotation, GameObjectManager.Instance.effectsContainer.transform);

        fishHookedProgressBar = Instantiate(fishHookedProgressBarPrefab, UiManager.Instance.perspectiveCanvas.transform);
        fishCaughtProgressBar = Instantiate(fishCaughtProgressBarPrefab, UiManager.Instance.perspectiveCanvas.transform);


        OnFishHooked?.Invoke(CurrentFish);
    }

    public void FishSpawned()
    {
        GameObjectManager.Instance.playerState.SetPlayerFishingState(PlayerFishingState.FishSpawned);

        CurrentFish = SpawnFishFromLake(_currentWorldLake);
        _currentSpawnExclamation = Instantiate(fishSpawnedExclamation, GameObjectManager.Instance.effectsContainer.transform);

        ParticleSystem splashParticles = Instantiate(fishSpawnedEffect, _currentFishPosition, Quaternion.identity, GameObjectManager.Instance.effectsContainer.transform);
        FishSplash fishSplash = splashParticles.GetComponent<FishSplash>();
        fishSplash.OnInstantiate(CurrentFish, _currentFishingBob);

        OnFishSpawned?.Invoke(CurrentFish, _currentFishingBob);
    }

    public void FishCaught()
    {
        AudioManager.PlaySound(MainLibrarySounds.Complete);
        OnFishCaught?.Invoke(CurrentFish);
        GameObjectManager.Instance.playerState.SetPlayerFishingState(PlayerFishingState.FishCaught);
    }

    public void CastRod(float castPower)
    {
        StartCoroutine(SpawnBob());
    }

    private FishData SpawnFishFromLake(WorldLake worldLake)
    {
        float totalWeight = 0f;

        foreach (FishData fishData in worldLake.lakeData.fishInLake)
        {
            totalWeight += rarityWeights[fishData.itemRarity];
        }

        float randomValue = UnityEngine.Random.Range(0f, totalWeight);

        float cumulativeWeight = 0f;
        foreach (FishData fishData in worldLake.lakeData.fishInLake)
        {
            float weight = rarityWeights[fishData.itemRarity];
            cumulativeWeight += weight;

            if (randomValue <= cumulativeWeight)
                return fishData;
        }

        Debug.Log("Spawning null fish - this is bad");

        return null;
    }

    public void ReelFishPressed()
    {
        FishCaughtProgressBar fishCaughtProgressBarInst = fishCaughtProgressBar.GetComponent<FishCaughtProgressBar>();
        fishCaughtProgressBarInst.ReelFishPressed();
        if (fishCaughtProgressBarInst.FishCaughtProgressBarImage.fillAmount >= 1f)
        {
            FishCaught();
        }
    }

    IEnumerator SpawnBob()
    {
        yield return new WaitForSeconds(bobShowDelayTime);

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

            bobRb.angularVelocity = new Vector3(Random.Range(0f, bobAngularSpeed), Random.Range(0f, bobAngularSpeed), Random.Range(0f, bobAngularSpeed));
            bobRb.velocity = newDirection * fishingPower;

            AudioManager.PlaySound(MainLibrarySounds.CastRod);
        }
        else
        {
            Debug.Log("Casted rod but active item is not a fishing rod");
        }
    }

    public void StopFishingIfFishing(InventorySlot slot)
    {
        if (GameObjectManager.Instance.playerState.CurrentPlayerActionState == PlayerActionState.Fishing)
            StopFishing();
    }

    public void StopFishing()
    {
        GameObjectManager.Instance.playerState.SetPlayerActionState(PlayerActionState.None);
        GameObjectManager.Instance.playerState.SetPlayerFishingState(PlayerFishingState.None);

        _currentFishPosition = Vector3.zero;
        currentFishingPositionTransform.gameObject.SetActive(false);
        currentFishingPositionTransform.position = _currentFishPosition;
        CurrentFish = null;

        DestroyAndSetNull(ref _currentFishingBob);
        DestroyAndSetNull(ref _currentFishOnHookEffect);
        DestroyAndFade(ref fishCaughtProgressBar);
        DestroyAndFade(ref fishHookedProgressBar);

        AudioManager.StopSound(MainLibrarySounds.FishOnHookNormal);

        OnStopFishing?.Invoke();
    }

    private void DestroyAndSetNull(ref GameObject obj)
    {
        Destroy(obj);
        obj = null;
    }

    private void DestroyAndSetNull(ref ParticleSystem obj)
    {
        Destroy(obj);
        obj = null;
    }

    private void DestroyAndFade(ref GameObject obj)
    {
        obj.TryGetComponent(out UIFader uiFader);
        {
            uiFader.FadeAndDestroy();
            obj = null;
        }
    }
}
