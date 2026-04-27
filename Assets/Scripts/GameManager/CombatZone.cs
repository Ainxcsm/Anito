using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatZone : MonoBehaviour
{
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] Transform[] spawnPoints;

    [SerializeField] GameObject[] gates;

    [SerializeField] int waves = 3;
    [SerializeField] int enemiesPerWave = 2;
    [SerializeField] float timeBetweenWaves = 1.5f;

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

        StartCoroutine(HandleWaves());
    }

    IEnumerator HandleWaves()
    {
        for (int w = 0; w < waves; w++)
        {
            Debug.Log("Starting Wave " + (w + 1));

            SpawnWave(enemiesPerWave);

            // wait until all enemies are dead
            yield return new WaitUntil(() => enemies.Count == 0);

            Debug.Log("Wave " + (w + 1) + " cleared");

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        finished = true;
        EndCombat();
    }

    void SpawnWave(int count)
{
    enemies.Clear();

    for (int i = 0; i < count; i++)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }

        if (enemyPrefabs.Length == 0)
        {
            Debug.LogError("No enemy prefabs assigned!");
            return;
        }

        Transform spawn = spawnPoints[i % spawnPoints.Length];

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(prefab, spawn.position, Quaternion.identity);

        enemy.transform.position = new Vector3(spawn.position.x, spawn.position.y, 0);
        enemy.transform.localScale = Vector3.one;

        Debug.Log("Spawned: " + prefab.name);

        enemies.Add(enemy);
    }
}

    void Update()
    {
        if (!activated || finished) return;

        enemies.RemoveAll(e => e == null);
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