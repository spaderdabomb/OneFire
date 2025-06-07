using GinjaGaming.FinalCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishHookedProgressBar : MonoBehaviour
{
    private float _startTime = -1f;
    private float _timeRemaining = -1f;
    private bool _fishCaught = false;

    [SerializeField] private Image timeRemainingProgressBar;


    private void Awake()
    {
        _startTime = FishingManager.Instance.CurrentFish.timeToEscape;
        _timeRemaining = _startTime;
    }
    
    private void OnEnable()
    {
        FishingManager.Instance.OnFishCaught += FishCaught;
    }

    private void OnDisable()
    {
        FishingManager.Instance.OnFishCaught -= FishCaught;
    }

    private void FishCaught(FishData fishData)
    {
        _fishCaught = true;
    }

    private void Update()
    {
        if (_fishCaught)
            return;

        _timeRemaining -= Time.deltaTime;
        timeRemainingProgressBar.fillAmount = _timeRemaining / _startTime;

        if (timeRemainingProgressBar.fillAmount <= 0f)
        {
            FishingManager.Instance.StopFishing();
        }
    }

    public void DeltaTimeRemaining(float delta)
    {
        _timeRemaining -= delta;
    }
}
