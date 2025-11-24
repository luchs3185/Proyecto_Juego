using UnityEngine;

public class PortalConFade : MonoBehaviour
{
    // A dónde mandará este portal al jugador
    public Transform destino;

    private void OnTriggerEnter(Collider other)
    {
        // Miramos si el objeto que entra tiene el componente Player
        Player player = other.GetComponent<Player>();

        if (player != null && destino != null)
        {
            // Usamos el método del Player que hace fundido + teletransporte
            player.StartCoroutine(player.TeleportWithFade(destino.position));
        }
    }
}