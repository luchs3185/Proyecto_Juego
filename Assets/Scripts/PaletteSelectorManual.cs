using UnityEngine;
using UnityEngine.UI;

public class PaletteSelectorManual : MonoBehaviour
{
    public Button normalButton;
    public Button protanopiaButton;
    public Button deuteranopiaButton;
    public Button tritanopiaButton;
    public Button backButton;

    private void Start()
    {
        if (normalButton != null) normalButton.onClick.AddListener(() => Select(0));
        if (protanopiaButton != null) protanopiaButton.onClick.AddListener(() => Select(1));
        if (deuteranopiaButton != null) deuteranopiaButton.onClick.AddListener(() => Select(2));
        if (tritanopiaButton != null) tritanopiaButton.onClick.AddListener(() => Select(3));
        if (backButton != null) backButton.onClick.AddListener(ClosePanel);
    }

    public void Select(int index)
    {
        if (ColorPaletteManager.Instance != null)
            ColorPaletteManager.Instance.SetPalette(index);
    }

    public void ClosePanel()
    {
        gameObject.SetActive(false);
    }
}
