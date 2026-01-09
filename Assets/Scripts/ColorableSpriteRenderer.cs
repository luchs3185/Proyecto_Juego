using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorableSpriteRenderer : MonoBehaviour
{
    private SpriteRenderer sr;

    private void OnEnable()
    {
        sr = GetComponent<SpriteRenderer>();
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
        if (sr == null) return;
        ColorSet colors = ColorPaletteManager.Instance.GetCurrentColors();
        sr.color = colors.primary;
    }
}
