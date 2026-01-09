using UnityEngine;

public class TrapPlatform_Boss : MonoBehaviour
{
    [Header("Tiempo antes de desaparecer")]
    public float disappearDelay = 0.8f; // segundos antes de desaparecer
    public GameObject objectToActivateOnDeath;
    public GameObject Respawn;
    private bool triggered = false;

    private void OnCollisionEnter(Collision collision)
    {
        // Si choca con el jugador
        if (!triggered && collision.gameObject.CompareTag("Player"))
        {
            triggered = true;
            Invoke(nameof(Disappear), disappearDelay);
        }
    }

    private void Disappear()
    {
        if (objectToActivateOnDeath != null)
        {
            objectToActivateOnDeath.SetActive(true);
        }
        if (Respawn != null)
        {
            Respawn.SetActive(true);
        }
        gameObject.SetActive(false); // desaparece
    }
}
