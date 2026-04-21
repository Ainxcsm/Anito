using UnityEngine;
using Unity.Cinemachine;

public class MapTransition : MonoBehaviour
{
    [SerializeField] BoxCollider2D mapBoundary;
    [SerializeField] Direction direction;

    CinemachineConfiner2D confiner2D;

    enum Direction { Up, Down, Left, Right }

    private void Awake()
    {
        confiner2D = FindAnyObjectByType<CinemachineConfiner2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            confiner2D.BoundingShape2D = mapBoundary;
            UpdatePlayerPosition(collision.gameObject);
        }
    }

    private void UpdatePlayerPosition(GameObject player)
    {
        Vector3 newPos = player.transform.position;

        switch (direction)
        {
            case Direction.Up:
                newPos.y += .6f;
                break;
            case Direction.Down:
                newPos.y -= .6f;
                break;
            case Direction.Left:
                newPos.x -= .6f;
                break;
            case Direction.Right:
                newPos.x += .6f;
                break;
        }

        player.transform.position = newPos;
    }
}