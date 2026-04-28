using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject currentItem;

    public void SetItem(GameObject item)
    {
        currentItem = item;
    }

    public void Clear()
    {
        currentItem = null;
    }
}