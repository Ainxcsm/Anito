using TMPro;
using UnityEngine;

public class DamageTextPopup : MonoBehaviour
{
    public TMP_Text damageText;

    [Header("Movement")]
    public float moveSpeed = 80f;
    public float lifetime = 0.8f;

    [Header("Scale")]
    public float startScale = 2f;
    public float endScale = 1.4f;

    [Header("Text Visual")]
    public float fontSize = 48f;
    public Color textColor = Color.white;

    private float timer;
    private Color originalColor;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (damageText == null)
        {
            damageText = GetComponent<TMP_Text>();
        }

        if (damageText == null)
        {
            Debug.LogError("DamageTextPopup has no TMP_Text component.");
            return;
        }

        damageText.text = "0";
        damageText.fontSize = fontSize;
        damageText.color = textColor;
        damageText.alignment = TextAlignmentOptions.Center;
        damageText.raycastTarget = false;

        originalColor = damageText.color;

        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one * startScale;
        }
        else
        {
            transform.localScale = Vector3.one * startScale;
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        }

        float progress = timer / lifetime;

        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, progress);
        }
        else
        {
            transform.localScale = Vector3.Lerp(Vector3.one * startScale, Vector3.one * endScale, progress);
        }

        if (damageText != null)
        {
            Color color = originalColor;
            color.a = Mathf.Lerp(1f, 0f, progress);
            damageText.color = color;
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(float damage)
    {
        if (damageText == null)
        {
            damageText = GetComponent<TMP_Text>();
        }

        if (damageText == null)
        {
            Debug.LogError("DamageTextPopup Setup failed. Missing TMP_Text.");
            return;
        }

        damageText.text = Mathf.RoundToInt(damage).ToString();
        damageText.fontSize = fontSize;
        damageText.color = textColor;
        damageText.alignment = TextAlignmentOptions.Center;
        damageText.raycastTarget = false;

        originalColor = damageText.color;

        Debug.Log("DamageTextPopup setup complete: " + damageText.text);
    }
}