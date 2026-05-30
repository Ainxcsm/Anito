using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{
    public static DamageTextSpawner Instance;

    public DamageTextPopup damageTextPrefab;
    public Canvas targetCanvas;

    [Header("Spawn Settings")]
    public Vector2 screenOffset = new Vector2(0f, 35f);
    public float randomXOffset = 20f;

    [Header("Debug")]
    public bool showDebug = true;

    private Camera mainCamera;
    private RectTransform canvasRect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        mainCamera = Camera.main;

        if (targetCanvas == null)
        {
            targetCanvas = FindAnyObjectByType<Canvas>();
        }

        if (targetCanvas != null)
        {
            canvasRect = targetCanvas.GetComponent<RectTransform>();
        }

        if (showDebug)
        {
            Debug.Log("DamageTextSpawner ready.");
        }
    }

    public void SpawnDamageText(float damage, Vector3 worldPosition)
    {
        if (damageTextPrefab == null)
        {
            Debug.LogError("DamageTextPrefab is missing on DamageTextSpawner.");
            return;
        }

        if (targetCanvas == null)
        {
            Debug.LogError("Target Canvas is missing on DamageTextSpawner.");
            return;
        }

        if (canvasRect == null)
        {
            canvasRect = targetCanvas.GetComponent<RectTransform>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is missing. Cannot place damage text.");
            return;
        }

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(worldPosition);

        if (screenPosition.z < 0f)
        {
            Debug.LogError("Damage text is behind the camera.");
            return;
        }

        Camera canvasCamera = null;

        if (targetCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvasCamera = targetCanvas.worldCamera;

            if (canvasCamera == null)
            {
                canvasCamera = mainCamera;
            }
        }

        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            canvasCamera,
            out localPoint
        );

        Vector2 randomOffset = new Vector2(Random.Range(-randomXOffset, randomXOffset), 0f);
        Vector2 finalPosition = localPoint + screenOffset + randomOffset;

        DamageTextPopup popup = Instantiate(damageTextPrefab, targetCanvas.transform);
        popup.transform.SetAsLastSibling();

        RectTransform popupRect = popup.GetComponent<RectTransform>();

        if (popupRect != null)
        {
            popupRect.anchorMin = new Vector2(0.5f, 0.5f);
            popupRect.anchorMax = new Vector2(0.5f, 0.5f);
            popupRect.pivot = new Vector2(0.5f, 0.5f);
            popupRect.anchoredPosition = finalPosition;
            popupRect.localRotation = Quaternion.identity;
            popupRect.localScale = Vector3.one;
        }

        popup.Setup(damage);

        if (showDebug)
        {
            Debug.Log("Damage text spawned: " + damage + " at canvas position " + finalPosition);
        }
    }
}