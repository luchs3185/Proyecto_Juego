using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    public Player player;

    [Header("UI Controles")]
    [SerializeField] private GameObject menuControles;

    [Header("UI Accesibilidad")]
    [SerializeField] private GameObject menuAccesibilidad;
    [SerializeField] private Image botonInvencibilidadImg;
    [SerializeField] private Image botonDaltonismoImg;
    [SerializeField] private Text textoDaltonismo; // Muestra: Normal, Protanopia, etc.

    // --- MÉTODOS DE PAUSA ---
    public void Pausar()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void Reanudar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
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
   
    // --- MAPA DE CONTROLES ---
    public void AbrirControles()
    {
        menuPausa.SetActive(false);
        menuControles.SetActive(true);
    }

    public void CerrarControles()
    {
        menuControles.SetActive(false);
        menuPausa.SetActive(true);
    }

    // --- ACCESIBILIDAD ---
    public void AbrirAccesibilidad()
    {
        menuPausa.SetActive(false);
        menuAccesibilidad.SetActive(true);
        UpdateAccessibilityUI(); 
    }

    public void CerrarAccesibilidad()
    {
        menuAccesibilidad.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void ToggleInvencibilidad()
    {
        player.invencible = !player.invencible;
        UpdateAccessibilityUI();
    }

    /// <summary>
    /// Cambia cíclicamente entre: 0 (Normal), 1 (Protanopia), 2 (Deuteranopia), 3 (Tritanopia)
    /// </summary>
    public void ToggleDaltonismo()
    {
        // 1. Obtenemos el índice actual guardado en PlayerPrefs
        int currentIndex = PlayerPrefs.GetInt("ColorPaletteIndex", 0);

        // 2. Calculamos el siguiente índice (0 -> 1 -> 2 -> 3 -> 0)
        int nextIndex = (currentIndex + 1) % 4;

        // 3. Le decimos al Manager que cambie la paleta globalmente
        if (ColorPaletteManager.Instance != null)
        {
            ColorPaletteManager.Instance.SetPalette(nextIndex);
        }

        // 4. Actualizamos la UI del menú
        UpdateAccessibilityUI();
    }

    private void UpdateAccessibilityUI()
    {
        // Feedback Invencibilidad
        if (botonInvencibilidadImg != null)
            botonInvencibilidadImg.color = player.invencible ? Color.green : Color.white;
            
        // Feedback Daltonismo
        int currentIndex = PlayerPrefs.GetInt("ColorPaletteIndex", 0);

        if (botonDaltonismoImg != null)
        {
            // Si es modo Normal (0) blanco, si es cualquier daltónico verde
            botonDaltonismoImg.color = (currentIndex == 0) ? Color.white : Color.green;
        }

        if (textoDaltonismo != null)
        {
            string[] nombresModos = { "Modo: Normal", "Modo: Protanopia", "Modo: Deuteranopia", "Modo: Tritanopia" };
            textoDaltonismo.text = nombresModos[currentIndex];
        }
    }
}   