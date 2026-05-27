using UnityEngine;
using UnityEngine.EventSystems;

public class UITabPlayerLock : MonoBehaviour
{
    private Running player;

    private void Awake()
    {
        player = FindAnyObjectByType<Running>();
    }

    private void OnEnable()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Running>();
        }

        if (player != null)
        {
            player.SetUILock(true);
        }
    }

    private void OnDisable()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        if (player == null)
        {
            player = FindAnyObjectByType<Running>();
        }

        if (player != null)
        {
            player.SetUILock(false);
        }
    }
}