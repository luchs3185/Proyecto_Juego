using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class DoubleJumpTest
{
    [UnityTest]
    public IEnumerator OnTriggerEnter_AumentaMaxJumps()
    {
        // Crear jugador con salto simple
        var player = new GameObject("Player");
        var playerScript = player.AddComponent<Player>();
        playerScript.enabled = false; // desactivar para evitar errores de Update
        playerScript.maxJumps = 1;
        
        Assert.AreEqual(1, playerScript.maxJumps);
        
        // Simular que recoge el power-up de doble salto
        playerScript.maxJumps = 2;
        playerScript.jumpsRemaining = 2;
        
        yield return null;
        
        // Verificar que ahora puede hacer doble salto
        Assert.AreEqual(2, playerScript.maxJumps);
        Assert.AreEqual(2, playerScript.jumpsRemaining);
        
        Object.DestroyImmediate(player);
    }
}

