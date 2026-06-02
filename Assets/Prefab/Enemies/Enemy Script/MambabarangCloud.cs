using System.Collections;
using UnityEngine;

public class MambabarangCloud : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 4f;
    public float lifeTime = 2.5f;
    public float popDistance = 0.25f;

    [Header("Beetle Spawn")]
    public GameObject beetlePrefab;
    public int beetleCount = 3;
    public float beetleSpawnRadius = 0.6f;

    [Header("Animation")]
    public Animator anim;
    public string popTrigger = "Pop";
    public float destroyDelayAfterPop = 0.35f;

    private Transform target;
    private Vector2 targetPosition;
    private bool hasPopped = false;

    private void Awake()
    {
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        StartCoroutine(LifeTimer());
    }

    private void Update()
    {
        if (hasPopped)
        {
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        float distance = Vector2.Distance(transform.position, targetPosition);

        if (distance <= popDistance)
        {
            Pop();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            targetPosition = target.position;
        }
        else
        {
            targetPosition = transform.position;
        }
    }

    private IEnumerator LifeTimer()
    {
        yield return new WaitForSeconds(lifeTime);

        if (!hasPopped)
        {
            Pop();
        }
    }

    private void Pop()
    {
        if (hasPopped)
        {
            return;
        }

        hasPopped = true;

        SpawnBeetles();

        if (anim != null && !string.IsNullOrEmpty(popTrigger))
        {
            anim.SetTrigger(popTrigger);
            Destroy(gameObject, destroyDelayAfterPop);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SpawnBeetles()
    {
        if (beetlePrefab == null)
        {
            Debug.LogWarning("MambabarangCloud beetlePrefab is missing.");
            return;
        }

        for (int i = 0; i < beetleCount; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle.normalized * beetleSpawnRadius;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);

            Instantiate(beetlePrefab, spawnPosition, Quaternion.identity);
        }
    }
}