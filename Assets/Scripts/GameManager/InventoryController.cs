using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    private Slot[] slots;

    void Start()
    {
        // Build slots properly (prevents hierarchy issues)
        slots = new Slot[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, inventoryPanel.transform);
            slots[i] = slotObj.GetComponent<Slot>();
            slots[i].currentItem = null;
        }
    }

    public bool AddItem(GameObject itemPrefab)
    {
        foreach (Slot slot in slots)
        {
            if (slot == null) continue;

            // IMPORTANT: clean destroyed references
            if (slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slot.transform);
                newItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                slot.currentItem = newItem;
                return true;
            }
        }

        Debug.Log("Inventory is full");
        return false;
    }
}