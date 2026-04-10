using System;
using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    public float stopDist = 0.05f;
    public float cdTimer = 0f;

    public Enemy enemy;
    public StatPlayer statplayer;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rb.freezeRotation = true;
        rb.gravityScale = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
{
    if (target == null)
        return;

    if (cdTimer > 0f)
        cdTimer -= Time.fixedDeltaTime;

    float dist = Vector2.Distance(target.position, transform.position);
    bool isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");

    // Stop movement if currently attacking
    if (isAttacking)
    {
        rb.linearVelocity = Vector2.zero;
        return;
    }

    // Attack check first
    if (dist <= enemy.attackRange && cdTimer <= 0f)
    {
        cdTimer = enemy.attackCd;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("isAttack");
        return; // prevent moving this frame
    }

    // Movement
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
        statplayer = target.GetComponent<StatPlayer>();
        if (statplayer != null)
            statplayer.TakeDamage(enemy.damage);
    }
}
