using UnityEngine;
using System;

public class ColorPaletteManager : MonoBehaviour
{
    public static ColorPaletteManager Instance { get; private set; }
    public event Action OnPaletteChanged;

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
        currentPaletteIndex = index;
        PlayerPrefs.SetInt("ColorPaletteIndex", index);
        
        // Avisa a todos los objetos que deben cambiar de color
        if (OnPaletteChanged != null) OnPaletteChanged.Invoke();
    }

    public ColorSet GetCurrentColors()
    {
        if (paletteAsset == null) return new ColorSet();
        return paletteAsset.GetModo(currentPaletteIndex);
    }
}