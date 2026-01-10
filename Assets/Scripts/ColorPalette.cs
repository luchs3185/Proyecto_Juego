using UnityEngine;

[CreateAssetMenu(fileName = "NuevaPaleta", menuName = "Accesibilidad/Paleta Daltonismo")]
public class ColorPalette : ScriptableObject
{
    // 0: Normal, 1: Protanopia, 2: Deuteranopia, 3: Tritanopia
    public ColorSet[] modos = new ColorSet[4];

    public ColorSet GetModo(int index)
    {
        if (index < 0 || index >= modos.Length) return modos[0];
        return modos[index];
    }
}