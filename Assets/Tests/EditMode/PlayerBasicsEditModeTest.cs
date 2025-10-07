using NUnit.Framework;
using UnityEngine;

public class PlayerBasicsEditModeTest
{
    [Test]
    public void Player_TieneComponentesBasicosYValoresCoherentes()
    {
        var go = new GameObject("Player");
        var rb = go.AddComponent<Rigidbody>(); // 3D
        var pi = go.AddComponent<UnityEngine.InputSystem.PlayerInput>(); // Input System
        var player = go.AddComponent<Player>();

        Assert.IsNotNull(rb, "Falta Rigidbody en Player.");
        Assert.IsNotNull(pi, "Falta PlayerInput en Player.");
        Assert.GreaterOrEqual(player.maxJumps, 1, "maxJumps debe ser al menos 1.");
    }
}
