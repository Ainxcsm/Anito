using System.Collections.Generic;
using UnityEngine;

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

    [Header("Hit Detection")]
    public LayerMask damageLayers;

    [HideInInspector] public bool isActiveWeapon = false;

    private Vector3 originalLocalPos;
    private SpriteRenderer parentSprite;

    private float currentAttackDamage;

    private readonly Collider2D[] overlapResults = new Collider2D[32];
    private readonly HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    private void Awake()
    {
        originalLocalPos = transform.localPosition;
        currentAttackDamage = damage;

        if (weaponCollider == null)
        {
            weaponCollider = GetComponent<Collider2D>();
        }

        if (weaponCollider != null)
        {
            weaponCollider.isTrigger = true;
            weaponCollider.enabled = false;
        }

        if (transform.parent != null)
        {
            parentSprite = transform.parent.GetComponent<SpriteRenderer>();
        }
    }

    public void Attack()
    {
        Attack(damage);
    }

    public void Attack(float attackDamage)
    {
        if (!isActiveWeapon)
        {
            return;
        }

        currentAttackDamage = attackDamage;

        hitObjects.Clear();
        FaceWeapon();

        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }

        PlayAttackSound();

        CheckHits();
    }

    public void CheckHits()
    {
        if (!isActiveWeapon)
        {
            return;
        }

        if (weaponCollider == null)
        {
            return;
        }

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        if (damageLayers.value != 0)
        {
            filter.useLayerMask = true;
            filter.SetLayerMask(damageLayers);
        }
        else
        {
            filter.useLayerMask = false;
        }

        int count = weaponCollider.Overlap(filter, overlapResults);

        for (int i = 0; i < count; i++)
        {
            Collider2D hit = overlapResults[i];

            if (hit == null)
            {
                continue;
            }

            TryHit(hit);
        }
    }

    public void StopAttack()
    {
        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }

        isActiveWeapon = false;
        hitObjects.Clear();
        transform.localPosition = originalLocalPos;
        currentAttackDamage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
    }

    private void TryHit(Collider2D other)
    {
        if (!isActiveWeapon)
        {
            return;
        }

        if (!LayerAllowed(other.gameObject.layer))
        {
            return;
        }

        Enemy enemy = other.GetComponent<Enemy>();

        if (enemy == null)
        {
            enemy = other.GetComponentInParent<Enemy>();
        }

        if (enemy != null)
        {
            if (hitObjects.Contains(enemy.gameObject))
            {
                return;
            }

            hitObjects.Add(enemy.gameObject);
            enemy.TakeDamage(currentAttackDamage);

            Debug.Log(gameObject.name + " hit enemy: " + enemy.name + " for " + currentAttackDamage);
            return;
        }

        Breakable breakable = other.GetComponent<Breakable>();

        if (breakable == null)
        {
            breakable = other.GetComponentInParent<Breakable>();
        }

        if (breakable != null)
        {
            if (hitObjects.Contains(breakable.gameObject))
            {
                return;
            }

            hitObjects.Add(breakable.gameObject);
            breakable.TakeDamage(currentAttackDamage);

            Debug.Log(gameObject.name + " hit breakable: " + breakable.name + " for " + currentAttackDamage);
        }
    }

    private bool LayerAllowed(int layer)
    {
        if (damageLayers.value == 0)
        {
            return true;
        }

        return (damageLayers.value & (1 << layer)) != 0;
    }

    private void FaceWeapon()
    {
        if (parentSprite != null && parentSprite.flipX)
        {
            transform.localPosition = new Vector3(-Mathf.Abs(originalLocalPos.x), originalLocalPos.y, originalLocalPos.z);
        }
        else
        {
            transform.localPosition = new Vector3(Mathf.Abs(originalLocalPos.x), originalLocalPos.y, originalLocalPos.z);
        }
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
}