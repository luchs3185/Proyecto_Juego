using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    public Dialogue dialogue;               // El ScriptableObject con el diálogo
    public GameObject interactPrompt;       // UI de "Presiona W para hablar"

    [Header("Player Input")]
    public PlayerInput playerInput;         // Arrastrar el Player con PlayerInput

    private bool playerInside = false;      // Controla si el jugador está dentro del trigger
    public Action onDialogueEnd;            // Acción que se ejecuta al terminar el diálogo

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerInput != null)
            playerInput.actions["Dialogue"].performed += OnDialogue;
    }

    private void OnDisable()
    {
        if (playerInput != null)
            playerInput.actions["Dialogue"].performed -= OnDialogue;
    }

    // Método que se llama al pulsar W (acción Dialogue)
    public void OnDialogue(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (!playerInside) return;

        Debug.Log("Acción Dialogue detectada y jugador dentro del trigger");

        if (DialogueManager.Instance.IsActive())
            DialogueManager.Instance.DisplayNextLine();
        else
            StartDialogue();
    }

    private void StartDialogue()
    {
        if (dialogue == null)
        {
            Debug.LogWarning("Dialogue no asignado en DialogueTrigger de " + gameObject.name);
            return;
        }

        DialogueManager.Instance.ShowDialogue(dialogue);

        // Suscribirse al evento de cierre del diálogo
        DialogueManager.Instance.OnDialogueClosed += OnDialogueFinished;
    }

    private void OnDialogueFinished()
    {
        // Ejecuta la acción que haya sido asignada
        onDialogueEnd?.Invoke();

        // Desuscribirse para que no se repita
        DialogueManager.Instance.OnDialogueClosed -= OnDialogueFinished;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (interactPrompt != null)
                interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (interactPrompt != null)
                interactPrompt.SetActive(false);
        }
    }
}
