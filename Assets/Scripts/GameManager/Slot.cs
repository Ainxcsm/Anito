using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public GameObject currentItem;
    public Image slotActivate;

    public void SetItem(GameObject item)
    {
        currentItem = item;
    }

    public void Clear()
    {
        currentItem = null;
        ClearSelectionVisual();
    }

    public bool HasItem()
    {
        return currentItem != null;
    }

    public void SelectSlot()
    {
        if (ResetSelectionOnClose.IsSelectionBlocked)
        {
            return;
        }

        if (slotActivate != null)
        {
            slotActivate.gameObject.SetActive(true);
            slotActivate.enabled = true;
        }

        Debug.Log("Selected slot: " + gameObject.name);
    }

    public void ClearSelectionVisual()
    {
        if (slotActivate != null)
        {
            slotActivate.enabled = false;
            slotActivate.gameObject.SetActive(false);
        }
    }
}