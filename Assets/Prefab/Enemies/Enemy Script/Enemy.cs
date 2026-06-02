using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 5f;
    public float speed = 4f;
    public float damage = 5f;
    public float armor = 0f;
    public float attackRange = 1f;
    public float detectionRange = 5f;
    public float attackCd = 0.5f;

    [HideInInspector] public float currentHealth;
    [HideInInspector] public float currentArmor;

    private bool isDead;

    [Header("Components")]
    public Animator animator;

    private Rigidbody2D rb;
    private Collider2D col;
    private EnemyAudio enemyAudio;

    [Header("Death")]
    public bool useDeathAnimation = true;
    public string deathTriggerName = "Die";
    public float destroyDelayWithoutDeathAnimation = 0f;

    [Header("Loot")]
    public LootEntry[] lootTable;

    [Header("Coin Drop Settings")]
    public GameObject coinPrefab;
    public bool dropCoins = true;
    public int minCoins = 1;
    public int maxCoins = 5;
    public float coinDropSpread = 0.3f;

    [Header("Damage Text")]
    public bool showDamageText = true;

    private Coroutine dotCoroutine;
    private Coroutine armorReductionCoroutine;

    void Awake()
    {
        currentHealth = maxHealth;
        currentArmor = armor;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        enemyAudio = GetComponent<EnemyAudio>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    public float TakeDamage(float amount)
    {
        if (isDead)
        {
            return 0f;
        }

        float finalDamage = Mathf.Max(amount - currentArmor, 1f);

        currentHealth -= finalDamage;

        if (showDamageText)
        {
            if (DamageTextSpawner.Instance != null)
            {
                DamageTextSpawner.Instance.SpawnDamageText(finalDamage, transform.position);
            }
            else
            {
                Debug.LogError("DamageTextSpawner.Instance is null.");
            }
        }

        if (enemyAudio != null)
        {
            enemyAudio.PlayHit();
        }
        else if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayEnemyHit();
        }

        Debug.Log("Enemy hit: " + finalDamage + " | HP: " + currentHealth + " | Armor: " + currentArmor);

        if (currentHealth <= 0f)
        {
            Die();
        }

        return finalDamage;
    }

    public void ApplyDamageOverTime(float damagePerSecond, float duration)
    {
        if (isDead)
        {
            return;
        }

        if (dotCoroutine != null)
        {
            StopCoroutine(dotCoroutine);
        }

        dotCoroutine = StartCoroutine(DamageOverTimeRoutine(damagePerSecond, duration));
    }

    private IEnumerator DamageOverTimeRoutine(float damagePerSecond, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            if (isDead)
            {
                yield break;
            }

            float damageThisFrame = damagePerSecond * Time.deltaTime;
            TakeDamage(damageThisFrame);

            timer += Time.deltaTime;
            yield return null;
        }

        dotCoroutine = null;
    }

    public void ApplyArmorReduction(float amount, float duration)
    {
        if (isDead)
        {
            return;
        }

        if (armorReductionCoroutine != null)
        {
            StopCoroutine(armorReductionCoroutine);
        }

        armorReductionCoroutine = StartCoroutine(ArmorReductionRoutine(amount, duration));
    }

    private IEnumerator ArmorReductionRoutine(float amount, float duration)
    {
        currentArmor = Mathf.Max(armor - amount, 0f);

        yield return new WaitForSeconds(duration);

        currentArmor = armor;
        armorReductionCoroutine = null;
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
            rb.bodyType = RigidbodyType2D.Kinematic;
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

        DropLoot();
        DropCoins();

        if (animator != null && useDeathAnimation)
        {
            animator.SetTrigger(deathTriggerName);
        }
        else
        {
            Destroy(gameObject, destroyDelayWithoutDeathAnimation);
        }
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

        if (total <= 0f)
        {
            return;
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

    private void DropCoins()
    {
        if (!dropCoins)
        {
            return;
        }

        if (coinPrefab == null)
        {
            return;
        }

        int coinAmount = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < coinAmount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * coinDropSpread;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
        }
    }

    public void FinishDeath()
    {
        Destroy(gameObject);
    }
}