using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    public Player player;

    [Header("UI Controles")]
    [SerializeField] private GameObject menuControles;

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

    public void ToggleEasyMode()
    {

     player.easyMode=!player.easyMode;   
    }

    public void Cerrar()
    {
        Application.Quit(); 
    }
   
    //para mapa de controles
    public void AbrirControles()
    {
        menuPausa.SetActive(false); //el menu de pausa desaparece
        menuControles.SetActive(true); //el menu de controles aparece
    }

    public void CerrarControles()
    {
        menuControles.SetActive(false); //el menu de controles desaparece
        menuPausa.SetActive(true); //el menu de pausa aparece
    }

    [Header("UI Accesibilidad")]
    [SerializeField] private GameObject menuAccesibilidad;
    [SerializeField] private UnityEngine.UI.Image botonInvencibilidadImg;
    [SerializeField] private UnityEngine.UI.Image botonDaltonismoImg;

    public void AbrirAccesibilidad()
    {
        menuPausa.SetActive(false); //el menu de pausa desaparece
        menuAccesibilidad.SetActive(true); //el menu de accesibilidad aparece
        UpdateAccessibilityUI();
    }

    public void CerrarAccesibilidad()
    {
        menuAccesibilidad.SetActive(false); //el menu de accesibilidad desaparece
        menuPausa.SetActive(true); //el menu de pausa aparece
    }

    public void ToggleInvencibilidad()
    {
        player.invencible = !player.invencible;
        UpdateAccessibilityUI();
    }

    public void ToggleDaltonismo()
    {
        // Aquí iría la lógica futura de daltonismo
        // player.daltonismo = !player.daltonismo;
        UpdateAccessibilityUI();
    }

    private void UpdateAccessibilityUI()
    {
        // Cambiar color para indicar ON/OFF (Blanco = OFF, Verde = ON)
        if (botonInvencibilidadImg != null)
            botonInvencibilidadImg.color = player.invencible ? Color.green : Color.white;
            
        // if (botonDaltonismoImg != null)
        //    botonDaltonismoImg.color = player.daltonismo ? Color.green : Color.white; // Descomentar cuando exista
    }
}
