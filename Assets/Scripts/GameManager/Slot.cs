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
    }
}