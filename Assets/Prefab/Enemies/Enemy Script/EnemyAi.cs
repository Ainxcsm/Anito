using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    private Transform target;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private EnemyAudio enemyAudio;

    public Enemy enemy;

    [Header("Movement")]
    public float stopDist = 0.35f;
    public bool alwaysChasePlayer = true;

    [Header("Attack")]
    public float attackAnimationLockDuration = 0.45f;

    [Header("Stun")]
    public bool showStunDebug = true;

    private EnemyState currentState = EnemyState.Idle;
    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;
    private bool hasDealtDamageThisAttack = false;

    private bool isStunned = false;
    private float stunEndTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAudio = GetComponent<EnemyAudio>();

        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }
    }

    void Start()
    {
        FindPlayer();

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0;
        }
    }

    void Update()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (target == null)
        {
            FindPlayer();
        }
    }

    void FixedUpdate()
    {
        if (target == null || enemy == null || rb == null)
        {
            StopMoving();
            return;
        }

        if (isStunned)
        {
            if (Time.time >= stunEndTime)
            {
                isStunned = false;

                if (showStunDebug)
                {
                    Debug.Log(name + " stun ended.");
                }
            }
            else
            {
                StopMoving();
                return;
            }
        }

        if (isAttacking || IsCurrentlyInAttackAnimation())
        {
            StopMoving();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer <= enemy.attackRange && attackCooldownTimer <= 0f)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (alwaysChasePlayer || distanceToPlayer <= enemy.detectionRange)
        {
            ChangeState(EnemyState.Chase);
        }
        else
        {
            ChangeState(EnemyState.Idle);
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                StopMoving();
                break;

            case EnemyState.Chase:
                ChasePlayer(distanceToPlayer);
                break;

            case EnemyState.Attack:
                TryAttack();
                break;
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
    }

    private void ChasePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer <= stopDist)
        {
            StopMoving();
            return;
        }

        MoveToward(target.position, enemy.speed);
    }

    private void TryAttack()
    {
        StopMoving();

        if (attackCooldownTimer > 0f)
        {
            return;
        }

        StartAttack();
    }

    private void StartAttack()
    {
        if (isStunned)
        {
            return;
        }

        isAttacking = true;
        hasDealtDamageThisAttack = false;
        attackCooldownTimer = enemy.attackCd;

        if (enemyAudio != null)
        {
            enemyAudio.PlayAttack();
        }
        else if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayEnemyAttack();
        }

        if (anim != null)
        {
            anim.SetBool("isWalk", false);
            anim.SetTrigger("isAttack");
        }

        Invoke(nameof(EndAttackLock), attackAnimationLockDuration);
    }

    private void EndAttackLock()
    {
        isAttacking = false;
    }

    private void MoveToward(Vector2 destination, float moveSpeed)
    {
        if (isStunned)
        {
            StopMoving();
            return;
        }

        Vector2 direction = (destination - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * moveSpeed;

        if (anim != null)
        {
            anim.SetBool("isWalk", true);
        }

        FlipToward(direction);
    }

    private void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (anim != null)
        {
            anim.SetBool("isWalk", false);
        }
    }

    private void FlipToward(Vector2 direction)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        if (Mathf.Abs(direction.x) > 0.01f)
        {
            spriteRenderer.flipX = direction.x > 0;
        }
    }

    private bool IsCurrentlyInAttackAnimation()
    {
        if (anim == null)
        {
            return false;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsTag("Attack");
    }

    public void Stun(float duration)
    {
        if (duration <= 0f)
        {
            return;
        }

        isStunned = true;
        stunEndTime = Time.time + duration;

        isAttacking = false;
        hasDealtDamageThisAttack = false;

        CancelInvoke(nameof(EndAttackLock));

        StopMoving();

        if (anim != null)
        {
            anim.SetBool("isWalk", false);
        }

        if (showStunDebug)
        {
            Debug.Log(name + " stunned for " + duration + " seconds.");
        }
    }

    public void DealDamage()
    {
        if (isStunned)
        {
            return;
        }

        if (target == null || enemy == null)
        {
            return;
        }

        if (hasDealtDamageThisAttack)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer > enemy.attackRange + 0.25f)
        {
            return;
        }

        StatPlayer player = target.GetComponent<StatPlayer>();

        if (player != null)
        {
            hasDealtDamageThisAttack = true;
            player.TakeDamage(enemy.damage);
        }
    }

    private void OnDisable()
    {
        CancelInvoke();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemy != null ? enemy.detectionRange : 5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemy != null ? enemy.attackRange : 1f);
    }
}