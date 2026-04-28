using TMPro;
using UnityEngine;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    public Sprite icon;

    public GameObject uiPrefab; // 🔥 IMPORTANT

    public int quantity = 1;

    private TMP_Text quantityText;

    private void Awake()
    {
        quantityText = GetComponentInChildren<TMP_Text>();
        UpdateQuantityDisplay();
    }

    public void UpdateQuantityDisplay()
    {
        if (quantityText != null)
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
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