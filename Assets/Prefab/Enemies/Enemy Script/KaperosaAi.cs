using System;
using UnityEngine;
using System.Collections;

public class KaperosaAI : MonoBehaviour
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

        bool isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack");

        float dist = Vector2.Distance(target.position, transform.position);

        // Stop movement if attacking
        if (isAttacking)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 velocity = Vector2.zero;

        // CHASE: player detected but not attacking
        if (dist <= enemy.detectionRange && dist > stopDist)
        {
            anim.SetBool("isWalk", true);

            // Play OpenMouth frozen pose
            anim.Play("OpenMouth", 0, 1f);  // jumps to last frame
            anim.speed = 0f;                // freeze the mouth open

            Vector2 dir = (target.position - transform.position).normalized;
            velocity = dir * enemy.speed;

            if (Mathf.Abs(dir.x) > 0.01f)
                spriteRenderer.flipX = dir.x > 0;
        }
        else
        {
            // Reset to Idle if player out of detection
            anim.SetBool("isWalk", false);
            anim.speed = 1f;
            anim.Play("Idle");
        }

        rb.linearVelocity = velocity;

        // ATTACK
        if (dist <= enemy.attackRange && cdTimer <= 0f)
        {
            cdTimer = enemy.attackCd;
            rb.linearVelocity = Vector2.zero;

            // Reset animator speed so Attack plays normally
            anim.speed = 1f;
            anim.SetTrigger("isAttack");
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
