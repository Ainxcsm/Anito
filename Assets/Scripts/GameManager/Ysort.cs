using UnityEngine;

public class YSort : MonoBehaviour
{
    public Transform sortPoint;
    public int sortingOffset = 0;
    public bool updateEveryFrame = false;

    private SpriteRenderer[] spriteRenderers;

    void Awake()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    void Start()
    {
        UpdateSortingOrder();
    }

    void LateUpdate()
    {
        if (updateEveryFrame)
        {
            UpdateSortingOrder();
        }
    }

    public void UpdateSortingOrder()
    {
        Vector3 positionToSort = sortPoint != null ? sortPoint.position : transform.position;
        int order = Mathf.RoundToInt(-positionToSort.y * 100) + sortingOffset;

        foreach (SpriteRenderer sr in spriteRenderers)
        {
            if (sr != null)
            {
                sr.sortingOrder = order;
            }
        }
    }
}