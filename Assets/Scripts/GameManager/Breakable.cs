using UnityEngine;

public class Breakable : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 1f;

    [Header("Item Drop Settings")]
    public GameObject[] possibleDrops;
    public float dropChance = 1f;

    [Header("Coin Drop Settings")]
    public GameObject coinPrefab;
    public bool dropCoins = true;
    public int minCoins = 1;
    public int maxCoins = 3;
    public float coinDropSpread = 0.25f;

    [Header("Effects")]
    public GameObject breakEffect;

    private float currentHealth;
    private bool isBroken = false;
    private Animator animator;
    private Collider2D col;

    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
    }

    public void TakeDamage(float damage)
    {
        if (isBroken)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Break();
        }
    }

    private void Break()
    {
        if (isBroken)
        {
            return;
        }

        isBroken = true;

        if (col != null)
        {
            col.enabled = false;
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayBreakableBreak();
        }

        DropItem();
        DropCoins();

        if (breakEffect != null)
        {
            Instantiate(breakEffect, transform.position, Quaternion.identity);
        }

        if (animator != null)
        {
            animator.SetTrigger("Break");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void DropItem()
    {
        if (possibleDrops == null || possibleDrops.Length == 0)
        {
            return;
        }

        float roll = Random.Range(0f, 1f);

        if (roll > dropChance)
        {
            return;
        }

        int randomIndex = Random.Range(0, possibleDrops.Length);
        GameObject selectedDrop = possibleDrops[randomIndex];

        if (selectedDrop != null)
        {
            Instantiate(selectedDrop, transform.position, Quaternion.identity);
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

    public void DestroyAfterBreakAnimation()
    {
        Destroy(gameObject);
    }
}