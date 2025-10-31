using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;

    public void Pausar()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false); //el boton de pausa desaparece
        menuPausa.SetActive(true); //el menu de pausa aparece
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true); //el boton de pausa aparece
        menuPausa.SetActive(false); //el menu de pausa desaparece
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Cerrar()
    {
        Application.Quit(); 
    }
}
