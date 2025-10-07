using NUnit.Framework;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.TestTools;

public class PlayerSmokePlayModeTest
{
    [UnityTest]
    public IEnumerator Saltar_SubeEnY_Y_ConsumeUnSalto()
    {
        // --- Setup mínimo en el propio test ---
        // Suelo (el tag "Ground" es importante para vuestro OnCollisionExit)
        var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.transform.position = new Vector3(0, -1, 0);
        ground.transform.localScale = new Vector3(20, 1, 20);
        ground.tag = "Ground";

        // Jugador
        var playerGO = new GameObject("Player");
        var rb = playerGO.AddComponent<Rigidbody>(); // 3D
        playerGO.AddComponent<UnityEngine.InputSystem.PlayerInput>();
        var player = playerGO.AddComponent<Player>();
        playerGO.transform.position = Vector3.zero;

        // Esperar un frame para que corra Start()
        yield return null;

        // --- Test ---
        Player.inGround = true;
        player.jumpsRemaining = player.maxJumps;

        float v0 = GetVelY(rb);

        // Invocar TryJump() (método privado) vía reflexión
        var tryJump = typeof(Player).GetMethod("TryJump", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(tryJump, "No se encontró TryJump en Player.");
        tryJump.Invoke(player, null);

        // Dejar pasar la física
        yield return new WaitForFixedUpdate();

        float v1 = GetVelY(rb);
        Assert.Greater(v1, v0, "Saltar no aumenta la velocidad vertical (Y).");
        Assert.AreEqual(player.maxJumps - 1, player.jumpsRemaining, "No se consumió un salto.");

        // --- Cleanup ---
        Object.Destroy(playerGO);
        Object.Destroy(ground);
        yield return null;
    }

    // Soporta proyectos con Rigidbody.velocity o .linearVelocity
    float GetVelY(Rigidbody body)
    {
        var prop = typeof(Rigidbody).GetProperty("linearVelocity");
        if (prop != null) return ((Vector3)prop.GetValue(body)).y;
        return body.linearVelocity.y;
    }
}
