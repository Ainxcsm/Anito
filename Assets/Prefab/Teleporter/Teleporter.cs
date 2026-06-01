using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum TeleportMode
{
    SameScene,
    DifferentScene
}

public class Teleporter : MonoBehaviour, IInteractable
{
    private static Dictionary<string, Teleporter> teleporters = new Dictionary<string, Teleporter>();
    private static string pendingDestinationId = "";
    private static float lastTeleportTime = -999f;
    private static GameObject persistentPlayer;

    [Header("Teleporter ID")]
    public string teleporterId;

    [Header("Destination")]
    public TeleportMode teleportMode = TeleportMode.SameScene;
    public string destinationTeleporterId;
    public string targetSceneName;

    [Header("Positions")]
    public Transform centerPoint;
    public Transform spawnPoint;

    [Header("Animation")]
    public Animator teleporterAnimator;
    public string teleportTriggerName = "Teleport";
    public float teleportAnimationDelay = 0.75f;

    [Header("Optional Fade")]
    public ScreenFader screenFader;
    public float fadeDuration = 0.35f;

    [Header("Settings")]
    public bool useFade = true;
    public bool snapPlayerToCenter = true;
    public float interactCooldown = 0.75f;

    private bool isTeleporting = false;
    private GameObject currentPlayer;

    private void Awake()
    {
        if (teleporterAnimator == null)
        {
            teleporterAnimator = GetComponent<Animator>();
        }

        if (centerPoint == null)
        {
            centerPoint = transform;
        }

        if (spawnPoint == null)
        {
            spawnPoint = transform;
        }

        if (screenFader == null)
        {
            screenFader = FindObjectOfType<ScreenFader>();
        }
    }

    private void OnEnable()
    {
        if (!string.IsNullOrEmpty(teleporterId))
        {
            teleporters[teleporterId] = this;
        }
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(teleporterId) && teleporters.ContainsKey(teleporterId))
        {
            if (teleporters[teleporterId] == this)
            {
                teleporters.Remove(teleporterId);
            }
        }
    }

    private void Start()
    {
        if (!string.IsNullOrEmpty(pendingDestinationId) && pendingDestinationId == teleporterId)
        {
            StartCoroutine(SpawnPlayerAfterSceneLoad());
        }
    }

    public bool CanInteract()
    {
        if (isTeleporting)
        {
            return false;
        }

        if (Time.unscaledTime < lastTeleportTime + interactCooldown)
        {
            return false;
        }

        return true;
    }

    public void Interact()
    {
        if (!CanInteract())
        {
            return;
        }

        if (currentPlayer == null)
        {
            currentPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        if (currentPlayer == null)
        {
            Debug.LogError("Teleporter could not find Player. Make sure the player has the Player tag.");
            return;
        }

        StartCoroutine(TeleportRoutine(currentPlayer));
    }

    private IEnumerator TeleportRoutine(GameObject player)
    {
        isTeleporting = true;
        lastTeleportTime = Time.unscaledTime;

        LockPlayer(player, true);

        if (snapPlayerToCenter && centerPoint != null)
        {
            player.transform.position = centerPoint.position;
        }

        if (teleporterAnimator != null && !string.IsNullOrEmpty(teleportTriggerName))
        {
            teleporterAnimator.SetTrigger(teleportTriggerName);
        }

        yield return new WaitForSeconds(teleportAnimationDelay);

        if (useFade && screenFader != null)
        {
            yield return screenFader.FadeOut(fadeDuration);
        }

        if (teleportMode == TeleportMode.SameScene)
        {
            TeleportSameScene(player);
        }
        else
        {
            TeleportDifferentScene(player);
        }
    }

    private void TeleportSameScene(GameObject player)
    {
        Teleporter destination = GetDestinationTeleporter();

        if (destination == null)
        {
            Debug.LogError("Destination teleporter not found: " + destinationTeleporterId);
            LockPlayer(player, false);
            isTeleporting = false;
            return;
        }

        player.transform.position = destination.spawnPoint.position;

        LockPlayer(player, false);

        if (useFade && screenFader != null)
        {
            StartCoroutine(screenFader.FadeIn(fadeDuration));
        }

        isTeleporting = false;
    }

    private void TeleportDifferentScene(GameObject player)
    {
        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("Target Scene Name is empty.");
            LockPlayer(player, false);
            isTeleporting = false;
            return;
        }

        if (string.IsNullOrEmpty(destinationTeleporterId))
        {
            Debug.LogError("Destination Teleporter ID is empty.");
            LockPlayer(player, false);
            isTeleporting = false;
            return;
        }

        pendingDestinationId = destinationTeleporterId;
        persistentPlayer = player;

        DontDestroyOnLoad(player);

        SceneManager.LoadScene(targetSceneName);
    }

    private IEnumerator SpawnPlayerAfterSceneLoad()
    {
        yield return null;

        GameObject player = persistentPlayer;

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        if (player == null)
        {
            Running running = FindObjectOfType<Running>();

            if (running != null)
            {
                player = running.gameObject;
            }
        }

        if (player == null)
        {
            Debug.LogError("No Player found after scene load.");
            yield break;
        }

        player.transform.position = spawnPoint.position;

        LockPlayer(player, false);

        if (screenFader == null)
        {
            screenFader = FindObjectOfType<ScreenFader>();
        }

        if (useFade && screenFader != null)
        {
            yield return screenFader.FadeIn(fadeDuration);
        }

        pendingDestinationId = "";
        persistentPlayer = null;
        lastTeleportTime = Time.unscaledTime;
        isTeleporting = false;
    }

    private Teleporter GetDestinationTeleporter()
    {
        if (string.IsNullOrEmpty(destinationTeleporterId))
        {
            return null;
        }

        if (teleporters.ContainsKey(destinationTeleporterId))
        {
            return teleporters[destinationTeleporterId];
        }

        return null;
    }

    private void LockPlayer(GameObject player, bool locked)
    {
        Running running = player.GetComponent<Running>();

        if (running != null)
        {
            running.SetUILock(locked);
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        Animator animator = player.GetComponent<Animator>();

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            currentPlayer = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && currentPlayer == collision.gameObject)
        {
            currentPlayer = null;
        }
    }
}