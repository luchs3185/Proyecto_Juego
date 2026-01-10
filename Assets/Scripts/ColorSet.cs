using UnityEngine;

[System.Serializable]
public class ColorSet
{
    public string nombreModo; 
    
    [Header("Colores Adaptados")]
    public Color jugador = Color.white;
    public Color enemigo = Color.red;
    public Color obstaculo = Color.black;
    public Color fondo = Color.gray;
    public Color vida = Color.green; // Nueva variable para la salud

    [Header("Interfaz")]
    public Color uiPrincipal = Color.white;
    public Color uiAcento = Color.yellow;
}