using System.Collections.Generic;
using UnityEngine;

public class SkillStunHitbox : MonoBehaviour
{
    [Header("Stun Settings")]
    public float stunDuration = 2f;
    public bool affectOnlyOncePerCast = true;

    private readonly HashSet<EnemyAI> stunnedEnemies = new HashSet<EnemyAI>();

    private void OnEnable()
    {
        stunnedEnemies.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStunEnemy(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryStunEnemy(other);
    }

    private void TryStunEnemy(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            return;
        }

        EnemyAI enemyAI = other.GetComponent<EnemyAI>();

        if (enemyAI == null)
        {
            enemyAI = other.GetComponentInParent<EnemyAI>();
        }

        if (enemyAI == null)
        {
            return;
        }

        if (affectOnlyOncePerCast && stunnedEnemies.Contains(enemyAI))
        {
            return;
        }

        stunnedEnemies.Add(enemyAI);
        enemyAI.Stun(stunDuration);

        Debug.Log("Stunned enemy: " + enemyAI.name);
    }

    public void ResetHitbox()
    {
        stunnedEnemies.Clear();
    }
}