using UnityEngine;
using System.Collections;

public class WeaponAttack : MonoBehaviour
{
    public enum WeaponType
    {
        Sword,
        Gun
    }

    public WeaponType weaponType;
    public float damage = 10f;
    public Collider2D weaponCollider;

    [HideInInspector] public bool isActiveWeapon = false;

    private Vector3 originalLocalPos;
    private SpriteRenderer parentSprite;

    void Start()
    {
        originalLocalPos = transform.localPosition;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        if (transform.parent != null)
        {
            parentSprite = transform.parent.GetComponent<SpriteRenderer>();
        }
    }

    public void Attack()
    {
        if (!isActiveWeapon)
        {
            return;
        }

        PlayAttackSound();
        StartCoroutine(HitRoutine());
    }

    private void PlayAttackSound()
    {
        if (SFXManager.Instance == null)
        {
            return;
        }

        if (weaponType == WeaponType.Sword)
        {
            SFXManager.Instance.PlaySwordSlash();
        }
        else if (weaponType == WeaponType.Gun)
        {
            SFXManager.Instance.PlayGunShot();
        }
    }

    IEnumerator HitRoutine()
    {
        yield return new WaitForSeconds(0.1f);

        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }

        if (parentSprite != null && parentSprite.flipX)
        {
            transform.localPosition = new Vector3(-originalLocalPos.x, originalLocalPos.y, originalLocalPos.z);
        }
        else
        {
            transform.localPosition = originalLocalPos;
        }

        yield return new WaitForSeconds(0.15f);

        StopAttack();
    }

    public void StopAttack()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        transform.localPosition = originalLocalPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActiveWeapon)
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log(gameObject.name + " hit " + enemy.name);
            }
        }

        Breakable breakable = other.GetComponent<Breakable>();

        if (breakable != null)
        {
            breakable.TakeDamage(damage);
            Debug.Log(gameObject.name + " broke/hit " + breakable.name);
        }
    }
}