using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class ImageFader : MonoBehaviour
{
    public Image targetImage;
    public float fadeDuration = 1f;
    public float holdDuration = 1f; // Time to stay fully visible before fading out
    public UnityEvent onFadeInComplete;

    void Reset()
    {
        targetImage = GetComponent<Image>();
    }

    private void Start()
    {
        FadeInThenEventThenFadeOut();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeImage(0f, 1f));
    }

    public void FadeOut()
    {
        StartCoroutine(FadeImage(1f, 0f));
    }

    public void FadeInThenEventThenFadeOut()
    {
        StartCoroutine(FadeInEventFadeOutRoutine());
    }

    private IEnumerator FadeImage(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = targetImage.color;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            targetImage.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }

    private IEnumerator FadeInEventFadeOutRoutine()
    {
        yield return StartCoroutine(FadeImage(0f, 1f));
        onFadeInComplete?.Invoke();
        yield return new WaitForSeconds(holdDuration);
        yield return StartCoroutine(FadeImage(1f, 0f));
    }
}
