using UnityEngine;
using UnityEngine.UI;

public class RelicSlotUI : MonoBehaviour
{
    [Header("References")]
    public Slot slot;
    public GameObject hoverPanel;
    public GameObject selectedPanel;
    public Transform itemHolder;

    [Header("Settings")]
    public bool onlyWorkWhenHasItem = true;
    public bool showDebugLogs = true;

    private static RelicSlotUI selectedSlot;

    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private bool isHovered;
    private bool isSelected;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (slot == null)
        {
            slot = GetComponent<Slot>();
        }

        if (itemHolder == null)
        {
            itemHolder = transform;
        }

        parentCanvas = GetComponentInParent<Canvas>();

        UpdateVisual();
    }

    private void Update()
    {
        bool mouseInside = IsMouseInsideSlot();

        if (mouseInside && !isHovered)
        {
            isHovered = true;

            if (showDebugLogs)
            {
                Debug.Log("Hover entered: " + gameObject.name);
            }

            UpdateVisual();
        }
        else if (!mouseInside && isHovered)
        {
            isHovered = false;

            if (showDebugLogs)
            {
                Debug.Log("Hover exited: " + gameObject.name);
            }

            UpdateVisual();
        }

        if (mouseInside && Input.GetMouseButtonDown(0))
        {
            TrySelectSlot();
        }

        if (onlyWorkWhenHasItem && !HasItem() && isSelected)
        {
            ClearSelection();
        }

        UpdateVisual();
    }

    private bool IsMouseInsideSlot()
    {
        if (rectTransform == null)
        {
            return false;
        }

        Camera cameraToUse = null;

        if (parentCanvas != null && parentCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            cameraToUse = parentCanvas.worldCamera;
        }

        return RectTransformUtility.RectangleContainsScreenPoint(
            rectTransform,
            Input.mousePosition,
            cameraToUse
        );
    }

    private void TrySelectSlot()
    {
        if (onlyWorkWhenHasItem && !HasItem())
        {
            if (showDebugLogs)
            {
                Debug.Log("Clicked empty slot: " + gameObject.name);
            }

            return;
        }

        if (selectedSlot != null && selectedSlot != this)
        {
            selectedSlot.Deselect();
        }

        selectedSlot = this;
        isSelected = true;

        if (showDebugLogs)
        {
            Debug.Log("Selected slot: " + gameObject.name);
        }

        UpdateVisual();
    }

    private bool HasItem()
    {
        if (slot != null && slot.currentItem != null)
        {
            return true;
        }

        if (itemHolder == null)
        {
            return false;
        }

        foreach (Transform child in itemHolder)
        {
            if (!child.gameObject.activeInHierarchy)
            {
                continue;
            }

            if (hoverPanel != null && child == hoverPanel.transform)
            {
                continue;
            }

            if (selectedPanel != null && child == selectedPanel.transform)
            {
                continue;
            }

            Image image = child.GetComponent<Image>();

            if (image != null && image.sprite != null)
            {
                return true;
            }

            Item item = child.GetComponent<Item>();

            if (item != null)
            {
                return true;
            }
        }

        return false;
    }

    private void Deselect()
    {
        isSelected = false;
        isHovered = false;
        UpdateVisual();
    }

    private void ClearSelection()
    {
        if (selectedSlot == this)
        {
            selectedSlot = null;
        }

        isSelected = false;
        isHovered = false;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        bool hasItem = HasItem();

        if (hoverPanel != null)
        {
            hoverPanel.SetActive(false);
        }

        if (selectedPanel != null)
        {
            selectedPanel.SetActive(false);
        }

        if (onlyWorkWhenHasItem && !hasItem)
        {
            return;
        }

        if (isSelected)
        {
            if (selectedPanel != null)
            {
                selectedPanel.SetActive(true);
            }

            return;
        }

        if (isHovered)
        {
            if (hoverPanel != null)
            {
                hoverPanel.SetActive(true);
            }
        }
    }
}