using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public Enemy enemy;

    public float stopDist = 0.05f;
    public float cdTimer = 0f;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.freezeRotation = true;
        rb.gravityScale = 0;
    }

    void FixedUpdate()
    {
        if (target == null || enemy == null) return;

        if (cdTimer > 0f)
            cdTimer -= Time.fixedDeltaTime;

        float dist = Vector2.Distance(target.position, transform.position);

        bool isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");

        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (dist <= enemy.attackRange && cdTimer <= 0f)
        {
            cdTimer = enemy.attackCd;

            rb.linearVelocity = Vector2.zero;
            anim.SetTrigger("isAttack");

            return;
        }

        if (dist <= enemy.detectionRange && dist > stopDist)
        {
            anim.SetBool("isWalk", true);

            Vector2 dir = (target.position - transform.position).normalized;
            rb.linearVelocity = dir * enemy.speed;

            if (Mathf.Abs(dir.x) > 0.01f)
                spriteRenderer.flipX = dir.x > 0;
        }
        else
        {
            anim.SetBool("isWalk", false);
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void DealDamage()
    {
        if (target == null) return;

        var player = target.GetComponent<StatPlayer>();

        if (player != null)
            player.TakeDamage(enemy.damage);
    }
}