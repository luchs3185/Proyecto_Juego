using UnityEngine;

public class ColorPaletteManager : MonoBehaviour
{
    public static ColorPaletteManager Instance { get; private set; }
    public System.Action OnPaletteChanged;

    [SerializeField] private ColorPalette paletteAsset;
    private int currentPaletteIndex = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        currentPaletteIndex = PlayerPrefs.GetInt("ColorPaletteIndex", 0);
    }

    public void SetPalette(int index)
    {
        if (index < 0 || index > 3) return;
        currentPaletteIndex = index;
        PlayerPrefs.SetInt("ColorPaletteIndex", index);
        OnPaletteChanged?.Invoke();
    }

    public ColorSet GetCurrentColors()
    {
        if (paletteAsset == null)
            return new ColorSet();
        return paletteAsset.GetColorSet(currentPaletteIndex);
    }

    public ColorSet GetPaletteByIndex(int index)
    {
        if (paletteAsset == null)
            return new ColorSet();
        return paletteAsset.GetColorSet(index);
    }
}
