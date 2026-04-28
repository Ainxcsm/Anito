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

    private readonly Queue<GameObject> activePopups = new();

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
        GameObject popup = Instantiate(popupPrefab, transform);

        TMP_Text text = popup.GetComponentInChildren<TMP_Text>();

        // ✅ FIX: ONLY target ItemIcon, NOT all Images
        Image icon = popup.transform.Find("ItemIcon")?.GetComponent<Image>();

        if (text != null)
            text.text = itemName;

        if (icon != null)
        {
            icon.sprite = itemIcon;
            icon.gameObject.SetActive(itemIcon != null);
        }
        else
        {
            Debug.LogError("ItemIcon not found in popup prefab!");
        }

        activePopups.Enqueue(popup);

        if (activePopups.Count > maxPopups)
        {
            Destroy(activePopups.Dequeue());
        }

        StartCoroutine(FadeOut(popup));
    }

    private IEnumerator FadeOut(GameObject popup)
    {
        yield return new WaitForSeconds(popupDuration);

        if (popup == null) yield break;

        CanvasGroup cg = popup.GetComponent<CanvasGroup>();

        if (cg == null)
        {
            Destroy(popup);
            yield break;
        }

        float t = 0f;

        while (t < 1f)
        {
            if (popup == null) yield break;

            cg.alpha = 1f - t;
            t += Time.deltaTime;
            yield return null;
        }

        Destroy(popup);
    }
}