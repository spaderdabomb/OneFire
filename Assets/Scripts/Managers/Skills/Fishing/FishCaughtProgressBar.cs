using JSAM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GinjaGaming.FinalCharacterController;

public class FishCaughtProgressBar : MonoBehaviour
{
    [SerializeField] private GameObject fishCaughtProgressBar;
    [SerializeField] private GameObject hookZone;
    [SerializeField] private GameObject fishCaughtIndicator;
    [SerializeField] private float indicatorPaddingPercent = 0.08f;
    [SerializeField] private float baseSpeedIndicator = 1f;
    [SerializeField] private float pitchShift = 0.05f;
    [SerializeField] private float progressFlashTime = 0.3f;
    [SerializeField] private Color progressFlashColor;

    public Image FishCaughtProgressBarImage { get; private set; }

    private RectTransform _fishCaughtIndicatorRect;
    private RectTransform _hookZoneRect;
    private float _indicatorLeftPos;
    private float _indicatorRightPos;
    private float _timeCounter = 0f;
    private float _startingPitch = 1f;
    private float _currentPitch = 1f;
    private float _progressFlashTimer = 0f;
    private bool _flashingProgressColor = false;
    private Color _progressFlashStartColor;
    private GameObject _successParticles = null; 
    private Vector3 _lastHookZonePosition = Vector3.zero;

    private void Awake()
    {
        FishCaughtProgressBarImage = fishCaughtProgressBar.GetComponent<Image>(); 
        _fishCaughtIndicatorRect = fishCaughtIndicator.GetComponent<RectTransform>();
        _hookZoneRect = hookZone.GetComponent<RectTransform>();

        _progressFlashStartColor = FishCaughtProgressBarImage.color;

        _indicatorLeftPos = -fishCaughtProgressBar.GetComponent<RectTransform>().rect.width / 2 * (1f - indicatorPaddingPercent);
        _indicatorRightPos = fishCaughtProgressBar.GetComponent<RectTransform>().rect.width / 2 * (1f - indicatorPaddingPercent);

        _fishCaughtIndicatorRect.localPosition = new Vector3(_indicatorLeftPos, _fishCaughtIndicatorRect.localPosition.y, _fishCaughtIndicatorRect.localPosition.z);
    }

    private void Start()
    {
        FishCaughtProgressBarImage.fillAmount = 0f;

        SoundFileObject sfo = AudioManager.GetSoundSafe(MainLibrarySounds.Success);
        sfo.startingPitch = _startingPitch;
    }

    private void Update()
    {
        UpdateUI();

        if (_flashingProgressColor)
        {
            float lerpFactor = Mathf.PingPong(2 * _progressFlashTimer / progressFlashTime, 1.0f);
            Color currentColor = Color.Lerp(_progressFlashStartColor, progressFlashColor, lerpFactor);
            FishCaughtProgressBarImage.color = currentColor;

            _progressFlashTimer += Time.deltaTime;
            _flashingProgressColor = _progressFlashTimer < progressFlashTime;
        }
    }

    private void LateUpdate()
    {
        if (_successParticles != null && _lastHookZonePosition != Vector3.zero)
        {
            Vector3 spawnPosition = GetUIWorldPoint(_lastHookZonePosition);
            Quaternion rotation = GetUIWorldRotation(_lastHookZonePosition);
            _successParticles.transform.position = spawnPosition;
            _successParticles.transform.rotation = rotation;
        }
    }

    public void ReelFishPressed()
    {
        if (InHookZone())
            HitZone();
        else
            MissedZone();

        FishCaughtProgressBarImage.fillAmount = Mathf.Clamp(FishCaughtProgressBarImage.fillAmount, 0f, 1f);
    }

    private Vector3 GetUIWorldPoint(Vector3 centerPosition)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, centerPosition);
        Ray ray = GameObjectManager.Instance.uiCamera.ScreenPointToRay(screenPoint);
        Vector3 spawnPosition = ray.origin + ray.direction * 10f;

        return spawnPosition;
    }

    private Quaternion GetUIWorldRotation(Vector3 centerPosition)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, centerPosition);
        Ray ray = GameObjectManager.Instance.uiCamera.ScreenPointToRay(screenPoint);
        Vector3 spawnPosition = ray.origin + ray.direction * 10f;
        Quaternion rotation = Quaternion.LookRotation(ray.direction);

        return rotation;
    }

    private void HitZone()
    {
        bool onLastSegment = OnLastSegment();
        FishCaughtProgressBarImage.fillAmount += 1f / FishingManager.Instance.CurrentFish.catchSegments;
        GameObjectManager.Instance.playerAnimation.SetReelingTrigger();

        // Spawn success burst effect at correct point
        _lastHookZonePosition = _hookZoneRect.TransformPoint(_hookZoneRect.rect.center);
        Vector3 spawnPosition = GetUIWorldPoint(_lastHookZonePosition);
        Quaternion rotation = GetUIWorldRotation(_lastHookZonePosition);
        _successParticles = Instantiate(FishingManager.Instance.successBurstYellowEffect, spawnPosition, rotation);

        // Audio
        AudioManager.PlaySound(MainLibrarySounds.ReelFishShort);
        SoundFileObject sfo = AudioManager.GetSoundSafe(MainLibrarySounds.Success);
        AudioManager.PlaySound(sfo);
        _currentPitch += pitchShift;
        sfo.startingPitch = _currentPitch;

        _progressFlashTimer = 0f;
        _flashingProgressColor = true;

        if (!onLastSegment)
            NewZonePosition();
    }

    private void NewZonePosition()
    {
        float minPosition = _indicatorLeftPos + _hookZoneRect.rect.width / 2;
        float maxPosition = _indicatorRightPos - _hookZoneRect.rect.width / 2;
        float randomX = Random.Range(minPosition, maxPosition);
        _hookZoneRect.localPosition = new Vector3(randomX, _hookZoneRect.localPosition.y, _hookZoneRect.localPosition.z);
    }

    private void MissedZone()
    {
        AudioManager.PlaySound(MainLibrarySounds.Miss);
        ItemData activeItem = GameObjectManager.Instance.playerEquippedItem.ActiveItemData;

        if (activeItem is FishingRodData)
        {
            FishingRodData fishingRod = (FishingRodData)activeItem;
            FishingManager.Instance.fishHookedProgressBar.GetComponent<FishHookedProgressBar>().DeltaTimeRemaining(fishingRod.missPenaltyTime);
        }
        else
        {
            Debug.LogWarning("In fishing minigame but fishing rod not equipped - bug alert!");
        }
    }

    public bool InHookZone()
    {
        float leftZonePos = _hookZoneRect.localPosition.x - _hookZoneRect.rect.width / 2;
        float rightZonePos = _hookZoneRect.localPosition.x + _hookZoneRect.rect.width / 2;

        return _fishCaughtIndicatorRect.localPosition.x > leftZonePos && _fishCaughtIndicatorRect.localPosition.x < rightZonePos;
    }

    public bool OnLastSegment()
    {
        float segmentLength = 1f / FishingManager.Instance.CurrentFish.catchSegments;
        float fillAmountRemaining = 1f - FishCaughtProgressBarImage.fillAmount;

        return fillAmountRemaining <= segmentLength;
    }

    private void UpdateUI()
    {
        if (GameObjectManager.Instance.playerState.CurrentPlayerFishingState == PlayerFishingState.FishCaught)
            return;

        _timeCounter += Time.deltaTime * baseSpeedIndicator;
        float t = (Mathf.Sin(_timeCounter) + 1.0f) / 2.0f;

        Vector3 targetPosLeft = new Vector3(_indicatorLeftPos, _fishCaughtIndicatorRect.localPosition.y, _fishCaughtIndicatorRect.localPosition.z);
        Vector3 targetPosRight = new Vector3(_indicatorRightPos, _fishCaughtIndicatorRect.localPosition.y, _fishCaughtIndicatorRect.localPosition.z);

        _fishCaughtIndicatorRect.localPosition = Vector3.Lerp(targetPosLeft, targetPosRight, t);
    }

    private void OnDestroy()
    {
        SoundFileObject sfo = AudioManager.GetSoundSafe(MainLibrarySounds.Success);
        sfo.startingPitch = _startingPitch;
    }
}
