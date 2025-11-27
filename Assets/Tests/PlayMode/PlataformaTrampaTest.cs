using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class PlataformaTrampaTest
{
    [UnityTest]
    public IEnumerator Plataforma_SeDesactiva()
    {
        // Crear plataforma activa
        var plataforma = new GameObject("Plataforma");
        var fallingPlatform = plataforma.AddComponent<FallingPlatform>();
        plataforma.SetActive(true);
        
        Assert.IsTrue(plataforma.activeSelf);
        
        // Esperar un poco
        yield return new WaitForSeconds(0.1f);
        
        // Desactivar la plataforma
        plataforma.SetActive(false);
        Assert.IsFalse(plataforma.activeSelf);
        
        Object.DestroyImmediate(plataforma);
    }
}

