using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIFader : MonoBehaviour
{
    public float fadeDuration = 1f;

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