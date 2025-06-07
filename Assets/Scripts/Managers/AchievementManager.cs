using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;
    
    [Header("Animation Parameters")]
    [SerializeField] private float displayDuration = 2.5f;

    [SerializeField] private float easeInDuration = 0.4f;
    [SerializeField] private float bounceMagnitude = 1f;
    [SerializeField] private float bounceDuration = 0.1f;
    [SerializeField] private float dropDistance = 10f;
    
    [Header("Effects")]
    [SerializeField] private GameObject iconGlowEffect;
    [SerializeField] private GameObject sparklesEffect;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject fishingTrophyPrefab;
    
    [Header("Misc")]
    [SerializeField] private float delayAfterFishCaught = 2f;

    private GameObject _activeBg;
    private GameObject _activeIcon;
    private GameObject _activeText;

    private Queue<FishData> _fishTrophyQueue = new();
    private bool _isAnimating = false;
    
    private float accumulatedTime = 0;
    [SerializeField] private FishData tempFishData;
    
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        FishingManager.Instance.OnNewFishCaught += ShowNewFishCaught;
    }

    private void OnDisable()
    {
        FishingManager.Instance.OnNewFishCaught -= ShowNewFishCaught;
    }

    // void Update()
    // {
    //     accumulatedTime += Time.deltaTime;
    //     if (accumulatedTime >= 3f)
    //     {
    //         ShowNewFishCaught(tempFishData);
    //         accumulatedTime = 0;
    //     }
    //
    // }

    public void ShowNewFishCaught(FishData fishdata)
    {
        _fishTrophyQueue.Enqueue(fishdata);

        if (!_isAnimating)
            StartCoroutine(ProcessFishQueue());
    }

    private IEnumerator ProcessFishQueue()
    {
        _isAnimating = true;

        while (_fishTrophyQueue.Count > 0)
        {
            FishData nextFish = _fishTrophyQueue.Dequeue();
            yield return ShowNewFishCaughtAfterDelayCoroutine(nextFish, delayAfterFishCaught);  
        }

        _isAnimating = false;
    }
    
    private IEnumerator ShowNewFishCaughtAfterDelayCoroutine(FishData fishdata, float delay)
    {
        yield return new WaitForSeconds(delay);
        ShowNewFishCaughtAfterDelay(fishdata);
        yield return new WaitForSeconds(easeInDuration + 2 * bounceDuration + displayDuration);
    }
    

    private void ShowNewFishCaughtAfterDelay(FishData fishData)
    {
        GameObject trophyGO = Instantiate(fishingTrophyPrefab, transform);
        trophyGO.GetComponent<FishingTrophy>().Init(fishData);
        RectTransform rt = trophyGO.GetComponent<RectTransform>();

        Vector2 originalPos = rt.anchoredPosition;
        Vector2 dropPos = originalPos + Vector2.down * dropDistance;
        Vector2 bounceUp = originalPos + Vector2.up * bounceMagnitude;

        StartCoroutine(PlayTrophyAnimation(rt, originalPos, dropPos, bounceUp));
    }
    
    private IEnumerator PlayTrophyAnimation(RectTransform rt, Vector2 originalPos, Vector2 dropPos, Vector2 bounceUp)
    {
        Vector2 dropDipPos = dropPos + Vector2.down * bounceMagnitude;

        // Ease In: originalPos → dropDipPos
        float t = 0f;
        while (t < easeInDuration)
        {
            t += Time.deltaTime;
            float progress = t / easeInDuration;
            rt.anchoredPosition = Vector2.Lerp(originalPos, dropDipPos, EaseInCubic(progress));
            yield return null;
        }
        rt.anchoredPosition = dropDipPos;

        // Bounce Up: dropDipPos → dropPos
        t = 0f;
        while (t < bounceDuration)
        {
            t += Time.deltaTime;
            float progress = t / bounceDuration;
            rt.anchoredPosition = Vector2.Lerp(dropDipPos, dropPos, EaseOutCubic(progress));
            yield return null;
        }
        rt.anchoredPosition = dropPos;

        // Wait at dropPos
        yield return new WaitForSeconds(displayDuration);

        // Final Rise: dropPos → originalPos
        t = 0f;
        while (t < easeInDuration)
        {
            t += Time.deltaTime;
            float progress = t / bounceDuration;
            rt.anchoredPosition = Vector2.Lerp(dropPos, originalPos, EaseInCubic(progress));
            yield return null;
        }
        rt.anchoredPosition = originalPos;

        Destroy(rt.gameObject);
    }
    
    private float EaseOutCubic(float x) => 1f - Mathf.Pow(1f - x, 3);
    private float EaseInCubic(float x) => x * x * x;
}
