using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterName;
    public Animator animator;

    [Header("Player Input")]
    public PlayerInput playerInput;

    [Header("Typing Settings")]
    public float typingSpeed = 0.03f;

    private bool isDialogueActive = false;
    private Queue<DialogueLine> lines = new Queue<DialogueLine>();
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private string fullSentence;

    // Evento que se dispara cuando se cierra un diálogo
    public event Action OnDialogueClosed;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Ocultar el panel al inicio
        if (animator != null)
            animator.gameObject.SetActive(false); // El panel del diálogo
    }


    public void ShowDialogue(Dialogue dialogue)
    {
        if (dialogue == null || dialogue.dialogueLines.Length == 0)
        {
            Debug.LogWarning("Dialogue vacío o nulo.");
            return;
        }

        isDialogueActive = true;

        // Activar el panel antes de animar
        if (animator != null && !animator.gameObject.activeSelf)
            animator.gameObject.SetActive(true);

        animator.Play("Show");

        if (playerInput != null)
        {
            playerInput.actions["Movement"].Disable();
            playerInput.actions["Jump"].Disable();
        }

        lines.Clear();
        foreach (DialogueLine line in dialogue.dialogueLines)
            lines.Enqueue(line);

        DisplayNextLine();
    }


    public void DisplayNextLine()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine); // Detener solo la coroutine de tipeo
            dialogueText.text = fullSentence;
            isTyping = false;
            return;
        }

        if (lines.Count == 0)
        {
            CloseDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();
        characterName.text = currentLine.name;
        fullSentence = currentLine.line;

        typingCoroutine = StartCoroutine(TypeSentence(currentLine.line));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void CloseDialogue()
    {
        isDialogueActive = false;
        animator.Play("Hide");

        if (playerInput != null)
        {
            playerInput.actions["Movement"].Enable();
            playerInput.actions["Jump"].Enable();
        }

        OnDialogueClosed?.Invoke();

        // Espera que la animación termine y luego oculta el panel
        StartCoroutine(HidePanelAfterAnimation());
    }

    private IEnumerator HidePanelAfterAnimation()
    {
        yield return new WaitForSeconds(0.3f); // Duración de la animación Hide
        animator.gameObject.SetActive(false);
    }


    public bool IsActive() => isDialogueActive;
}
