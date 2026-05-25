using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    private readonly HashSet<int> processedPickupObjects = new HashSet<int>();

    void Start()
    {
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }
    }

    public bool AddItem(GameObject item)
    {
        if (item == null)
        {
            return false;
        }

        int pickupObjectID = item.GetInstanceID();

        if (processedPickupObjects.Contains(pickupObjectID))
        {
            return false;
        }

        Item itemToAdd = item.GetComponent<Item>();

        if (itemToAdd == null)
        {
            Debug.LogError("Picked object has no Item component: " + item.name);
            return false;
        }

        processedPickupObjects.Add(pickupObjectID);

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

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem == null)
            {
                GameObject prefabToUse = itemToAdd.uiPrefab != null ? itemToAdd.uiPrefab : item;
                GameObject newItem = Instantiate(prefabToUse, slotTransform);

                RectTransform rect = newItem.GetComponent<RectTransform>();

                if (rect != null)
                {
                    rect.anchoredPosition = Vector2.zero;
                    rect.localScale = Vector3.one;
                    rect.localRotation = Quaternion.identity;
                }

                Item newItemComponent = newItem.GetComponent<Item>();

                if (newItemComponent != null)
                {
                    newItemComponent.ID = itemToAdd.ID;
                    newItemComponent.Name = itemToAdd.Name;
                    newItemComponent.icon = itemToAdd.icon;
                    newItemComponent.uiPrefab = itemToAdd.uiPrefab;
                    newItemComponent.quantity = 1;
                    newItemComponent.UpdateQuantityDisplay();
                }

                slot.currentItem = newItem;
                return true;
            }
        }

        processedPickupObjects.Remove(pickupObjectID);
        Debug.Log("Inventory is full");
        return false;
    }
}