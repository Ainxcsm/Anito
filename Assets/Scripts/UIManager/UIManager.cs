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

    void Update()
    {
        if (player != null && healthFill != null)
        {
            if (player.isDead)
            {
                healthFill.fillAmount = 0f;
            }
            else
            {
                targetHealthFill = Mathf.Clamp01(player.currentHealth / player.health);
                healthFill.fillAmount = Mathf.Lerp(
                    healthFill.fillAmount,
                    targetHealthFill,
                    Time.deltaTime * healthSmoothSpeed
                );
            }
        }

        if (Input.GetMouseButtonDown(0)) LeftClickAction();
        if (Input.GetMouseButtonDown(1)) RightClickAction();
        if (Input.GetKey(KeyCode.Space)) SpaceClickAction();
    }

    public void LeftClickAction()
    {
        if (leftClickImage != null)
            StartCoroutine(FlashImage(leftClickImage, leftNormalSprite, leftClickedSprite));
    }

    public void RightClickAction()
    {
        if (rightClickImage != null)
            StartCoroutine(FlashImage(rightClickImage, rightNormalSprite, rightClickedSprite));
    }

    public void SpaceClickAction()
    {
        if (spaceClickImage != null)
            StartCoroutine(FlashImage(spaceClickImage, spaceNormalSprite, spaceClickedSprite));
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