using System.Collections;
using UnityEngine;

public class CoinBounce : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float bounceHeight = 0.2f;
    public float bounceDuration = 0.35f;
    public float spreadRadius = 0.35f;
    public float collectDelay = 0.15f;

    [Header("Wall Safety")]
    public LayerMask wallLayer;
    public float coinRadius = 0.08f;
    public int safePositionAttempts = 12;

    private Coin coin;
    private Collider2D coinCollider;
    private bool hasBounced;

    private void Awake()
    {
        coin = GetComponent<Coin>();
        coinCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        if (!hasBounced)
        {
            hasBounced = true;
            StartCoroutine(BounceRoutine());
        }
    }

    private IEnumerator BounceRoutine()
    {
        if (coin != null)
        {
            coin.canCollect = false;
        }

        if (coinCollider != null)
        {
            coinCollider.enabled = false;
        }

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = GetSafeTargetPosition(startPosition);

        float timer = 0f;

        while (timer < bounceDuration)
        {
            timer += Time.deltaTime;

            float t = timer / bounceDuration;
            float height = Mathf.Sin(t * Mathf.PI) * bounceHeight;

            transform.position = Vector3.Lerp(startPosition, targetPosition, t) + Vector3.up * height;

            yield return null;
        }

        transform.position = targetPosition;

        yield return new WaitForSeconds(collectDelay);

        if (coinCollider != null)
        {
            coinCollider.enabled = true;
        }

        if (coin != null)
        {
            coin.canCollect = true;
        }
    }

    private Vector3 GetSafeTargetPosition(Vector3 startPosition)
    {
        for (int i = 0; i < safePositionAttempts; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * spreadRadius;
            Vector3 possiblePosition = startPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

            bool targetInsideWall = Physics2D.OverlapCircle(possiblePosition, coinRadius, wallLayer) != null;

            if (targetInsideWall)
            {
                continue;
            }

            Vector2 direction = possiblePosition - startPosition;
            float distance = direction.magnitude;

            if (distance <= 0.01f)
            {
                return startPosition;
            }

            RaycastHit2D wallBetween = Physics2D.CircleCast(
                startPosition,
                coinRadius,
                direction.normalized,
                distance,
                wallLayer
            );

            if (wallBetween.collider == null)
            {
                return possiblePosition;
            }
        }

        return startPosition;
    }
}