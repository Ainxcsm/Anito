using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    [Header("Enemy Specific Sounds")]
    public AudioClip attackSound;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip alertSound;

    [Header("Volume")]
    public float volume = 1f;

    public void PlayAttack()
    {
        PlaySound(attackSound);
    }

    public void PlayHit()
    {
        PlaySound(hitSound);
    }

    public void PlayDeath()
    {
        PlaySound(deathSound);
    }

    public void PlayAlert()
    {
        PlaySound(alertSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            return;
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlaySFX(clip, volume);
        }
    }
}