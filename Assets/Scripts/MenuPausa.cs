using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Necesario para manipular la UI

public class MenuPausa : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    public Player player;

    [Header("UI Controles")]
    [SerializeField] private GameObject menuControles;

    // --- MÉTODOS DE PAUSA (Igual que antes) ---
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

    public void ToggleEasyMode()
    {
        player.easyMode = !player.easyMode;   
    }

    public void Cerrar()
    {
        Application.Quit(); 
    }
   
    // --- MAPA DE CONTROLES (Igual que antes) ---
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

    // --- ACCESIBILIDAD (AQUÍ ESTÁ LA NUEVA LÓGICA) ---
    [Header("UI Accesibilidad")]
    [SerializeField] private GameObject menuAccesibilidad;
    [SerializeField] private Image botonInvencibilidadImg;
    [SerializeField] private Image botonDaltonismoImg;
    
    // Opcional: Si quieres mostrar texto diciendo qué modo es (ej: "Protanopia")
    [SerializeField] private Text textoDaltonismo; 

    public void AbrirAccesibilidad()
    {
        menuPausa.SetActive(false);
        menuAccesibilidad.SetActive(true);
        UpdateAccessibilityUI(); // Actualiza visualmente los botones al abrir
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

    public void ToggleDaltonismo()
    {
        // 1. Obtenemos el índice actual guardado en PlayerPrefs (por defecto 0)
        int currentIndex = PlayerPrefs.GetInt("ColorPaletteIndex", 0);

        // 2. Calculamos el siguiente índice (0 -> 1 -> 2 -> 3 -> 0)
        // El operador % 4 asegura que si llega a 4, vuelva a 0.
        int nextIndex = (currentIndex + 1) % 4;

        // 3. Le decimos al Manager que cambie la paleta
        if (ColorPaletteManager.Instance != null)
        {
            ColorPaletteManager.Instance.SetPalette(nextIndex);
        }

        // 4. Actualizamos la UI para reflejar el cambio
        UpdateAccessibilityUI();
    }

    private void UpdateAccessibilityUI()
    {
        // --- Feedback Visual Invencibilidad ---
        if (botonInvencibilidadImg != null)
            botonInvencibilidadImg.color = player.invencible ? Color.green : Color.white;
            
        // --- Feedback Visual Daltonismo ---
        if (botonDaltonismoImg != null)
        {
            // Leemos qué modo está activo actualmente
            int currentIndex = PlayerPrefs.GetInt("ColorPaletteIndex", 0);

            // Si el índice es 0 (Normal), botón blanco. Si es > 0 (Daltónico), botón verde.
            botonDaltonismoImg.color = (currentIndex == 0) ? Color.white : Color.green;
        }

        // (Opcional) Actualizar texto si tienes una referencia de Texto
        if (textoDaltonismo != null)
        {
            string[] nombresModos = { "Normal", "Protanopia", "Deuteranopia", "Tritanopia" };
            int index = PlayerPrefs.GetInt("ColorPaletteIndex", 0);
            textoDaltonismo.text = nombresModos[index]; // Muestra el nombre del modo
        }
    }
}