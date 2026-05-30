using UnityEngine;
using UnityEngine.InputSystem;

public class Running : MonoBehaviour
{
    public StatPlayer statPlayer;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public WeaponAttack swordAttack;
    public WeaponAttack gunAttack;
    public InteractionDetector interactionDetector;

    private bool canMove = true;
    private bool dialogueLocked = false;
    private bool actionLocked = false;
    private bool damageLocked = false;
    private bool uiLocked = false;

    [Header("Dash Settings")]
    public float dashForce = .75f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTime;
    private float lastDash = -Mathf.Infinity;
    private Vector2 dashDirection;

    private float damageLockEndTime = 0f;
    private Coroutine actionUnlockRoutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (statPlayer == null)
        {
            statPlayer = GetComponent<StatPlayer>();
        }
    }

    void Update()
    {
        UpdateDamageLock();
        UpdateMovementLock();

        if (damageLocked || dialogueLocked || uiLocked)
        {
            CancelDash();
            ForceStopMovement();
            return;
        }

        if (!canMove)
        {
            moveInput = Vector2.zero;

            if (animator != null)
            {
                animator.SetBool("isMoving", false);
            }

            return;
        }

        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
        {
            moveInput.y += 1;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            moveInput.y -= 1;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            moveInput.x -= 1;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            moveInput.x += 1;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= lastDash + dashCooldown)
        {
            StartDash();
        }

        if (Keyboard.current.eKey.wasPressedThisFrame && !isDashing)
        {
            if (interactionDetector != null)
            {
                interactionDetector.TryInteract();
            }
        }

        moveInput = moveInput.normalized;

        if (animator != null)
        {
            animator.SetBool("isMoving", moveInput != Vector2.zero);
        }

        if (spriteRenderer != null)
        {
            if (moveInput.x < 0)
            {
                spriteRenderer.flipX = true;
            }
            else if (moveInput.x > 0)
            {
                spriteRenderer.flipX = false;
            }
        }
    }

    void FixedUpdate()
    {
        UpdateDamageLock();
        UpdateMovementLock();

        if (damageLocked || dialogueLocked || uiLocked)
        {
            CancelDash();
            ForceStopMovement();
            return;
        }

        if (isDashing)
        {
            if (dashTime > 0)
            {
                rb.linearVelocity = dashDirection * dashForce;
                dashTime -= Time.fixedDeltaTime;
            }
            else
            {
                StopDash();
            }

            return;
        }

        if (canMove)
        {
            rb.linearVelocity = moveInput * statPlayer.speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void StartDash()
    {
        UpdateDamageLock();

        if (dialogueLocked || damageLocked || uiLocked || isDashing)
        {
            return;
        }

        isDashing = true;
        dashTime = dashDuration;
        lastDash = Time.time;
        dashDirection = moveInput;

        if (dashDirection == Vector2.zero)
        {
            if (spriteRenderer != null && spriteRenderer.flipX)
            {
                dashDirection = Vector2.left;
            }
            else
            {
                dashDirection = Vector2.right;
            }
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayDash();
        }

        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }

        actionLocked = true;
        UpdateMovementLock();
    }

    private void StopDash()
    {
        isDashing = false;
        dashTime = 0f;
        dashDirection = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        actionLocked = false;
        UpdateMovementLock();
    }

    private void CancelDash()
    {
        isDashing = false;
        dashTime = 0f;
        dashDirection = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        actionLocked = false;
        UpdateMovementLock();
    }

    public void AttackMelee()
    {
        UpdateDamageLock();

        if (dialogueLocked || damageLocked || uiLocked || isDashing || actionLocked)
        {
            return;
        }

        LockMovement();
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        if (swordAttack != null)
        {
            swordAttack.isActiveWeapon = true;
            swordAttack.Attack();
        }
    }

    public void AttackGun()
    {
        UpdateDamageLock();

        if (dialogueLocked || damageLocked || uiLocked || isDashing || actionLocked)
        {
            return;
        }

        LockMovement();
        if (animator != null)
        {
            animator.SetTrigger("AttackR");
        }
        if (gunAttack != null)
        {
            gunAttack.isActiveWeapon = true;
            gunAttack.Attack();
        }
    }

    public void EndAttack()
    {
        if (swordAttack != null)
        {
            swordAttack.StopAttack();
            swordAttack.isActiveWeapon = false;
        }

        if (gunAttack != null)
        {
            gunAttack.StopAttack();
            gunAttack.isActiveWeapon = false;
        }
        if (actionUnlockRoutine != null )
        {
            StopCoroutine(actionUnlockRoutine);
            actionUnlockRoutine = null;
        }

        if (!damageLocked && !dialogueLocked && !uiLocked)
        {
            UnlockMovement();
        }
        else
        {
            actionLocked = false;
            UpdateMovementLock();
        }
    }
    private System.Collections.IEnumerator UnlockActionAfterDelay (float delay)
    {
        yield return new WaitForSeconds(delay);

        if(!damageLocked && !dialogueLocked && !uiLocked)
        {
            UnlockMovement();
        } else
        {
            actionLocked = false;
            UpdateMovementLock();
        }

        actionUnlockRoutine = null;
    }

    public void LockMovement()
    {
        actionLocked = true;
        UpdateMovementLock();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (actionUnlockRoutine != null)
        {
            StopCoroutine(actionUnlockRoutine);
        }

        actionUnlockRoutine = StartCoroutine(UnlockActionAfterDelay(0.45f));
    }

    public void UnlockMovement()
    {
        actionLocked = false;
        UpdateMovementLock();
    }

    public void SetDialogueLock(bool locked)
    {
        dialogueLocked = locked;
        UpdateMovementLock();

        if (locked)
        {
            CancelDash();
            ForceStopMovement();
        }
    }

    public void SetUILock(bool locked)
    {
        uiLocked = locked;
        UpdateMovementLock();

        if (locked)
        {
            CancelDash();
            ForceStopMovement();
        }
    }

    public void StunFromDamage(float duration)
    {
        damageLocked = true;
        damageLockEndTime = Time.time + duration;

        CancelDash();

        actionLocked = false;

        if (swordAttack != null)
        {
            swordAttack.StopAttack();
            swordAttack.isActiveWeapon = false;
        }

        if (gunAttack != null)
        {
            gunAttack.StopAttack();
            gunAttack.isActiveWeapon = false;
        }

        ForceStopMovement();
        UpdateMovementLock();

        Debug.Log("PLAYER STUNNED FOR: " + duration);
    }

    private void UpdateDamageLock()
    {
        if (damageLocked && Time.time >= damageLockEndTime)
        {
            damageLocked = false;
            UpdateMovementLock();
        }
    }

    private void ForceStopMovement()
    {
        moveInput = Vector2.zero;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void UpdateMovementLock()
    {
        canMove = !dialogueLocked && !actionLocked && !damageLocked && !uiLocked;
    }
}