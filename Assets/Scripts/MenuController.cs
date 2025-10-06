using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
public class MenuController : MonoBehaviour
{
    [Header("Controlador de menu")]
    [Tooltip("Menu UI para el modo debug")]
    public GameObject debugUI;
    public static bool debug = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Activa o desactiva el menu de debug
    public void ToggleDebugInfo(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            debug = !debug;
            ToggleMenu(debugUI);
        }
    }
    void ToggleMenu(GameObject menu)
    {
        menu.SetActive(!menu.activeSelf);
    }
}
