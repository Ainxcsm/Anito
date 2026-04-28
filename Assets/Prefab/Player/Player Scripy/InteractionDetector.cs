using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private IInteractable interactableInRange = null;

    public GameObject interactionIcon;

    private bool interactLock;

    void Start()
    {
        if (interactionIcon != null)
            interactionIcon.SetActive(false);
    }

    public void TryInteract()
    {
        if (interactLock) return;

        if (interactableInRange == null) return;

        interactLock = true;

        Debug.Log("TRY INTERACT CALLED");

        interactableInRange.Interact();

        StartCoroutine(ResetLock());
    }

    private System.Collections.IEnumerator ResetLock()
    {
        yield return null; // wait 1 frame
        interactLock = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponentInParent<IInteractable>();

        if (interactable != null && interactable.CanInteract())
        {
            interactableInRange = interactable;

            if (interactionIcon != null)
                interactionIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponentInParent<IInteractable>();

        if (interactable != null && interactable == interactableInRange)
        {
            interactableInRange = null;

            if (interactionIcon != null)
                interactionIcon.SetActive(false);
        }
    }
}