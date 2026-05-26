using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Return
    }

    private Transform target;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private EnemyAudio enemyAudio;

    public Enemy enemy;

    [Header("Movement")]
    public float stopDist = 0.35f;
    public float lostTargetRange = 7f;
    public float patrolRadius = 2f;
    public float patrolWaitTime = 1.5f;
    public bool returnToSpawnWhenLost = true;

    [Header("Attack Cooldown")]
    public float attackCooldown = 1.5f;
    public float attackLockDuration = 0.45f;

    private EnemyState currentState = EnemyState.Idle;
    private Vector2 spawnPosition;
    private Vector2 patrolTarget;
    private float patrolTimer = 0f;
    private float attackCooldownTimer = 0f;
    private bool isAttacking = false;
    private bool hasDealtDamageThisAttack = false;

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

        spawnPosition = transform.position;
    }

    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
        }

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0;
        }

        if (enemy != null)
        {
            attackCooldown = enemy.attackCd;
        }

        PickNewPatrolTarget();
    }

    void Update()
    {
        if (attackCooldownTimer > 0f)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if (target == null || enemy == null || rb == null)
        {
            StopMoving();
            return;
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
        else if (distanceToPlayer <= enemy.detectionRange)
        {
            ChangeState(EnemyState.Chase);
        }
        else if (currentState == EnemyState.Chase && distanceToPlayer > lostTargetRange)
        {
            if (returnToSpawnWhenLost)
            {
                ChangeState(EnemyState.Return);
            }
            else
            {
                ChangeState(EnemyState.Patrol);
            }
        }
        else if (currentState != EnemyState.Return && currentState != EnemyState.Patrol)
        {
            ChangeState(EnemyState.Patrol);
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                StopMoving();
                break;

            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                ChasePlayer(distanceToPlayer);
                break;

            case EnemyState.Attack:
                TryAttack();
                break;

            case EnemyState.Return:
                ReturnToSpawn();
                break;
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

    private void Patrol()
    {
        patrolTimer -= Time.fixedDeltaTime;

        float distanceToPatrolTarget = Vector2.Distance(transform.position, patrolTarget);

        if (distanceToPatrolTarget <= 0.15f || patrolTimer <= 0f)
        {
            PickNewPatrolTarget();
        }

        MoveToward(patrolTarget, enemy.speed * 0.45f);
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
        isAttacking = true;
        hasDealtDamageThisAttack = false;
        attackCooldownTimer = attackCooldown;

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

        Invoke(nameof(EndAttackLock), attackLockDuration);
    }

    private void EndAttackLock()
    {
        isAttacking = false;
    }

    private void ReturnToSpawn()
    {
        float distanceToSpawn = Vector2.Distance(transform.position, spawnPosition);

        if (distanceToSpawn <= 0.2f)
        {
            PickNewPatrolTarget();
            ChangeState(EnemyState.Patrol);
            return;
        }

        MoveToward(spawnPosition, enemy.speed * 0.65f);
    }

    private void MoveToward(Vector2 destination, float moveSpeed)
    {
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

    private void PickNewPatrolTarget()
    {
        Vector2 randomOffset = Random.insideUnitCircle * patrolRadius;
        patrolTarget = spawnPosition + randomOffset;
        patrolTimer = patrolWaitTime;
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

    public void DealDamage()
    {
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
        Vector2 center = Application.isPlaying ? spawnPosition : (Vector2)transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemy != null ? enemy.detectionRange : 5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, enemy != null ? enemy.attackRange : 1f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, patrolRadius);
    }
}