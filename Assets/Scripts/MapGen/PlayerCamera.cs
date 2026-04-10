using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player; // assign in Inspector or auto-find by tag

    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 0, -10);
    public float smoothTime = 0.2f; // adjust for follow speed

    private Vector3 velocity = Vector3.zero;

    void FixedUpdate()
    {
        transform.LookAt(player);
    }

    void LateUpdate()
    {   

        if (player == null)
        {
            Debug.Log("No player assigned!");
            return;
        }
        Debug.Log("Camera Pos: " + transform.position + " | Target Pos: " + (player.position + offset));
    }


}
