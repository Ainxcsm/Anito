using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMeleeCombo : MonoBehaviour
{
    public Animator animator;
    public Running playerMovement;
    public WeaponAttack swordAttack;

    [Header("Combo Triggers")]
    public string melee1Trigger = "Melee1";
    public string melee2Trigger = "Melee2";
    public string melee3Trigger = "Melee3";

    [Header("Damage Per Slash")]
    public float melee1Damage = 5f;
    public float melee2Damage = 5f;
    public float melee3Damage = 5f;

    [Header("Slash Duration")]
    public float melee1Duration = 0.45f;
    public float melee2Duration = 0.45f;
    public float melee3Duration = 0.5f;

    [Header("Hitbox Start Delay")]
    public float melee1HitboxDelay = 0.12f;
    public float melee2HitboxDelay = 0.08f;
    public float melee3HitboxDelay = 0.08f;

    [Header("Hitbox Active Time")]
    public float melee1HitboxActiveTime = 0.3f;
    public float melee2HitboxActiveTime = 0.3f;
    public float melee3HitboxActiveTime = 0.35f;

    [Header("Combo Settings")]
    public float comboInputWindow = 0.38f;
    public float comboEndDelay = 0.08f;

    private int currentCombo;
    private bool isAttacking;
    private bool queuedNext;
    private Coroutine comboRoutine;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (playerMovement == null)
        {
            playerMovement = GetComponent<Running>();
        }
    }

    private void Update()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            PressAttack();
        }
    }

    private void PressAttack()
    {
        if (!isAttacking)
        {
            comboRoutine = StartCoroutine(ComboRoutine());
            return;
        }

        queuedNext = true;
        Debug.Log("Next melee slash queued.");
    }

    private IEnumerator ComboRoutine()
    {
        isAttacking = true;
        currentCombo = 1;
        queuedNext = false;

        if (playerMovement != null)
        {
            playerMovement.LockMovement();
        }

        while (currentCombo <= 3)
        {
            queuedNext = false;

            float slashDuration = GetSlashDuration(currentCombo);
            float hitboxDelay = GetHitboxDelay(currentCombo);
            float hitboxActiveTime = GetHitboxActiveTime(currentCombo);

            PlayComboAnimation(currentCombo);

            float timer = 0f;

            while (timer < hitboxDelay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            StartWeaponHitbox(currentCombo);

            float hitboxTimer = 0f;

            while (hitboxTimer < hitboxActiveTime)
            {
                if (swordAttack != null)
                {
                    swordAttack.CheckHits();
                }

                hitboxTimer += Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }

            StopWeaponHitBox();

            while (timer < comboInputWindow)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            while (timer < slashDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            if (queuedNext && currentCombo < 3)
            {
                currentCombo++;
            }
            else
            {
                break;
            }
        }

        yield return new WaitForSeconds(comboEndDelay);

        ResetCombo();
    }

    private void PlayComboAnimation(int combo)
    {
        if (animator == null)
        {
            return;
        }

        if (combo == 1)
        {
            animator.ResetTrigger(melee1Trigger);
            animator.SetTrigger(melee1Trigger);
        }
        else if (combo == 2)
        {
            animator.ResetTrigger(melee2Trigger);
            animator.SetTrigger(melee2Trigger);
        }
        else if (combo == 3)
        {
            animator.ResetTrigger(melee3Trigger);
            animator.SetTrigger(melee3Trigger);
        }

        Debug.Log("Melee combo slash: " + combo);
    }

    private void StartWeaponHitbox(int combo)
    {
        if (swordAttack == null)
        {
            return;
        }

        float slashDamage = GetSlashDamage(combo);

        swordAttack.isActiveWeapon = true;
        swordAttack.Attack(slashDamage);

        Debug.Log("Sword hitbox started for combo: " + combo + " | Damage: " + slashDamage);
    }

    private void StopWeaponHitBox()
    {
        if (swordAttack == null)
        {
            return;
        }

        swordAttack.StopAttack();

        Debug.Log("Sword hitbox stopped for combo: " + currentCombo);
    }

    private float GetSlashDamage(int combo)
    {
        if (combo == 1)
        {
            return melee1Damage;
        }

        if (combo == 2)
        {
            return melee2Damage;
        }

        return melee3Damage;
    }

    private float GetSlashDuration(int combo)
    {
        if (combo == 1)
        {
            return melee1Duration;
        }

        if (combo == 2)
        {
            return melee2Duration;
        }

        return melee3Duration;
    }

    private float GetHitboxDelay(int combo)
    {
        if (combo == 1)
        {
            return melee1HitboxDelay;
        }

        if (combo == 2)
        {
            return melee2HitboxDelay;
        }

        return melee3HitboxDelay;
    }

    private float GetHitboxActiveTime(int combo)
    {
        if (combo == 1)
        {
            return melee1HitboxActiveTime;
        }

        if (combo == 2)
        {
            return melee2HitboxActiveTime;
        }

        return melee3HitboxActiveTime;
    }

    private void ResetCombo()
    {
        StopWeaponHitBox();

        currentCombo = 0;
        queuedNext = false;
        isAttacking = false;

        if (playerMovement != null)
        {
            playerMovement.UnlockMovement();
        }

        comboRoutine = null;

        Debug.Log("Melee combo ended.");
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public void CancelCombo()
    {
        if (comboRoutine != null)
        {
            StopCoroutine(comboRoutine);
            comboRoutine = null;
        }

        ResetCombo();

        Debug.Log("Melee combo cancelled.");
    }
}