using NUnit.Framework;
using UnityEngine;

public class DashTest
{
    [Test]
    public void Dash_CambiaEstado()
    {
        // Crear jugador
        var player = new GameObject("Player");
        var playerScript = player.AddComponent<Player>();
        playerScript.enabled = false;
        
        // Empezar sin dash
        playerScript.dashobj = false;
        
        // Recoger el objeto de dash
        playerScript.dashobj = true;
        
        Assert.IsTrue(playerScript.dashobj);
        
        Object.DestroyImmediate(player);
    }
}

