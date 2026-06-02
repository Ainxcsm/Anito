using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class MapTransition : MonoBehaviour
{
    [SerializeField] BoxCollider2D mapBoundary;
    [SerializeField] Direction direction;

    CinemachineConfiner2D confiner2D;

    enum Direction { Up, Down, Left, Right, Upright, Upleft, Downleft, Downright }

    private void Awake()
    {
        confiner2D = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(HandleTransition(collision.gameObject));
        }
    }

    IEnumerator HandleTransition(GameObject player)
    {
        // 1. Move player first
        UpdatePlayerPosition(player);

        // 2. Wait one frame (IMPORTANT)
        yield return null;

        // 3. Then update camera bounds
        confiner2D.BoundingShape2D = mapBoundary;
        confiner2D.InvalidateCache();
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += .45f;
                break;
            case Direction.Upright:
                newPos.y += .45f;
                newPos.x += .45f;
                break;
            case Direction.Upleft:
                newPos.y += .45f;
                newPos.x -= .45f;
                break;
            case Direction.Down:
                newPos.y -= .45f;
                break;
            case Direction.Downright:
                newPos.x += 0.45f;
                newPos.y -= .45f;
                break;
            case Direction.Downleft:
                newPos.y -= 0.45f;
                newPos.x -= 0.45f;
                break;
            case Direction.Left:
                newPos.x -= .45f;
                break;
            case Direction.Right:
                newPos.x += .45f;
                break;
        }

        player.transform.position = newPos;
    }
}