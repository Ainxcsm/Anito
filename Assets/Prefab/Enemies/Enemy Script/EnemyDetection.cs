using UnityEngine;

[RequireComponent(typeof(EnemyAI))]
public class EnemyDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public LayerMask playerLayer;

    private EnemyAI enemyAI;
    private Transform player;

    void Start()
    {
        enemyAI = GetComponent<EnemyAI>();
        player = GameObject.FindWithTag("Player")?.transform;
    }

    void Update()
    {
        if(player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        // Enable or disable EnemyAI based on detection
        enemyAI.enabled = dist <= detectionRadius;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
