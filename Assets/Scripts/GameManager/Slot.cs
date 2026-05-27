using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject currentItem;
    public Image slotActivate;

    private static Slot selectedSlot;
    private bool isSelected;

    private void OnEnable()
    {
        isSelected = false;
        ClearSelectionVisual();

        if (selectedSlot == this)
        {
            selectedSlot = null;
        }
    }

    private void OnDisable()
    {
        isSelected = false;
        ClearSelectionVisual();

        if (selectedSlot == this)
        {
            selectedSlot = null;
        }
    }

    public void SetItem(GameObject item)
    {
        currentItem = item;
    }

    public void Clear()
    {
        currentItem = null;
        DeselectSlot();
    }

    public bool HasItem()
    {
        return currentItem != null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentItem == null)
        {
            return;
        }

        SelectSlot();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null)
        {
            return;
        }

        SelectSlot();
    }

    public void SelectSlot()
    {
        if (selectedSlot != null && selectedSlot != this)
        {
            selectedSlot.DeselectSlot();
        }

        selectedSlot = this;
        isSelected = true;

        if (slotActivate != null)
        {
            slotActivate.gameObject.SetActive(true);
            slotActivate.enabled = true;
        }

        Debug.Log("Selected slot: " + gameObject.name);
    }

    public void DeselectSlot()
    {
        isSelected = false;

        if (selectedSlot == this)
        {
            selectedSlot = null;
        }

        ClearSelectionVisual();
    }

    public void ClearSelectionVisual()
    {
        if (slotActivate != null)
        {
            slotActivate.enabled = false;
            slotActivate.gameObject.SetActive(false);
        }
    }

    public static void ClearSelectedSlot()
    {
        if (selectedSlot != null)
        {
            selectedSlot.DeselectSlot();
        }

        selectedSlot = null;
    }
}