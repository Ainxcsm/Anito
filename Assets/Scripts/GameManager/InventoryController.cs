using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;

    private readonly HashSet<int> processedPickupObjects = new HashSet<int>();
    private StatPlayer playerStats;

    void Start()
    {
        playerStats = FindAnyObjectByType<StatPlayer>();

        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        RecalculatePlayerStats();
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

                if (slotItem != null && slotItem.CanStackWith(itemToAdd))
                {
                    slotItem.AddToStack(1);
                    RecalculatePlayerStats();
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
                    newItemComponent.itemKey = itemToAdd.GetStackKey();
                    newItemComponent.Name = itemToAdd.Name;
                    newItemComponent.icon = itemToAdd.icon;
                    newItemComponent.uiPrefab = itemToAdd.uiPrefab;
                    newItemComponent.quantity = 1;
                    newItemComponent.hasPassiveEffect = itemToAdd.hasPassiveEffect;
                    newItemComponent.effects = itemToAdd.effects;
                    newItemComponent.UpdateQuantityDisplay();
                }

                slot.currentItem = newItem;
                RecalculatePlayerStats();
                return true;
            }
        }

        processedPickupObjects.Remove(pickupObjectID);
        Debug.Log("Inventory is full");
        return false;
    }

    public List<Item> GetInventoryItems()
    {
        List<Item> items = new List<Item>();

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();

            if (slot != null && slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();

                if (item != null)
                {
                    items.Add(item);
                }
            }
        }

        return items;
    }

    public void RecalculatePlayerStats()
    {
        if (playerStats == null)
        {
            playerStats = FindAnyObjectByType<StatPlayer>();
        }

        if (playerStats != null)
        {
            playerStats.RecalculateStatsFromInventory(this);
        }
    }
}