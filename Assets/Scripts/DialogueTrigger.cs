using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security;

[Serializable]
public class DialogueLine
{
    public string name;

    [TextArea(3, 10)]
    public string line;
}

[Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();

}
public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    public Transform player;
    public float interactionDistance = 3f;
    private bool speaking = false;
    public void TriggerDialogue()
    {
        DialogueManager.Instance.ShowDialogue(dialogue);
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactionDistance && Input.GetKeyDown(KeyCode.N))
        {
            if (speaking)
            {
                DialogueManager.Instance.DisplayNextLine();
            }
            else
            {
                speaking = true;
                TriggerDialogue();

            }
        }
    }

}