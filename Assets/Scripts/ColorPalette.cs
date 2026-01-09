using UnityEngine;

[System.Serializable]
public class ColorSet
{
    public Color primary;
    public Color secondary;
    public Color ui;
    public Color uiAccent;
}

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Accessibility/ColorPalette")]
public class ColorPalette : ScriptableObject
{
    public ColorSet[] palettes = new ColorSet[4];

    public ColorSet GetColorSet(int index)
    {
        if (index < 0 || index >= palettes.Length)
            return palettes[0];
        return palettes[index];
    }
}
