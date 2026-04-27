using UnityEngine;
using System.Collections;

public class SceneFade : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public float fadeDuration = 1.5f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        if (fadePanel == null)
        {
            Debug.LogError("FadePanel not assigned!");
            yield break;
        }

        fadePanel.alpha = 1f;

        float t = 1f;

        while (t > 0f)
        {
            t -= Time.deltaTime / fadeDuration;
            fadePanel.alpha = t;
            yield return null;
        }

        fadePanel.alpha = 0f;
    }
}