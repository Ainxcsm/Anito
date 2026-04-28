using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }
    }

    public bool AddItem(GameObject item)
    {
        Item itemToAdd = item.GetComponent<Item>();
        if (itemToAdd == null) return false;

        // 🔥 STACK FIRST
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem != null)
            {
                Item slotItem = slot.currentItem.GetComponent<Item>();

                if (slotItem != null && slotItem.ID == itemToAdd.ID)
                {
                    slotItem.AddToStack(1);
                    return true;
                }
            }
        }

        // 🔥 CREATE NEW ITEM (same prefab)
        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(item, slotTransform);

                // 🔥 IMPORTANT: fix UI position
                RectTransform rect = newItem.GetComponent<RectTransform>();
                if (rect != null)
                    rect.anchoredPosition = Vector2.zero;

                slot.currentItem = newItem;

                return true;
            }
        }

        Debug.Log("Inventory is full");
        return false;
    }
}