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

    [HideInInspector]
    public float currentHealth;

    [Header("Components")]
    public Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer[] spriteRenderers;
    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

       
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            if (rb.bodyType == RigidbodyType2D.Dynamic)
                rb.isKinematic = true; 
        }

      
        if (col != null)
            col.enabled = false;

        foreach (var comp in GetComponents<MonoBehaviour>())
        {
            if (comp != this) 
                comp.enabled = false;
        }

        // Play death animation
        if (animator != null)
            animator.SetTrigger("Die");
    }

    
    public void FinishDeath()
    {
       
        foreach (var sr in spriteRenderers)
            sr.enabled = false;

        if (animator != null)
            animator.enabled = false;

    
        Destroy(gameObject, 0.01f);
    }
}
