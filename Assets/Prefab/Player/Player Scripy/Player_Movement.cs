using UnityEngine;
using UnityEngine.InputSystem;

public class Running : MonoBehaviour
{
    public StatPlayer statPlayer;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public WeaponAttack swordAttack; // assign sword hitbox
    public WeaponAttack gunAttack;   // assign gun hitbox

    bool canMove = true;

    [Header("Dash Settings")]
    public float dashForce = .75f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;

    private bool isDashing = false;
    private float dashTime;
    private float lastDash = -Mathf.Infinity;
    private Vector2 dashDirection;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!canMove)
        {
            moveInput = Vector2.zero;
            animator.SetBool("isMoving", false);
            return;
        }

        moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        if (Keyboard.current.spaceKey.wasPressedThisFrame && Time.time >= lastDash + dashCooldown && canMove)
        {
            StartDash();
        }

        moveInput = moveInput.normalized;
        animator.SetBool("isMoving", moveInput != Vector2.zero);

        if (moveInput.x < 0) spriteRenderer.flipX = true;
        else if (moveInput.x > 0) spriteRenderer.flipX = false;


    }

    void StartDash()
    {   
        isDashing = true;
        dashTime = dashDuration;
        lastDash = Time.time;

        // Use current movement input as dash direction
        dashDirection = moveInput;
        if (dashDirection == Vector2.zero) // if no input, dash forward based on sprite
            dashDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        animator.SetTrigger("Dash");

        LockMovement(); // optional: lock normal movement during dash
    }

    void FixedUpdate()
    {
        if (isDashing)
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
                UnlockMovement();
            }
        }
        else
        {
            if (canMove)
                rb.linearVelocity = moveInput * statPlayer.speed;
            else
                rb.linearVelocity = Vector2.zero;
        }
    }


    public void AttackMelee()
    {
        LockMovement();
        swordAttack.isActiveWeapon = true;
        swordAttack.Attack();
    }

    public void AttackGun()
    {
        LockMovement();
        gunAttack.isActiveWeapon = true;
        gunAttack.Attack();
    }

    public void EndAttack()
    {
        UnlockMovement();
        swordAttack.StopAttack();
        gunAttack.StopAttack();
        swordAttack.isActiveWeapon = false;
        gunAttack.isActiveWeapon = false;
    }

    public void LockMovement()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }
}
