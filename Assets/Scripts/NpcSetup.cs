using UnityEngine;
using System; 

public class NPCSetup : MonoBehaviour
{
    public DialogueTrigger dialogueTrigger;
    public Player player;
    public GameObject objectToActivate;
    private void Start()
    {
        // Asignar qué variable activar al final del diálogo
        dialogueTrigger.onDialogueEnd = () =>
        {
            if (gameObject.name == "Rat_DIalogue"){
                player.dashobj = true;
                if (objectToActivate != null)
                    {
                        objectToActivate.SetActive(true);
                    }
            }
            else if (gameObject.name == "Crow"){
                player.maxJumps = 2;
                player.jumpsRemaining = 2;
                if (objectToActivate != null)
                    {
                        objectToActivate.SetActive(true);
                    }
            }
            else if (gameObject.name == "Topo"){
                player.digobj = true;
                if (objectToActivate != null)
                    {
                        objectToActivate.SetActive(true);
                    }
            }
        };
    }
}
