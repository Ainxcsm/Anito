using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    public Sprite icon;

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