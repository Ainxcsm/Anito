using UnityEngine;

[ExecuteAlways]
public class YSort : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Breakable Only Sorting")]
    public float yOffset = -0.15f;
    public int sortingOffset = 0;
    public int precision = 100;

    private Breakable breakable;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        breakable = GetComponent<Breakable>();

        ApplySorting();
    }

    private void LateUpdate()
    {
        ApplySorting();
    }

    private void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        breakable = GetComponent<Breakable>();

        ApplySorting();
    }

    private void ApplySorting()
    {
        if (breakable == null)
        {
            return;
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            return;
        }

        float sortY = transform.position.y + yOffset;
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-sortY * precision) + sortingOffset;
    }
}