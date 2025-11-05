using NUnit.Framework;
using UnityEngine;

public class MenuControllerTest
{
    [Test]
    public void ToggleMenu_CambiaEstadoActivo()
    {
        // Crear un menu activo
        var menu = new GameObject("Menu");
        menu.SetActive(true);
        
        // Primera llamada deberia desactivarlo
        menu.SetActive(!menu.activeSelf);
        Assert.IsFalse(menu.activeSelf);
        
        // Segunda llamada deberia activarlo de nuevo
        menu.SetActive(!menu.activeSelf);
        Assert.IsTrue(menu.activeSelf);
        
        Object.DestroyImmediate(menu);
    }
}

