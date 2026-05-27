using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResetSelectionOnClose : MonoBehaviour
{
    public Transform resetRoot;
    public float selectionBlockTime = 0.15f;

    public static bool IsSelectionBlocked { get; private set; }

    private void OnEnable()
    {
        StartCoroutine(ResetAfterFrame());
    }

    private void OnDisable()
    {
        ResetSelection();
    }

    private IEnumerator ResetAfterFrame()
    {
        IsSelectionBlocked = true;

        yield return null;

        ResetSelection();

        yield return new WaitForSecondsRealtime(selectionBlockTime);

        IsSelectionBlocked = false;
    }

    public void ResetSelection()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Transform root = resetRoot != null ? resetRoot : transform;

        Slot[] slots = root.GetComponentsInChildren<Slot>(true);

        foreach (Slot slot in slots)
        {
            slot.ClearSelectionVisual();
        }

        Toggle[] toggles = root.GetComponentsInChildren<Toggle>(true);

        foreach (Toggle toggle in toggles)
        {
            toggle.SetIsOnWithoutNotify(false);
        }

        Selectable[] selectables = root.GetComponentsInChildren<Selectable>(true);

        foreach (Selectable selectable in selectables)
        {
            selectable.OnDeselect(null);
        }

        Debug.Log("Inventory/UI selection reset.");
    }
}