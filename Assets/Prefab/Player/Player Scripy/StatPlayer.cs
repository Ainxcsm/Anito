using System.Collections;
using UnityEngine;

public class StatPlayer : MonoBehaviour
{
    [Header("Stats")]
    public float health = 100;
    public float speed = 1.25f;
    public float meleeDamage = 10;
    public float rangeDamage = 12;
    public float defense = 5;

    [HideInInspector]
    public float currentHealth;

    public bool isDead = false; 
    public Animator anim;

    // Reference to GameOverManager to show panel on death
    public GameOver gameOverManager;

    void Awake()
    {
        currentHealth = health;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return; 

        currentHealth -= amount;
        if (anim != null) anim.SetTrigger("isHurt");
        currentHealth = Mathf.Clamp(currentHealth, 0, health);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // play death animation if available
        if (anim != null)
        {
            anim.SetTrigger("isDead");
        }

        // disable player controller scripts so input/physics stop
        var controllers = GetComponents<MonoBehaviour>();
        foreach (var c in controllers)
        {
            // skip this script and GameOverManager references; disable others
            if (c == this) continue;
            // Optionally: only disable known controller scripts instead of all
            c.enabled = false;
        }

        // Show Game Over UI and pause the game
        if (gameOverManager != null)
        {
            gameOverManager.ShowGameOver();
        }
        else
        {
            // fallback: pause the game if manager is not assigned
            Time.timeScale = 0f;
        }

        // DON'T destroy the player immediately; keep it in scene so UI can be shown.
        // If you really want to destroy after an animation: StartCoroutine(DestroyAfterDeath());
    }

    // optional coroutine if you want to remove player object after animation
    IEnumerator DestroyAfterDeath(float delay = 1f)
    {
        yield return new WaitForSecondsRealtime(delay); // use realtime because timescale = 0
        Destroy(gameObject);
    }
}
