using UnityEngine;
using System; 

public class NPCSetup : MonoBehaviour
{
    public DialogueTrigger dialogueTrigger;
    public Player player;

    private void Start()
    {
        // Asignar qué variable activar al final del diálogo
        dialogueTrigger.onDialogueEnd = () =>
        {
            if (gameObject.name == "Rat_DIalogue")
                player.dashobj = true;
            else if (gameObject.name == "Crow")
                player.maxJumps = 2;
            else if (gameObject.name == "Topo")
                player.digobj = true;
        };
    }
}
