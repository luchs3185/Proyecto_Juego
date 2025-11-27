using NUnit.Framework;
using UnityEngine;

public class DigTest
{
    [Test]
    public void Dig_CambiaEstado()
    {
        // Crear jugador
        var player = new GameObject("Player");
        var playerScript = player.AddComponent<Player>();
        playerScript.enabled = false;
        
        // Empezar sin dig
        playerScript.digobj = false;
        
        // Recoger el objeto de dig
        playerScript.digobj = true;
        
        Assert.IsTrue(playerScript.digobj);
        
        Object.DestroyImmediate(player);
    }
}

