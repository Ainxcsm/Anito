using UnityEngine;
using System.Collections;

public class GunAttack : MonoBehaviour
{
    public float damage = 12f;
    public float range = 3f;
    public Vector2 size = new Vector2(1f, 0.5f);

    private SpriteRenderer parentSprite;
    private Transform playerTransform;

    void Start()
    {
        parentSprite = transform.parent.GetComponent<SpriteRenderer>();
        playerTransform = transform.parent;
    }

    public void Attack()
    {
        // Start coroutine to wait for timing
        StartCoroutine(FireAfterDelay(0.22f)); // adjust 0.15 to match your animation
    }

    IEnumerator FireAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector2 origin = playerTransform.position;
        Vector2 direction = parentSprite.flipX ? Vector2.left : Vector2.right;
        origin += direction * (size.x / 2);

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, direction, range, LayerMask.GetMask("Enemy"));

        if (hit.collider != null)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Gun hit: " + enemy.name);
            }
        }

        Debug.DrawRay(origin, direction * range, Color.green, 0.2f);
    }
}
