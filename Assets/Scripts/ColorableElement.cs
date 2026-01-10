using UnityEngine;
using UnityEngine.UI;

public class ColorableElement : MonoBehaviour
{
    public enum ElementRole { Jugador, Enemigo, Obstaculo, Fondo, UI_Principal, UI_Acento, Vida }
    
    [Header("Configuración")]
    public ElementRole miRol;

    private SpriteRenderer sr;
    private Graphic uiGraphic;
    private Color colorOriginal; 
    private bool inicializado = false;

    private void Awake()
    {
        EnsureReferences();
    }

    private void EnsureReferences()
    {
        if (inicializado) return;
        
        sr = GetComponent<SpriteRenderer>();
        uiGraphic = GetComponent<Graphic>();

        // Guardamos tu arte original
        if (sr != null) colorOriginal = sr.color;
        else if (uiGraphic != null) colorOriginal = uiGraphic.color;
        else {
            Debug.LogWarning($"<color=red>OJO:</color> {gameObject.name} no tiene SpriteRenderer ni Graphic!");
        }
        
        inicializado = true;
    }

    private void OnEnable()
    {
        if (ColorPaletteManager.Instance != null)
        {
            ColorPaletteManager.Instance.OnPaletteChanged += ApplyColor;
            ApplyColor();
        }
    }

    private void OnDisable()
    {
        if (ColorPaletteManager.Instance != null)
            ColorPaletteManager.Instance.OnPaletteChanged -= ApplyColor;
    }

    public void ApplyColor()
    {
        EnsureReferences();
        if (ColorPaletteManager.Instance == null) return;

        int currentIdx = PlayerPrefs.GetInt("ColorPaletteIndex", 0);

        // MODO 0: NORMAL (Tu juego original)
        if (currentIdx == 0)
        {
            SetFinalColor(colorOriginal);
            return;
        }

        // MODOS DE DISCAPACIDAD
        ColorSet config = ColorPaletteManager.Instance.GetCurrentColors();
        if (config == null) return;

        Color colorAdaptado = Color.magenta; // Si lo ves rosa neón, es que el rol no existe

        switch (miRol)
        {
            case ElementRole.Jugador: colorAdaptado = config.jugador; break;
            case ElementRole.Enemigo: colorAdaptado = config.enemigo; break;
            case ElementRole.Obstaculo: colorAdaptado = config.obstaculo; break;
            case ElementRole.Fondo: colorAdaptado = config.fondo; break;
            case ElementRole.UI_Principal: colorAdaptado = config.uiPrincipal; break;
            case ElementRole.UI_Acento: colorAdaptado = config.uiAcento; break;
            case ElementRole.Vida: colorAdaptado = config.vida; break;
        }

        SetFinalColor(colorAdaptado);
    }

    private void SetFinalColor(Color c)
    {
        if (sr != null) sr.color = c;
        if (uiGraphic != null) uiGraphic.color = c;
    }
}