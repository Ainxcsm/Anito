using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [Header("Health")]
    public Image healthFill;
    public StatPlayer player;
    public float healthSmoothSpeed = 5f;

    [Header("Click UI")]
    public Image leftClickImage;
    public Image rightClickImage;
    public Image spaceClickImage;

    public Sprite leftNormalSprite;
    public Sprite leftClickedSprite;
    public Sprite rightNormalSprite;
    public Sprite rightClickedSprite;
    public Sprite spaceNormalSprite;
    public Sprite spaceClickedSprite;

    public float clickDuration = 0.1f;

    private float targetHealthFill;

    void Start()
    {
        FindRuntimePlayer();
        ForceHealthBarUpdate();
    }

    void Update()
    {
        if (player == null || !player.gameObject.scene.isLoaded)
        {
            FindRuntimePlayer();
        }

        UpdateHealthBar();

        if (Input.GetMouseButtonDown(0))
        {
            LeftClickAction();
        }

        if (Input.GetMouseButtonDown(1))
        {
            RightClickAction();
        }

        if (Input.GetKey(KeyCode.Space))
        {
            SpaceClickAction();
        }
    }

    private void FindRuntimePlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            StatPlayer foundPlayer = playerObject.GetComponent<StatPlayer>();

            if (foundPlayer != null)
            {
                player = foundPlayer;
                Debug.Log("UIManager found runtime Player: " + player.gameObject.name);
                return;
            }
        }

        StatPlayer[] allPlayers = FindObjectsOfType<StatPlayer>(true);

        foreach (StatPlayer foundPlayer in allPlayers)
        {
            if (foundPlayer.gameObject.scene.isLoaded)
            {
                player = foundPlayer;
                Debug.Log("UIManager found scene StatPlayer: " + player.gameObject.name);
                return;
            }
        }

        Debug.LogWarning("UIManager could not find runtime Player.");
    }

    private void UpdateHealthBar()
    {
        if (player == null)
        {
            return;
        }

        if (healthFill == null)
        {
            Debug.LogWarning("UIManager healthFill is missing.");
            return;
        }

        float maxHealth = player.maxHealth;

        if (maxHealth <= 0f)
        {
            maxHealth = player.health;
        }

        if (maxHealth <= 0f)
        {
            return;
        }

        if (player.isDead)
        {
            targetHealthFill = 0f;
        }
        else
        {
            targetHealthFill = Mathf.Clamp01(player.currentHealth / maxHealth);
        }

        healthFill.fillAmount = Mathf.Lerp(
            healthFill.fillAmount,
            targetHealthFill,
            Time.deltaTime * healthSmoothSpeed
        );
    }

    private void ForceHealthBarUpdate()
    {
        if (player == null || healthFill == null)
        {
            return;
        }

        float maxHealth = player.maxHealth;

        if (maxHealth <= 0f)
        {
            maxHealth = player.health;
        }

        if (maxHealth <= 0f)
        {
            return;
        }

        healthFill.fillAmount = Mathf.Clamp01(player.currentHealth / maxHealth);
    }

    public void LeftClickAction()
    {
        if (leftClickImage != null)
        {
            StartCoroutine(FlashImage(leftClickImage, leftNormalSprite, leftClickedSprite));
        }
    }

    public void RightClickAction()
    {
        if (rightClickImage != null)
        {
            StartCoroutine(FlashImage(rightClickImage, rightNormalSprite, rightClickedSprite));
        }
    }

    public void SpaceClickAction()
    {
        if (spaceClickImage != null)
        {
            StartCoroutine(FlashImage(spaceClickImage, spaceNormalSprite, spaceClickedSprite));
        }
    }

    private IEnumerator FlashImage(Image img, Sprite normal, Sprite clicked)
    {
        if (img == null || normal == null || clicked == null)
        {
            Debug.LogWarning("FlashImage missing reference!");
            yield break;
        }

        img.sprite = clicked;
        yield return new WaitForSeconds(clickDuration);
        img.sprite = normal;
    }
}