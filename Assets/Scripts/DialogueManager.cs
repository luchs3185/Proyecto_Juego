using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using System;
using UnityEngine.InputSystem;



public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterName;

    private bool isDialogueActive = false;
    private Queue<DialogueLine> lines = new Queue<DialogueLine>();
    public float typingSpeed = 1f;
    public Animator animator;
    public PlayerInput playerInput;
    private void Start()
    {
        if (Instance == null)
            Instance = this;


    }

    public void ShowDialogue(Dialogue dialogue)
    {

        Debug.Log(dialogue.dialogueLines[0].line);
        isDialogueActive = true;
        animator.Play("Show");
        playerInput.actions["Movement"].Disable();
        playerInput.actions["Jump"].Disable();
        lines.Clear();
        foreach (DialogueLine line in dialogue.dialogueLines)
        {
            lines.Enqueue(line);
        }
        DisplayNextLine();
    }

    public void CloseDialogue()
    {
        isDialogueActive = false;
        animator.Play("Hide");
        playerInput.actions["Movement"].Enable();
        playerInput.actions["Jump"].Enable();
    }

    public void Update()
    {

    }

    public bool IsActive() => isDialogueActive;

    public void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            CloseDialogue();
            return;
        }
        DialogueLine currentLine = lines.Dequeue();

        characterName.text = currentLine.name;

        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentLine));
    }
    IEnumerator TypeSentence(DialogueLine line)
    {
        dialogueText.text = "";
        foreach (char letter in line.line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}