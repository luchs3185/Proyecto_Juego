using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Tiempo antes de desaparecer")]
    public float disappearDelay = 0.2f; // segundos antes de desaparecer

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
        gameObject.SetActive(false); // desaparece
    }
}
