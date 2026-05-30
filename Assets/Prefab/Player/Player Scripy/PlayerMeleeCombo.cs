using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEditor.Animations;
using UnityEngine.Rendering;
public class PlayerMeleeCombo : MonoBehaviour
{
    public Animator animator;
    public Running playerMovement;
    public WeaponAttack swordAttack;

    [Header("Combo Triggers")]
    public string melee1Trigger = "Melee1";
    public string melee2Trigger = "Melee2";
    public string melee3Trigger = "Melee3";

    [Header("Timing")]
    public float slashDuration = 0.4f;
    public float comboInputWindow = 0.3f;
    public float comboEndDelay = 0.1f;

    private int currentCombo;
    private bool isAttacking;
    private bool queuedNext;

    private void Awake()
    {
        if(animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if(playerMovement == null)
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

        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            PressAttack();
        }
    }

    private void PressAttack ( )
    {
        if (!isAttacking)
        {
            StartCoroutine(ComboRoutine());
            return;
        }
        queuedNext = true;
    }
    private IEnumerator ComboRoutine()
    {
        isAttacking = true;
        currentCombo = 1;
        queuedNext = false;

        if(playerMovement != null)
        {
            playerMovement.LockMovement();
        }

        while (currentCombo <= 3)
        {
            PlayComboAnimation(currentCombo);
            StartWeaponHitbox();

            float timer = 0f;
            queuedNext = false;

            while (timer < comboInputWindow)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            float remainingTime = slashDuration - comboInputWindow;

            if (remainingTime > 0f)
            {
                yield return new WaitForSeconds(remainingTime);
            }

            StopWeaponHitBox();

            if(queuedNext && currentCombo < 3)
            {
                currentCombo++;
            } else
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
            animator.SetTrigger(melee1Trigger);
        } else if (combo == 2)
        {
            animator.SetTrigger(melee2Trigger);
        } else if ( combo == 3)
        {
            animator.SetTrigger(melee3Trigger);
        }

        Debug.Log("Melee combo slash: " + combo);
    }

    private void StartWeaponHitbox()
    {
        if(swordAttack == null)
        {
            return;
        }
        swordAttack.isActiveWeapon = true;
        swordAttack.Attack();
    }

    private void StopWeaponHitBox()
    {
        if(swordAttack == null)
        {
            return;
        }
        swordAttack.StopAttack();
        swordAttack.isActiveWeapon = false;
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

        Debug.Log("Melee Combo Eneded");
    }
}
