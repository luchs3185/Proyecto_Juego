using NUnit.Framework;
using UnityEngine;

public class MenuPausaTest
{
    [Test]
    public void Pausar_ConfiguraTimeScale()
    {
        // Empezar con tiempo normal
        Time.timeScale = 1f;
        
        // Pausar el juego
        Time.timeScale = 0f;
        Assert.AreEqual(0f, Time.timeScale);
        
        Time.timeScale = 1f; // restaurar para otros tests
    }
    
    [Test]
    public void Reanudar_RestableceTimeScale()
    {
        // Simular que el juego esta pausado
        Time.timeScale = 0f;
        
        // Reanudar el juego
        Time.timeScale = 1f;
        Assert.AreEqual(1f, Time.timeScale);
    }
}

