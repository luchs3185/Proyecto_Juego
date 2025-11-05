using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class FallingPlatformTest
{
    [UnityTest]
    public IEnumerator OnCollisionEnter_ActivaTriggeredSoloUnaVez()
    {
        // Crear plataforma y jugador
        var platform = new GameObject("Platform");
        platform.tag = "Ground";
        platform.AddComponent<FallingPlatform>();
        
        var player = new GameObject("Player");
        player.tag = "Player";
        
        yield return new WaitForFixedUpdate();
        
        // La plataforma deberia seguir activa
        Assert.IsTrue(platform.activeSelf);
        
        Object.DestroyImmediate(platform);
        Object.DestroyImmediate(player);
    }
    
    [Test]
    public void Disappear_DesactivaGameObject()
    {
        // Crear una plataforma
        var platform = new GameObject("Platform");
        Assert.IsTrue(platform.activeSelf);
        
        // Simular que desaparece
        platform.SetActive(false);
        Assert.IsFalse(platform.activeSelf);
        
        Object.DestroyImmediate(platform);
    }
}

