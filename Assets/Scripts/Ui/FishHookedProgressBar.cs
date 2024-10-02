using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishHookedProgressBar : MonoBehaviour
{
    public RectTransform uiElement;

    private float _startTime = -1f;
    private float _timeRemaining = -1f;

    [SerializeField] private Image timeRemainingProgressBar;
    [SerializeField] private float yoffset = 0.5f;

    private void OnEnable()
    {
        FishingManager.Instance.OnFishHooked += Initialize;
    }

    private void Initialize(FishData fishData)
    {
        _startTime = fishData.timeToEscape;
        _timeRemaining = _startTime;
    }

    private void Update()
    {
        if (timeRemainingProgressBar == null)
            return;

        _timeRemaining -= Time.deltaTime;
        timeRemainingProgressBar.fillAmount = _timeRemaining / _startTime;
    }


    void LateUpdate()   
    {
        if (FishingManager.Instance.CurrentFishPosition == Vector3.zero)
            return;

        if (uiElement != null)
        {
            Vector3 newPos = new Vector3(
                FishingManager.Instance.CurrentFishPosition.x,
                FishingManager.Instance.CurrentFishPosition.y + yoffset,
                FishingManager.Instance.CurrentFishPosition.z
                );

            uiElement.position = newPos;
        }
    }
}
