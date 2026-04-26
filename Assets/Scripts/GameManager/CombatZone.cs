using UnityEngine;
using System.Collections.Generic;

public class CombatZone : MonoBehaviour
{
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform[] spawnPoints;
    [SerializeField] int enemyCount = 3;

    [SerializeField] GameObject[] gates;

    List<GameObject> enemies = new List<GameObject>();

    bool activated = false;
    bool finished = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated && collision.CompareTag("Player"))
        {
            activated = true;
            Debug.Log("Combat Started");
            StartCombat();
        }
    }

    void StartCombat()
    {
        // LOCK GATES + DEBUG
        foreach (GameObject gate in gates)
        {
            if (gate == null)
            {
                Debug.LogWarning("NULL gate found in array!");
                continue;
            }

            Debug.Log("Closing gate: " + gate.name);
            gate.SetActive(true);
        }

        // SPAWN ENEMIES
        for (int i = 0; i < enemyCount; i++)
        {
            if (spawnPoints.Length == 0)
            {
                Debug.LogError("No spawn points assigned!");
                return;
            }

            Transform spawn = spawnPoints[i % spawnPoints.Length];

            if (enemyPrefab == null)
            {
                Debug.LogError("Enemy prefab is NULL!");
                return;
            }

            GameObject enemy = Instantiate(enemyPrefab, spawn.position, Quaternion.identity);

            // FORCE FIX visibility issues
            enemy.transform.position = new Vector3(spawn.position.x, spawn.position.y, 0);
            enemy.transform.localScale = Vector3.one;

            Debug.Log("Spawned enemy at: " + enemy.transform.position);

            enemies.Add(enemy);
        }
    }

    void Update()
    {
        if (!activated || finished) return;

        enemies.RemoveAll(e => e == null);

        if (enemies.Count == 0)
        {
            finished = true;
            EndCombat();
        }
    }

    void EndCombat()
    {
        Debug.Log("Combat Finished - Opening gates");

        foreach (GameObject gate in gates)
        {
            if (gate == null)
            {
                Debug.LogWarning("NULL gate in array on EndCombat!");
                continue;
            }

            Debug.Log("Opening gate: " + gate.name);
            gate.SetActive(false);
        }
    }
}