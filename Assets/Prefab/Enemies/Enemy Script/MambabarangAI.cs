using System.Collections;
using UnityEngine;

public class MambabarangAI : MonoBehaviour
{
    private enum BossState
    {
        Idle,
        Chase,
        Casting,
        Dead
    }

    [Header("References")]
    public Mambabarang boss;
    public Transform target;
    public Transform cloudSpawnPoint;
    public GameObject cloudPrefab;
    public GameObject aoeWarningPrefab;
    public LayerMask playerLayer;

    [Header("Movement")]
    public float preferredDistance = 5f;
    public float tooCloseDistance = 2.5f;
    public float movementSmoothness = 1f;
    public bool retreatWhenTooClose = false;
    public float retreatSpeedMultiplier = 0.4f;

    [Header("Attack Timing")]
    public float firstAttackDelay = 1.25f;
    public float attackDecisionCooldown = 2.25f;

    [Header("Beetle Cloud Attack")]
    public float cloudGatherDuration = 1f;
    public string cloudGatherTrigger = "CloudGather";
    public string cloudShootTrigger = "CloudShoot";

    [Header("AOE Attack")]
    public float aoeGatherDuration = 0.85f;
    public float aoeWarningDuration = 1.1f;
    public float aoeRadius = 2.5f;
    public float aoeDamage = 20f;
    public string aoeTrigger = "AOE";

    [Header("Animation")]
    public string walkBool = "isWalk";

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private BossState currentState = BossState.Idle;
    private bool isCasting = false;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (boss == null)
        {
            boss = GetComponent<Mambabarang>();
        }

        if (cloudSpawnPoint == null)
        {
            cloudSpawnPoint = transform;
        }
    }

    private void Start()
    {
        FindPlayer();

        if (rb != null)
        {
            rb.freezeRotation = true;
            rb.gravityScale = 0f;
        }

        nextAttackTime = Time.time + firstAttackDelay;
    }

    private void Update()
    {
        if (boss == null)
        {
            return;
        }

        if (boss.currentHealth <= 0f)
        {
            ChangeState(BossState.Dead);
            StopMoving();
            enabled = false;
            return;
        }

        if (target == null)
        {
            FindPlayer();
        }
    }

    private void FixedUpdate()
    {
        if (boss == null || rb == null || target == null)
        {
            StopMoving();
            return;
        }

        if (currentState == BossState.Dead)
        {
            StopMoving();
            return;
        }

        if (isCasting)
        {
            StopMoving();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);

        if (distanceToPlayer > boss.detectionRange)
        {
            ChangeState(BossState.Idle);
            StopMoving();
            return;
        }

        if (Time.time >= nextAttackTime && distanceToPlayer <= boss.attackRange)
        {
            ChooseAttack();
            return;
        }

        ChangeState(BossState.Chase);
        MoveAroundPlayer(distanceToPlayer);
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            target = playerObject.transform;
        }
    }

    private void ChangeState(BossState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
    }

    private void MoveAroundPlayer(float distanceToPlayer)
    {
        Vector2 directionToPlayer = ((Vector2)target.position - rb.position).normalized;

        if (distanceToPlayer > preferredDistance)
        {
            Vector2 moveDirection = directionToPlayer;
            rb.linearVelocity = moveDirection * boss.speed * movementSmoothness;

            if (anim != null)
            {
                anim.SetBool(walkBool, true);
            }

            FlipToward(directionToPlayer);
            return;
        }

        if (distanceToPlayer < tooCloseDistance)
        {
            FlipToward(directionToPlayer);

            if (retreatWhenTooClose)
            {
                Vector2 retreatDirection = -directionToPlayer;
                rb.linearVelocity = retreatDirection * boss.speed * retreatSpeedMultiplier;

                if (anim != null)
                {
                    anim.SetBool(walkBool, true);
                }
            }
            else
            {
                StopMoving();
            }

            return;
        }

        StopMoving();
        FlipToward(directionToPlayer);
    }

    private void ChooseAttack()
    {
        StopMoving();

        int roll = Random.Range(0, 2);

        if (roll == 0)
        {
            StartCoroutine(BeetleCloudAttackRoutine());
        }
        else
        {
            StartCoroutine(AOEAttackRoutine());
        }
    }

    private IEnumerator BeetleCloudAttackRoutine()
    {
        isCasting = true;
        ChangeState(BossState.Casting);
        StopMoving();

        if (anim != null)
        {
            anim.SetBool(walkBool, false);
            anim.SetTrigger(cloudGatherTrigger);
        }

        yield return new WaitForSeconds(cloudGatherDuration);

        if (anim != null)
        {
            anim.SetTrigger(cloudShootTrigger);
        }

        ShootCloud();

        EndAttack();
    }

    public void ShootCloud()
    {
        if (cloudPrefab == null)
        {
            Debug.LogWarning("Mambabarang cloudPrefab is missing.");
            return;
        }

        if (target == null)
        {
            return;
        }

        Vector3 spawnPosition = cloudSpawnPoint != null ? cloudSpawnPoint.position : transform.position;
        GameObject cloudObject = Instantiate(cloudPrefab, spawnPosition, Quaternion.identity);

        MambabarangCloud cloud = cloudObject.GetComponent<MambabarangCloud>();

        if (cloud != null)
        {
            cloud.SetTarget(target);
        }
    }

    private IEnumerator AOEAttackRoutine()
    {
        isCasting = true;
        ChangeState(BossState.Casting);
        StopMoving();

        if (anim != null)
        {
            anim.SetBool(walkBool, false);
            anim.SetTrigger(aoeTrigger);
        }

        yield return new WaitForSeconds(aoeGatherDuration);

        Vector3 aoePosition = target != null ? target.position : transform.position;

        GameObject warningObject = null;

        if (aoeWarningPrefab != null)
        {
            warningObject = Instantiate(aoeWarningPrefab, aoePosition, Quaternion.identity);
            warningObject.transform.localScale = new Vector3(aoeRadius * 2f, aoeRadius * 2f, 1f);
        }

        yield return new WaitForSeconds(aoeWarningDuration);

        DealAOEDamage(aoePosition);

        if (warningObject != null)
        {
            Destroy(warningObject);
        }

        EndAttack();
    }

    private void DealAOEDamage(Vector3 aoePosition)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(aoePosition, aoeRadius, playerLayer);

        foreach (Collider2D hit in hits)
        {
            StatPlayer player = hit.GetComponent<StatPlayer>();

            if (player == null)
            {
                player = hit.GetComponentInParent<StatPlayer>();
            }

            if (player != null)
            {
                player.TakeDamage(aoeDamage);
            }
        }
    }

    private void EndAttack()
    {
        isCasting = false;
        nextAttackTime = Time.time + attackDecisionCooldown;
        ChangeState(BossState.Chase);
    }

    private void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (anim != null)
        {
            anim.SetBool(walkBool, false);
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

    private void OnDisable()
    {
        StopAllCoroutines();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (boss != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, boss.detectionRange);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, boss.attackRange);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, aoeRadius);
    }
}