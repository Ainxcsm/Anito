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
        if (pickupLocked) return;

        if (!collision.CompareTag("Item")) return;

        Item item = collision.GetComponent<Item>();
        if (item == null) return;

        pickupLocked = true;

        Debug.Log("TRIGGER HIT: " + collision.name);

        bool added = inventoryController.AddItem(collision.gameObject);

        if (added)
        {
            item.Pickup();

            // 🔥 IMPORTANT: disable collider FIRST
            collision.enabled = false;

            Destroy(collision.gameObject);
        }

        // allow next pickup next frame
        StartCoroutine(ResetPickup());
    }

    private System.Collections.IEnumerator ResetPickup()
    {
        yield return null;
        pickupLocked = false;
    }
}