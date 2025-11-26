using UnityEngine;

public class FinalTrigger : MonoBehaviour
{
    public GameObject finalScreen; // asigna aquí el Canvas

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            finalScreen.SetActive(true); // activa la pantalla final
            Time.timeScale = 0f; // opcional: pausa el juego
        }
    }
}

