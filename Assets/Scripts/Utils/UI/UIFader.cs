using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    public float fadeDuration = 0.75f;
    public bool fadeIn;
    public bool FadeInFinished {get; private set;}
    
    public event Action OnFadeInFinished;

    private void Start()
    {
        if (fadeIn)
            FadeIn();
        else
            FadeInDone();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInAnimation());

    }
    private IEnumerator FadeInAnimation()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        FadeInDone();
    }

    private void FadeInDone()
    {
        FadeInFinished = true;
        OnFadeInFinished?.Invoke();
    }

    public void FadeAndDestroy()
    {
        StartCoroutine(FadeOutThenDestroy());
    }

    private IEnumerator FadeOutThenDestroy()
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        float startAlpha = canvasGroup.alpha;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0;
        Destroy(gameObject);
    }
}