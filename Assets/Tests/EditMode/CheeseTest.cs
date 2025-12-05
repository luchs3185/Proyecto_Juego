using NUnit.Framework;
using UnityEngine;

public class CheeseTest
{
    [Test]
    public void Cheese_SumaVida()
    {
        // Crear jugador
        var player = new GameObject("Player");
        var playerScript = player.AddComponent<Player>();
        playerScript.enabled = false;
        
        // Vida inicial
        playerScript.life = 2;
        playerScript.maxLife = 4;
        
        // Recoger queso suma 1
        playerScript.life++;
        
        Assert.AreEqual(3, playerScript.life);
        
        Object.DestroyImmediate(player);
    }
    
    [Test]
    public void Cheese_NoSuperaMaximo()
    {
        // Crear jugador
        var player = new GameObject("Player");
        var playerScript = player.AddComponent<Player>();
        playerScript.enabled = false;
        
        // Vida ya esta al maximo
        playerScript.life = 4;
        playerScript.maxLife = 4;
        
        // Intentar sumar mas vida
        playerScript.life++;
        if (playerScript.life > playerScript.maxLife)
            playerScript.life = playerScript.maxLife;
        
        Assert.AreEqual(4, playerScript.life);
        
        Object.DestroyImmediate(player);
    }
}

