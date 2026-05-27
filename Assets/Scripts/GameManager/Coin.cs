using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    [HideInInspector] public bool canCollect = false;

    public void Collect()
    {
        if (!canCollect)
        {
            return;
        }

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoins(value);
        }
        else
        {
            Debug.LogError("CoinManager not found.");
        }

        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayItemPickup();
        }

        Destroy(gameObject);
    }
}