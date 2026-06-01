using System.Collections;
using UnityEngine;

public class StatPlayer : MonoBehaviour
{
    [Header("Base Stats")]
    public float health = 100;
    public float speed = 1.25f;
    public float meleeDamage = 10;
    public float defense = 5;

    [Header("Base Cooldowns")]
    public float dashCooldown = 1f;
    public float skillCooldownReduction = 0f;

    [Header("Current Final Stats")]
    public float maxHealth;
    public float currentSpeed;
    public float currentMeleeDamage;
    public float currentDefense;
    public float currentHealOverTime;
    public float currentDamageOverTime;
    public float currentDamageOverTimeDuration;
    public float currentLifeSteal;
    public float currentDashCooldown;
    public float currentSkillCooldownReduction;
    public float currentCritChance;
    public float currentCritDamage;
    public float currentAttackSpeed;
    public float currentEnemyArmorReduction;
    public float currentEnemyArmorReductionDuration;
    public float currentEvasion;

    [Header("Damage Stun")]
    public float damageStunDuration = 0.7f;

    [Header("Death")]
    public float deathAnimationDelay = 1.5f;
    public string deathAnimationStateName = "";

    [HideInInspector] public float currentHealth;
    public bool isDead = false;

    public Animator anim;
    public GameOver gameOverManager;

    private Running playerMovement;
    private Coroutine deathRoutine;
    private bool gameOverStarted = false;

    void Awake()
    {
        ResetFinalStats();
        currentHealth = maxHealth;

        playerMovement = GetComponent<Running>();

        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    void Update()
    {
        ApplyHealOverTime();
    }

    private void ResetFinalStats()
    {
        maxHealth = health;
        currentSpeed = speed;
        currentMeleeDamage = meleeDamage;
        currentDefense = defense;

        currentHealOverTime = 0f;
        currentDamageOverTime = 0f;
        currentDamageOverTimeDuration = 0f;
        currentLifeSteal = 0f;

        currentDashCooldown = dashCooldown;
        currentSkillCooldownReduction = skillCooldownReduction;

        currentCritChance = 0f;
        currentCritDamage = 1.5f;

        currentAttackSpeed = 1f;

        currentEnemyArmorReduction = 0f;
        currentEnemyArmorReductionDuration = 0f;

        currentEvasion = 0f;
    }

    public void RecalculateStatsFromInventory(InventoryController inventory)
    {
        float oldMaxHealth = maxHealth;

        ResetFinalStats();

        if (inventory != null)
        {
            foreach (Item item in inventory.GetInventoryItems())
            {
                if (item == null || !item.hasPassiveEffect || item.effects == null)
                {
                    continue;
                }

                foreach (ItemEffect effect in item.effects)
                {
                    float finalAmount = effect.scaleWithQuantity ? effect.amount * item.quantity : effect.amount;
                    ApplyPassiveEffect(effect, finalAmount);
                }
            }
        }

        currentDashCooldown = Mathf.Max(currentDashCooldown, 0.1f);
        currentSkillCooldownReduction = Mathf.Clamp(currentSkillCooldownReduction, 0f, 0.9f);
        currentCritChance = Mathf.Clamp01(currentCritChance);
        currentCritDamage = Mathf.Max(currentCritDamage, 1f);
        currentAttackSpeed = Mathf.Max(currentAttackSpeed, 0.1f);
        currentEvasion = Mathf.Clamp01(currentEvasion);

        if (maxHealth > oldMaxHealth)
        {
            currentHealth += maxHealth - oldMaxHealth;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Stats recalculated | HP: " + currentHealth + "/" + maxHealth +
                  " | Speed: " + currentSpeed +
                  " | Melee: " + currentMeleeDamage +
                  " | Defense: " + currentDefense +
                  " | HOT: " + currentHealOverTime +
                  " | DOT: " + currentDamageOverTime +
                  " | Lifesteal: " + currentLifeSteal +
                  " | Dash CD: " + currentDashCooldown +
                  " | Skill CD Reduction: " + currentSkillCooldownReduction +
                  " | Crit Chance: " + currentCritChance +
                  " | Crit Damage: " + currentCritDamage +
                  " | Attack Speed: " + currentAttackSpeed +
                  " | Armor Reduction: " + currentEnemyArmorReduction +
                  " | Evasion: " + currentEvasion);
    }

    private void ApplyPassiveEffect(ItemEffect effect, float amount)
    {
        switch (effect.effectType)
        {
            case ItemEffectType.MaxHealth:
                maxHealth += amount;
                break;

            case ItemEffectType.Speed:
                currentSpeed += amount;
                break;

            case ItemEffectType.MeleeDamage:
                currentMeleeDamage += amount;
                break;

            case ItemEffectType.Defense:
                currentDefense += amount;
                break;

            case ItemEffectType.HealOverTime:
                currentHealOverTime += amount;
                break;

            case ItemEffectType.DamageOverTime:
                currentDamageOverTime += amount;
                currentDamageOverTimeDuration = Mathf.Max(currentDamageOverTimeDuration, effect.duration);
                break;

            case ItemEffectType.LifeSteal:
                currentLifeSteal += amount;
                break;

            case ItemEffectType.CooldownReduction:
                if (effect.cooldownTarget == CooldownReductionTarget.Dash)
                {
                    currentDashCooldown -= amount;
                }
                else if (effect.cooldownTarget == CooldownReductionTarget.Skill)
                {
                    currentSkillCooldownReduction += amount;
                }
                break;

            case ItemEffectType.CritChance:
                currentCritChance += amount;
                break;

            case ItemEffectType.CritDamage:
                currentCritDamage += amount;
                break;

            case ItemEffectType.AttackSpeed:
                currentAttackSpeed += amount;
                break;

            case ItemEffectType.EnemyArmorReduction:
                currentEnemyArmorReduction += amount;
                currentEnemyArmorReductionDuration = Mathf.Max(currentEnemyArmorReductionDuration, effect.duration);
                break;

            case ItemEffectType.Evasion:
                currentEvasion += amount;
                break;
        }
    }

    private void ApplyHealOverTime()
    {
        if (isDead)
        {
            return;
        }

        if (currentHealOverTime <= 0f)
        {
            return;
        }

        if (currentHealth >= maxHealth)
        {
            return;
        }

        currentHealth += currentHealOverTime * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (isDead)
        {
            return;
        }

        if (Random.value < currentEvasion)
        {
            Debug.Log("PLAYER DODGED THE ATTACK.");
            return;
        }

        float finalDamage = Mathf.Max(amount - currentDefense, 1f);

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("PLAYER TOOK DAMAGE: " + finalDamage);

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

    public void Heal(float amount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Player healed: " + amount + " | HP: " + currentHealth);
    }

    public void ApplyLifeSteal(float damageDealt)
    {
        if (isDead)
        {
            return;
        }

        if (currentLifeSteal <= 0f)
        {
            return;
        }

        float healAmount = damageDealt * currentLifeSteal;

        if (healAmount <= 0f)
        {
            return;
        }

        Heal(healAmount);
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Time.timeScale = 1f;

        PlayDeathAnimation();

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

        if (deathRoutine != null)
        {
            StopCoroutine(deathRoutine);
        }

        deathRoutine = StartCoroutine(ShowGameOverAfterDeathAnimation());
    }

    private void PlayDeathAnimation()
    {
        if (anim == null)
        {
            Debug.LogError("Player Animator is missing.");
            return;
        }

        anim.SetBool("isMoving", false);

        if (!string.IsNullOrEmpty(deathAnimationStateName))
        {
            anim.Play(deathAnimationStateName, 0, 0f);
        }
        else
        {
            anim.SetTrigger("isDead");
        }

        anim.Update(0f);
    }

    private IEnumerator ShowGameOverAfterDeathAnimation()
    {
        yield return new WaitForSecondsRealtime(deathAnimationDelay);
        ShowGameOverNow();
    }

    public void FinishDeathAnimation()
    {
        ShowGameOverNow();
    }

    private void ShowGameOverNow()
    {
        if (gameOverStarted)
        {
            return;
        }

        gameOverStarted = true;

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