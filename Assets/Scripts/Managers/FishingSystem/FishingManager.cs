using GinjaGaming.FinalCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using OneFireUi;
using Sirenix.OdinInspector;
using JSAM;
using System.Linq;

[DefaultExecutionOrder(-3)]
public class FishingManager : SerializedMonoBehaviour, IPersistentData
{
    public static FishingManager Instance;

    [Header("Debug")] 
    [SerializeField] private bool clearData;

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
    public GameObject newFishCaughtEffect;
    public Dictionary<ItemRarity, ParticleSystem> fishOnHookRarityEffectDict;

    [Header("UI")]
    public GameObject fishHookedProgressBarPrefab;
    public GameObject fishCaughtProgressBarPrefab;

    [Header("WorldObjects")] 
    public Camera uiWorldCamera;
    public Transform currentFishingPositionTransform;

    [HideInInspector] public GameObject fishCaughtProgressBar = null;
    [HideInInspector] public GameObject fishHookedProgressBar = null;
    
    [HideInInspector] public int TotalFishCaught { get; set; } = 0;
    [HideInInspector] public Dictionary<string, bool> FishCaughtDict { get; set; } = new Dictionary<string, bool>();
    [HideInInspector] public Dictionary<string, DateTime> FishDateCaughtDict { get; set; } = new Dictionary<string, DateTime>();
    [HideInInspector] public Dictionary<string, int> FishTotalCaughtDict { get; set; } = new Dictionary<string, int>();
    [HideInInspector] public Dictionary<string, float> FishCaughtBestWeightDict { get; set; } = new Dictionary<string, float>();

    private ParticleSystem _currentFishOnHookEffect;
    private ParticleSystem _currentFishOnHookRarityEffect;

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
        
        if (clearData)
            ClearData();
        else
            LoadData();
        
        AddToSaveable();
        
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
        _currentFishOnHookRarityEffect = Instantiate(fishOnHookRarityEffectDict[CurrentFish.itemRarity], _currentFishPosition + 0.3f * Vector3.up, Quaternion.identity, GameObjectManager.Instance.effectsContainer.transform);

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

        if (!FishCaughtDict.ContainsKey(CurrentFish.fishID))
        {
            NewFishCaught(CurrentFish);
            CatchAllLowerTierFish(CurrentFish);
            AudioManager.PlaySound(MainLibrarySounds.zenfisher_fishcaught_01);
            Instantiate(newFishCaughtEffect, uiWorldCamera.transform);
        }

        TotalFishCaught += 1;
        IncrementOrCreateKey(FishTotalCaughtDict, CurrentFish.fishID);
    }
    
    private void NewFishCaught(FishData fishData)
    {
        FishCaughtDict.TryAdd(fishData.fishID, true);
        FishDateCaughtDict.TryAdd(fishData.fishID, DateTime.Now);
        IncrementOrCreateKey(FishTotalCaughtDict, fishData.fishID);
        UpdateIfBiggerOrCreateKeyWithFloatValue(FishCaughtBestWeightDict, fishData.fishID, fishData.weight);
        OnNewFishCaught?.Invoke(CurrentFish);
    }
    
    private void CatchAllLowerTierFish(FishData fishData)
    {
        for (int i = (int)fishData.itemRarity - 1; i >= 0; i--)
        {
            string fishID = fishData.baseName + (ItemRarity)i;
            if (i < (int)fishData.itemRarity && !FishCaughtDict.ContainsKey(fishID))
            {
                FishData newFish = fishData.CreateNew((ItemRarity)i);
                CurrentFish = newFish;
                NewFishCaught(CurrentFish);
                continue;
            }

            break;
        }
    }

    public void CastRod(float castPower)
    {
        StartCoroutine(SpawnBob());
    }

    private FishData SpawnFishFromLake(WorldLake worldLake)
    {
        FishData randomFish = GetRandomFishFromLake(worldLake);
        ItemRarity randomRarity = GetRandomRarity();

        FishData newFish = randomFish.CreateNew(randomRarity);
        newFish.weight = Random.Range(newFish.weightRangeLow, newFish.weightRangeHigh);

        return newFish;
    }

    public FishData GetRandomFishFromLake(WorldLake worldLake)
    {
        float totalRate = worldLake.lakeData.fishInLake.Sum(fishData => fishData.catchRate);
        float randomRateValue = UnityEngine.Random.Range(0f, totalRate);
        
        float cumulativeRate = 0f;
        foreach (FishData fishData in worldLake.lakeData.fishInLake)
        {
            cumulativeRate += fishData.catchRate;
            if (randomRateValue <= cumulativeRate)
                return fishData;           
        }
        
        Debug.Log("Spawning null fish - this is bad");
        return null;
    }
    
    public ItemRarity GetRandomRarity()
    {
        float totalWeight = rarityWeights.Values.Sum();
        float randomWeight = UnityEngine.Random.Range(0f, totalWeight);

        float cumulative = 0f;
        foreach (var pair in rarityWeights)
        {
            cumulative += pair.Value;
            if (randomWeight <= cumulative)
                return pair.Key;
        }

        Debug.Log("Fallback rarity - this is bad");
        return rarityWeights.Keys.FirstOrDefault();
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
            bobRb.linearVelocity = newDirection * fishingPower;

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

        GameObject rarityEffectObj = _currentFishOnHookRarityEffect != null ? _currentFishOnHookRarityEffect.gameObject : null;

        DestroyAndSetNull(ref _currentFishingBob);
        DestroyAndSetNull(ref _currentFishOnHookEffect);
        DestroyAndSetNull(ref rarityEffectObj);
        DestroyAndFade(ref fishCaughtProgressBar);
        DestroyAndFade(ref fishHookedProgressBar);

        AudioManager.StopSound(MainLibrarySounds.FishOnHookNormal);

        OnStopFishing?.Invoke();
    }

    private void DestroyAndSetNull(ref GameObject obj)
    {
        if (obj == null)
            return;
        
        Destroy(obj);
        obj = null;
    }

    private void DestroyAndSetNull(ref ParticleSystem obj)
    {
        if (obj == null)
            return;
        
        Destroy(obj);
        obj = null;
    }

    private void DestroyAndFade(ref GameObject obj)
    {
        if (obj == null)
            return;
        
        obj.TryGetComponent(out UIFader uiFader);
        {
            uiFader.FadeAndDestroy();
            obj = null;
        }
    }
    
    #region Persistent Data Utils
    public void AddToSaveable()
    {
        PersistentDataManager.Instance.AddToPersistentDataList(this);
    }

    public void LoadData()
    {
        TotalFishCaught = ES3.Load(nameof(TotalFishCaught), defaultValue: TotalFishCaught);
        FishCaughtDict = ES3.Load(nameof(FishCaughtDict), defaultValue: FishCaughtDict);
        FishDateCaughtDict = ES3.Load(nameof(FishDateCaughtDict), defaultValue: FishDateCaughtDict);
        FishTotalCaughtDict = ES3.Load(nameof(FishTotalCaughtDict), defaultValue: FishTotalCaughtDict);
        FishCaughtBestWeightDict = ES3.Load(nameof(FishCaughtBestWeightDict), defaultValue: FishCaughtBestWeightDict); //
    }

    public void SaveData()
    {
        ES3.Save(nameof(TotalFishCaught), TotalFishCaught);
        ES3.Save(nameof(FishCaughtDict), FishCaughtDict);
        ES3.Save(nameof(FishDateCaughtDict), FishDateCaughtDict);
        ES3.Save(nameof(FishTotalCaughtDict), FishTotalCaughtDict);
        ES3.Save(nameof(FishCaughtBestWeightDict), FishCaughtBestWeightDict);
    }

    public void ClearData()
    {
        TotalFishCaught = 0;
        FishCaughtDict.Clear();
        FishDateCaughtDict.Clear();
        FishTotalCaughtDict.Clear();
        FishCaughtBestWeightDict.Clear();

        SaveData();
    }
    
    public int GetTotalFishCollected()
    {
        return FishCaughtDict.Count;
    }

    public bool IsFishCaught(string fishID)
    {
        return FishCaughtDict.TryGetValue(fishID, out bool fishCaught) ? fishCaught : false;
    }

    public bool IsFishTypeCaught(FishData fishData)
    {
        string fishName = fishData.baseName;
        foreach (ItemRarity itemRarity in Enum.GetValues(typeof(ItemRarity)))
        {
            string currentFishID = fishName + itemRarity;
            if (FishCaughtDict.TryGetValue(currentFishID, out bool fishCaught))
            {
                return fishCaught;
            }
        }

        return false;
    }

    // public int GetFishCollectedInBiome(BiomeType biomeType)
    // {
    //     List<FishData> biomeFish = BiomeExtensions.GetAllFishTypesInBiomes()[biomeType];
    //
    //     return biomeFish.Count(fish => FishCaughtDict.TryGetValue(fish.fishID, out bool caught) && caught);
    // }
    
    public int GetFishCollectedInBiome(BiomeType biomeType)
    {
        int fishCollectedInBiome = 0;
        List<FishData> biomeFishData = BiomeExtensions.GetAllFishTypesInBiomes()[biomeType];
        foreach (FishData biomeFish in  biomeFishData)
        {
            foreach (ItemRarity itemRarity in Enum.GetValues(typeof(ItemRarity)))
            {
                string currentFishID = biomeFish.baseName + itemRarity;
                if (FishCaughtDict.TryGetValue(currentFishID, out bool fishCaught))
                {
                    if (fishCaught)
                    {
                        fishCollectedInBiome++;
                    }
                }
            }
        }

        return fishCollectedInBiome;
    }
    
    public static void IncrementOrCreateKey(Dictionary<string, int> dictionary, string key)
    {
        dictionary[key] = dictionary.GetValueOrDefault(key) + 1;
    }

    static void UpdateIfBiggerOrCreateKeyWithFloatValue(Dictionary<string, float> dictionary, string key, float newValue)
    {
        if (dictionary.ContainsKey(key))
        {
            if (newValue > dictionary[key])
            {
                dictionary[key] = newValue;
            }
        }
        else
        {
            dictionary[key] = newValue;
        }
    }
    #endregion
}
