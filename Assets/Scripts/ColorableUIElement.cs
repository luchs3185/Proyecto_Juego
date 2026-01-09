using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Graphic))]
public class ColorableUIElement : MonoBehaviour
{
    private Graphic graphic;

    private void OnEnable()
    {
        graphic = GetComponent<Graphic>();
        if (ColorPaletteManager.Instance != null)
        {
            ColorPaletteManager.Instance.OnPaletteChanged += UpdateColor;
            UpdateColor();
        }
    }

    private void OnDisable()
    {
        if (ColorPaletteManager.Instance != null)
            ColorPaletteManager.Instance.OnPaletteChanged -= UpdateColor;
    }

    private void UpdateColor()
    {
        if (graphic == null) return;
        ColorSet colors = ColorPaletteManager.Instance.GetCurrentColors();
        graphic.color = colors.ui;
    }
}
