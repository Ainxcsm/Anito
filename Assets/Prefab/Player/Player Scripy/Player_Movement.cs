using System.Collections;
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

    [Header("Dash Settings")]
    public float dashForce = .75f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTime;
    private float lastDash = -Mathf.Infinity;
    private Vector2 dashDirection;
    private Coroutine damageStunCoroutine;

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
        UpdateMovementLock();

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
        UpdateMovementLock();

        if (isDashing && canMove)
        {
            if (dashTime > 0)
            {
                rb.linearVelocity = dashDirection * dashForce;
                dashTime -= Time.fixedDeltaTime;
            }
            else
            {
                isDashing = false;
                rb.linearVelocity = Vector2.zero;
                actionLocked = false;
                UpdateMovementLock();
            }
        }
        else
        {
            if (canMove)
            {
                rb.linearVelocity = moveInput * statPlayer.speed;
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }

    void StartDash()
    {
        if (dialogueLocked || damageLocked)
        {
            return;
        }

        isDashing = true;
        dashTime = dashDuration;
        lastDash = Time.time;
        dashDirection = moveInput;

        if (dashDirection == Vector2.zero)
        {
            dashDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
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

    public void AttackMelee()
    {
        if (dialogueLocked || damageLocked)
        {
            return;
        }

        LockMovement();

        if (swordAttack != null)
        {
            swordAttack.isActiveWeapon = true;
            swordAttack.Attack();
        }
    }

    public void AttackGun()
    {
        if (dialogueLocked || damageLocked)
        {
            return;
        }

        LockMovement();

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

        UnlockMovement();
    }

    public void LockMovement()
    {
        actionLocked = true;
        UpdateMovementLock();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
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
            StopPlayerMovement();
        }
    }

    public void StunFromDamage(float duration)
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        if (damageStunCoroutine != null)
        {
            StopCoroutine(damageStunCoroutine);
        }

        damageStunCoroutine = StartCoroutine(DamageStunRoutine(duration));
    }

    private IEnumerator DamageStunRoutine(float duration)
    {
        damageLocked = true;
        isDashing = false;
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

        UpdateMovementLock();
        StopPlayerMovement();

        yield return new WaitForSeconds(duration);

        damageLocked = false;
        damageStunCoroutine = null;
        UpdateMovementLock();
    }

    private void StopPlayerMovement()
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
        canMove = !dialogueLocked && !actionLocked && !damageLocked;
    }
}