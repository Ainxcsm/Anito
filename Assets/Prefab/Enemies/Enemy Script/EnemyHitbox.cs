using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public Enemy enemy; // assign automatically if null
    private bool isActive = false;

    void Start()
    {
        if(enemy == null)
            enemy = GetComponentInParent<Enemy>();
    }

    public void EnableHitbox()
    {
        isActive = true;
    }

    public void DisableHitbox()
    {
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!isActive) return;

        if(other.CompareTag("Player"))
        {
            var playerStats = other.GetComponent<StatPlayer>();
            if(playerStats != null)
            {
                playerStats.TakeDamage(enemy.damage);
            }
        }
    }
}
