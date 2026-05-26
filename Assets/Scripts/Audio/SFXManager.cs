using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Item / Interaction")]
    public AudioClip itemPickup;
    public AudioClip chestOpen;
    public AudioClip breakableBreak;
    public AudioClip teleporter;

    [Header("Player")]
    public AudioClip dash;
    public AudioClip swordSlash;
    public AudioClip gunShot;

    [Header("Default Enemy Sounds")]
    public AudioClip enemyHit;
    public AudioClip enemyDeath;
    public AudioClip enemyAttack;

    [Header("Audio Settings")]
    public float sfxVolume = 1f;

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    public void PlaySFX(AudioClip clip)
    {
        PlaySFX(clip, sfxVolume);
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            return;
        }

        audioSource.PlayOneShot(clip, volume);
    }

    public void PlayItemPickup()
    {
        PlaySFX(itemPickup);
    }

    public void PlayChestOpen()
    {
        PlaySFX(chestOpen);
    }

    public void PlayBreakableBreak()
    {
        PlaySFX(breakableBreak);
    }

    public void PlayTeleporter()
    {
        PlaySFX(teleporter);
    }

    public void PlayDash()
    {
        PlaySFX(dash);
    }

    public void PlaySwordSlash()
    {
        PlaySFX(swordSlash);
    }

    public void PlayGunShot()
    {
        PlaySFX(gunShot);
    }

    public void PlayEnemyHit()
    {
        PlaySFX(enemyHit);
    }

    public void PlayEnemyDeath()
    {
        PlaySFX(enemyDeath);
    }

    public void PlayEnemyAttack()
    {
        PlaySFX(enemyAttack);
    }
}