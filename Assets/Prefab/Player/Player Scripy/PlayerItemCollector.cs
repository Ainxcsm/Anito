using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;
    private bool pickupLocked;

    void Start()
    {
        inventoryController = FindAnyObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pickupLocked)
        {
            return;
        }

        if (!collision.CompareTag("Item"))
        {
            return;
        }

        Item item = collision.GetComponent<Item>();

        if (item == null)
        {
            return;
        }

        if (inventoryController == null)
        {
            inventoryController = FindAnyObjectByType<InventoryController>();
        }

        if (inventoryController == null)
        {
            Debug.LogError("InventoryController not found.");
            return;
        }

        pickupLocked = true;

        Debug.Log("TRIGGER HIT: " + collision.name);

        bool added = inventoryController.AddItem(collision.gameObject);

        if (added)
        {
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayItemPickup();
            }

            item.Pickup();

            collision.enabled = false;

            Destroy(collision.gameObject);
        }

        StartCoroutine(ResetPickup());
    }

    private System.Collections.IEnumerator ResetPickup()
    {
        yield return null;
        pickupLocked = false;
    }
}