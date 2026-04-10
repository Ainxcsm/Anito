using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    private Animator animator;
    private bool isAttacking;
    private Running running;

    void Start()
    {
        animator = GetComponent<Animator>();
        running = GetComponent<Running>();
        animator.SetLayerWeight(1, 1f);
    }

    void Update()
    {
        if (isAttacking) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            AttackMelee();

        if (Mouse.current.rightButton.wasPressedThisFrame)
            AttackGun();
    }

    void AttackMelee()
    {
        isAttacking = true;
        animator.SetTrigger("Attack"); // melee animation
        running.AttackMelee();
    }

    void AttackGun()
    {
        isAttacking = true;
        animator.SetTrigger("AttackR"); // gun animation
        running.AttackGun();
    }

    // Call this from animation event at the end of attack
    public void EndAttackEvent()
    {
        isAttacking = false;
        running.EndAttack();
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("AttackR");
    }
}
