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

    [SerializeField] private Color startColor;
    [SerializeField] private Color halfwayColor;
    [SerializeField] private Color lastQuarterColor;

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

        if (timeRemainingProgressBar.fillAmount <= 0.25f)
        {
            timeRemainingProgressBar.material.color = lastQuarterColor;
            timeRemainingProgressBar.material.SetColor("_EmissionColor", lastQuarterColor * 5.0f);

        }
        else if (timeRemainingProgressBar.fillAmount <= 0.5f)
        {
            timeRemainingProgressBar.material.color = halfwayColor;
            timeRemainingProgressBar.material.SetColor("_EmissionColor", halfwayColor * 2.5f);
        }
        else
        {
            timeRemainingProgressBar.material.color = startColor;
            timeRemainingProgressBar.material.SetColor("_EmissionColor", startColor * 3.0f);
        }
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
