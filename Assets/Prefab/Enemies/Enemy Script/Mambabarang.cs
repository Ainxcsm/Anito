using UnityEngine;

public class Mambabarang : Enemy
{
    [Header("Mambabarang Boss Info")]
    public string bossName = "Mambabarang";

    [Header("Boss Settings")]
    public bool isBoss = true;

    private void Reset()
    {
        ApplyRecommendedBossStats();
    }

    [ContextMenu("Apply Recommended Boss Stats")]
    public void ApplyRecommendedBossStats()
    {
        maxHealth = 250f;
        speed = 1.75f;
        damage = 15f;
        armor = 3f;
        attackRange = 7f;
        detectionRange = 12f;
        attackCd = 2.25f;

        dropCoins = true;
        minCoins = 20;
        maxCoins = 35;
        coinDropSpread = 0.75f;
    }
}