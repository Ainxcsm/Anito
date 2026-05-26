using UnityEngine;

public class Breakable : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 1f;

    [Header("Drop Settings")]
    public GameObject[] possibleDrops;
    public float dropChance = 1f;

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

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

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

        DropItem();

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

    public void DestroyAfterBreakAnimation()
    {
        Destroy(gameObject);
    }
}