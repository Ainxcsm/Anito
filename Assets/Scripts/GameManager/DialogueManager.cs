using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public GameObject panel;

    public Running player;

    public string[] lines;
    public float textSpeed = 0.03f;

    int index;
    bool isTyping;
    bool started;

    void Start()
    {
        panel.SetActive(false);
        StartCoroutine(BeginDialogue());
    }

    IEnumerator BeginDialogue()
    {
        yield return new WaitForSeconds(1.5f); // fade buffer

        player.LockMovement();

        panel.SetActive(true);
        started = true;

        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (!started) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in lines[index])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;
    }

    void NextLine()
    {
        index++;

        if (index < lines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        panel.SetActive(false);
        StartCoroutine(UnlockDelay());
    }

    IEnumerator UnlockDelay()
    {
        yield return new WaitForSeconds(0.3f);
        player.UnlockMovement();
    }
}