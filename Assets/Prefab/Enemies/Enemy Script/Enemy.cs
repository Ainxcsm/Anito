using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 5f;
    public float speed = 4f;
    public float damage = 5f;
    public float attackRange = 1f;
    public float detectionRange = 5f;
    public float attackCd = 0.5f;

    [HideInInspector] public float currentHealth;

    private bool isDead;

    [Header("Components")]
    public Animator animator;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer[] spriteRenderers;
    private EnemyAudio enemyAudio;

    [Header("Loot")]
    public LootEntry[] lootTable;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        enemyAudio = GetComponent<EnemyAudio>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;

        if (enemyAudio != null)
        {
            enemyAudio.PlayHit();
        }
        else if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayEnemyHit();
        }

        Debug.Log($"Enemy hit: {amount} | HP: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (enemyAudio != null)
        {
            enemyAudio.PlayDeath();
        }
        else if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayEnemyDeath();
        }

        Debug.Log("[ENEMY] DIED");

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (col != null)
        {
            col.enabled = false;
        }

        EnemyAI ai = GetComponent<EnemyAI>();

        if (ai != null)
        {
            ai.enabled = false;
        }

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        DropLoot();
    }

    void DropLoot()
    {
        if (lootTable == null || lootTable.Length == 0)
        {
            return;
        }

        float total = 0f;

        foreach (var item in lootTable)
        {
            total += item.chance;
        }

        float roll = Random.Range(0f, total);
        float current = 0f;

        foreach (var item in lootTable)
        {
            current += item.chance;

            if (roll <= current)
            {
                if (item.itemPrefab != null)
                {
                    Instantiate(item.itemPrefab, transform.position, Quaternion.identity);
                    Debug.Log("[ENEMY] Dropped item");
                }

                return;
            }
        }
    }

    public void FinishDeath()
    {
        Destroy(gameObject);
    }
}