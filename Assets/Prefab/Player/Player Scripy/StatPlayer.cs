using UnityEngine;

public class StatPlayer : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100;
    public float speed = 1.25f;
    public float meleeDamage = 10;
    public float rangeDamage = 12;
    public float defense = 5;

    [Header("Damage Stun")]
    public float damageStunDuration = 0.7f;

    [HideInInspector] public float currentHealth;
    public bool isDead = false;

    public Animator anim;
    public GameOver gameOverManager;

    private Running playerMovement;

    void Awake()
    {
        currentHealth = health;
        playerMovement = GetComponent<Running>();

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, health);

        Debug.Log("PLAYER TOOK DAMAGE: " + amount);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        if (playerMovement != null)
        {
            playerMovement.StunFromDamage(damageStunDuration);
        }
        else
        {
            Debug.LogError("Running script not found on player.");
        }

        if (anim != null)
        {
            anim.SetTrigger("isHurt");
        }
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        if (anim != null)
        {
            anim.SetTrigger("isDead");
        }

        Running running = GetComponent<Running>();

        if (running != null)
        {
            running.enabled = false;
        }

        InteractionDetector interactionDetector = GetComponentInChildren<InteractionDetector>();

        if (interactionDetector != null)
        {
            interactionDetector.enabled = false;
        }

        PlayerItemCollector itemCollector = GetComponent<PlayerItemCollector>();

        if (itemCollector != null)
        {
            itemCollector.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        else
        {
            Time.timeScale = 0f;
        }
    }
}