using UnityEngine;

public class YSort : MonoBehaviour
{
    public int sortingOffset = 0;
    public bool updateEveryFrame = false;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (spriteRenderer == null)
        {
            return;
        }

        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 10) + sortingOffset;
    }
}