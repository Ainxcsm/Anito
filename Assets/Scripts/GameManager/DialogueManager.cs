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

    private int index;
    private bool isTyping;
    private bool started;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (player == null)
        {
            player = FindObjectOfType<Running>();
        }

        StartCoroutine(BeginDialogue());
    }

    IEnumerator BeginDialogue()
    {
        yield return new WaitForSeconds(1.5f);

        if (player != null)
        {
            player.SetDialogueLock(true);
        }
        else
        {
            Debug.LogError("DialogueManager could not find the player Running script.");
        }

        if (panel != null)
        {
            panel.SetActive(true);
        }

        started = true;
        index = 0;

        if (lines != null && lines.Length > 0)
        {
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void Update()
    {
        if (!started)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }

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
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        started = false;

        if (panel != null)
        {
            panel.SetActive(false);
        }

        StartCoroutine(UnlockDelay());
    }

    IEnumerator UnlockDelay()
    {
        yield return new WaitForSeconds(0.3f);

        if (player != null)
        {
            player.SetDialogueLock(false);
        }
    }
}