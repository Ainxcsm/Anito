using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public LootEntry[] lootTable;

    [Header("Visuals")]
    public Sprite openedSprite;
    public Animator animator;

    [Header("Animation")]
    public string openTriggerName = "Open";
    public float destroyDelay = 0.6f;

    [Header("Loot")]
    public Vector3 lootDropOffset = new Vector3(0f, -0.2f, 0f);

    private bool isOpened;
    private SpriteRenderer spriteRenderer;
    private Collider2D chestCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        chestCollider = GetComponent<Collider2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public bool CanInteract()
    {
        return !isOpened;
    }

    public void Interact()
    {
        if (isOpened)
        {
            return;
        }

        OpenChest();
    }

    private void OpenChest()
    {
        isOpened = true;

        if (chestCollider != null)
        {
            chestCollider.enabled = false;
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayChestOpen();
        }

        GameObject loot = RollLoot();

        if (loot != null)
        {
            Instantiate(loot, transform.position + lootDropOffset, Quaternion.identity);
        }

        if (animator != null)
        {
            animator.SetTrigger(openTriggerName);
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            if (spriteRenderer != null && openedSprite != null)
            {
                spriteRenderer.sprite = openedSprite;
            }

            Destroy(gameObject, destroyDelay);
        }
    }

    private GameObject RollLoot()
    {
        if (lootTable == null || lootTable.Length == 0)
        {
            return null;
        }

        float totalChance = 0f;

        foreach (var item in lootTable)
        {
            totalChance += item.chance;
        }

        if (totalChance <= 0f)
        {
            return null;
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

    public void DestroyAfterOpenAnimation()
    {
        Destroy(gameObject);
    }
}