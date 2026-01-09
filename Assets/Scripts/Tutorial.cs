using UnityEngine;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
public class Tutorial : MonoBehaviour
{
    private bool playerInside = false;
    public GameObject interactPrompt; 
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
