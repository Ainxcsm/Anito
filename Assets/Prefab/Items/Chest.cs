using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public LootEntry[] lootTable;

    public Sprite openedSprite;

    private bool isOpened;

    public bool CanInteract()
    {
        return !isOpened;
    }

    public void Interact()
    {
        if (isOpened) return;

        OpenChest();
    }

    private void OpenChest()
    {
        isOpened = true;

        GetComponent<SpriteRenderer>().sprite = openedSprite;

        GameObject loot = RollLoot();

        if (loot != null)
        {
            Instantiate(loot, transform.position + Vector3.down * 0.2f, Quaternion.identity);
        }

        GetComponent<Collider2D>().enabled = false;
    }

    private GameObject RollLoot()
    {
        float totalChance = 0f;

        foreach (var item in lootTable)
        {
            totalChance += item.chance;
        }

        float roll = Random.Range(0f, totalChance);

        float current = 0f;

        foreach (var item in lootTable)
        {
            current += item.chance;

            if (roll <= current)
            {
                return item.itemPrefab;
            }
        }

        return null;
    }
}