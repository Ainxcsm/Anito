using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSkillController : MonoBehaviour
{
    public Animator playerAnimator;
    public SkillEffectController skillEffect;

    [Header("Skill Settings")]
    public float skillCooldown = 5f;
    public float skillCastDuration = 0.6f;

    [Header("Effect Timing Backup")]
    public bool useBackupEffectDelay = true;
    public float backupEffectDelay = 0.25f;

    [Header("Animator")]
    public string skillTriggerName = "Skill1";
    public string skillStateName = "Skill1-Anim";

    private float lastSkillTime = -Mathf.Infinity;
    private Running playerMovement;
    private SpriteRenderer playerSprite;
    private bool isCasting;
    private bool effectAlreadyPlayed;

    private void Awake()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }

        playerMovement = GetComponent<Running>();
        playerSprite = GetComponent<SpriteRenderer>();

        if (skillEffect != null)
        {
            skillEffect.HideEffect();
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            TryUseSkill();
        }
    }

    public void TryUseSkill()
    {
        if (isCasting)
        {
            return;
        }

        if (Time.time < lastSkillTime + skillCooldown)
        {
            Debug.Log("Skill is on cooldown.");
            return;
        }

        StartCoroutine(SkillRoutine());
    }

    private IEnumerator SkillRoutine()
    {
        isCasting = true;
        effectAlreadyPlayed = false;
        lastSkillTime = Time.time;

        if (playerMovement != null)
        {
            playerMovement.LockMovement();
        }

        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger(skillTriggerName);
            playerAnimator.SetTrigger(skillTriggerName);

            yield return null;

            playerAnimator.CrossFade(skillStateName, 0f);

            Debug.Log("Skill1 trigger sent.");
        }

        if (useBackupEffectDelay)
        {
            yield return new WaitForSeconds(backupEffectDelay);

            if (!effectAlreadyPlayed)
            {
                PlaySkillEffect();
                Debug.Log("Backup effect delay played the skill effect.");
            }

            float remainingTime = skillCastDuration - backupEffectDelay;

            if (remainingTime > 0f)
            {
                yield return new WaitForSeconds(remainingTime);
            }
        }
        else
        {
            yield return new WaitForSeconds(skillCastDuration);
        }

        EndSkill();
    }

    public void PlaySkillEffect()
    {
        if (effectAlreadyPlayed)
        {
            return;
        }

        effectAlreadyPlayed = true;

        if (skillEffect == null)
        {
            Debug.LogError("SkillEffectController is not assigned.");
            return;
        }

        bool facingLeft = false;

        if (playerSprite != null)
        {
            facingLeft = playerSprite.flipX;
        }

        skillEffect.SetFacing(facingLeft);
        skillEffect.PlaySkillEffect();

        Debug.Log("Skill effect played. Facing left: " + facingLeft);
    }

    public void EndSkill()
    {
        if (playerMovement != null)
        {
            playerMovement.UnlockMovement();
        }

        isCasting = false;
    }
}