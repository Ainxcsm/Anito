using System.Collections;
using UnityEngine;

public class SkillEffectController : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Collider2D skillCollider;
    public SkillStunHitbox stunHitbox;

    [Header("Effect Settings")]
    public string castTriggerName = "Cast";
    public float hideDelay = 0.5f;

    [Header("Manual Facing Positions")]
    public Vector3 rightLocalPosition = new Vector3(0.6f, 0f, 0f);
    public Vector3 leftLocalPosition = new Vector3(-0.6f, 0f, 0f);

    [Header("Manual Facing Scale")]
    public Vector3 rightLocalScale = new Vector3(1f, 1f, 1f);
    public Vector3 leftLocalScale = new Vector3(-1f, 1f, 1f);

    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (skillCollider == null)
        {
            skillCollider = GetComponent<Collider2D>();
        }

        if (stunHitbox == null)
        {
            stunHitbox = GetComponent<SkillStunHitbox>();
        }

        HideEffect();
    }

    public void SetFacing(bool facingLeft)
    {
        if (facingLeft)
        {
            transform.localPosition = leftLocalPosition;
            transform.localScale = leftLocalScale;
        }
        else
        {
            transform.localPosition = rightLocalPosition;
            transform.localScale = rightLocalScale;
        }
    }

    public void PlaySkillEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sortingLayerName = "Effects";
            spriteRenderer.sortingOrder = 100;
        }

        if (skillCollider != null)
        {
            skillCollider.enabled = true;
        }

        if (stunHitbox != null)
        {
            stunHitbox.ResetHitbox();
        }

        if (animator != null)
        {
            animator.ResetTrigger(castTriggerName);
            animator.SetTrigger(castTriggerName);
        }

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        HideEffect();
    }

    public void HideEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }

        if (skillCollider != null)
        {
            skillCollider.enabled = false;
        }
    }
}