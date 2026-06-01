using System.Collections;
using UnityEngine;

public class ScreenFader : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    private void Awake()
    {
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }

    public IEnumerator FadeOut(float duration)
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeIn(float duration)
    {
        if (canvasGroup == null)
        {
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }
}