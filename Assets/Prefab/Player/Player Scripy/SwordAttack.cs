using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public float damage = 10f;
    public Collider2D weaponCollider;

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
        weaponCollider.enabled = false;
    }

    public void AttackRight()
    {
        weaponCollider.enabled = true;
        transform.localPosition = originalPosition;
    }

    public void AttackLeft()
    {
        weaponCollider.enabled = true;
        transform.localPosition = new Vector3(-originalPosition.x, originalPosition.y, originalPosition.z);
    }

    public void StopAttack()
    {
        weaponCollider.enabled = false;
        transform.localPosition = originalPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null) enemy.TakeDamage(damage);
        }
    }
}
