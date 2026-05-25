using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUIController : MonoBehaviour
{
    public static ItemPickUIController Instance { get; private set; }

    public GameObject popupPrefab;
    public int maxPopups = 3;
    public float popupDuration = 3f;

    private readonly Queue<GameObject> activePopups = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void ShowItemPickup(string itemName, Sprite itemIcon)
    {
        if (popupPrefab == null)
        {
            Debug.LogError("Popup Prefab is missing in ItemPickUIController.");
            return;
        }

        GameObject popup = Instantiate(popupPrefab, transform);
        popup.SetActive(true);

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = popup.AddComponent<CanvasGroup>();
        }

        canvasGroup.alpha = 1f;

        TMP_Text text = popup.GetComponentInChildren<TMP_Text>(true);

        if (text != null)
        {
            text.text = itemName;
        }
        else
        {
            Debug.LogError("No TMP_Text found inside popup prefab.");
        }

        Image icon = null;
        Transform iconTransform = popup.transform.Find("ItemIcon");

        if (iconTransform != null)
        {
            icon = iconTransform.GetComponent<Image>();
        }

        if (icon == null)
        {
            Image[] images = popup.GetComponentsInChildren<Image>(true);

            foreach (Image image in images)
            {
                if (image.gameObject.name == "ItemIcon")
                {
                    icon = image;
                    break;
                }
            }
        }

        if (icon != null)
        {
            icon.sprite = itemIcon;
            icon.gameObject.SetActive(itemIcon != null);
        }
        else
        {
            Debug.LogError("ItemIcon not found in popup prefab. Make sure the icon object is named ItemIcon.");
        }

        activePopups.Enqueue(popup);

        if (activePopups.Count > maxPopups)
        {
            GameObject oldPopup = activePopups.Dequeue();

            if (oldPopup != null)
            {
                Destroy(oldPopup);
            }
        }

        StartCoroutine(FadeOut(popup));
    }

    private IEnumerator FadeOut(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration);

        if (popup == null)
        {
            yield break;
        }

        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            Destroy(popup);
            yield break;
        }

        float timer = 0f;
        float fadeDuration = 0.5f;

        while (timer < fadeDuration)
        {
            if (popup == null)
            {
                yield break;
            }

            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            yield return null;
        }

        Destroy(popup);
    }
}