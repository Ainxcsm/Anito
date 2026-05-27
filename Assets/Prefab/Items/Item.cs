using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;
    public string itemKey;
    public string Name;
    public Sprite icon;
    public GameObject uiPrefab;
    public int quantity = 1;

    private TMP_Text quantityText;

    private void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Name = gameObject.name;
        }

        if (string.IsNullOrWhiteSpace(itemKey))
        {
            itemKey = Name.Trim().ToLower().Replace(" ", "_");
        }
    }

    private void Awake()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            Name = gameObject.name;
        }

        if (string.IsNullOrWhiteSpace(itemKey))
        {
            itemKey = Name.Trim().ToLower().Replace(" ", "_");
        }

        quantityText = GetComponentInChildren<TMP_Text>(true);
        UpdateQuantityDisplay();
    }

    public string GetStackKey()
    {
        if (!string.IsNullOrWhiteSpace(itemKey))
        {
            return itemKey;
        }

        if (!string.IsNullOrWhiteSpace(Name))
        {
            return Name.Trim().ToLower().Replace(" ", "_");
        }

        return gameObject.name.Trim().ToLower().Replace(" ", "_");
    }

    public bool CanStackWith(Item otherItem)
    {
        if (otherItem == null)
        {
            return false;
        }

        return GetStackKey() == otherItem.GetStackKey();
    }

    public void UpdateQuantityDisplay()
    {
        if (quantityText != null)
        {
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
    }

    public void AddToStack(int amount = 1)
    {
        quantity += amount;
        UpdateQuantityDisplay();
    }

    public int RemoveFromStack(int amount = 1)
    {
        int removed = Mathf.Min(amount, quantity);
        quantity -= removed;
        UpdateQuantityDisplay();
        return removed;
    }

    public void Pickup()
    {
        Debug.Log("Pickup triggered: " + Name);

        if (ItemPickUIController.Instance != null)
        {
            ItemPickUIController.Instance.ShowItemPickup(Name, icon);
        }
        else
        {
            Debug.LogError("ItemPickUIController Instance is NULL.");
        }
    }
}