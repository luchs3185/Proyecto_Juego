using UnityEngine;

public class DigObj : MonoBehaviour
{
    [Header("Configuración del pickup")]
    /*public GameObject pickupEffect; // opcional, un efecto visual
    public AudioClip pickupSound;*/   // opcional, sonido
    public float destroyDelay = 0.1f;

    private bool collected = false;

    private void OnTriggerEnter(Collider other)
    {
        // Comprobamos si quien entra es el jugador
        Player player = other.GetComponent<Player>();

        if (player != null && !collected)
        {
            collected = true;

            // Aumentamos su número máximo de saltos
            player.digobj = true;

            //Efectos opcionales
           /* if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);*/

            //Destruimos el objeto
            //Destroy(gameObject, destroyDelay);
        }
    }
}
