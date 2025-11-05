using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;

public class FollowObjectTest
{
    [UnityTest]
    public IEnumerator LateUpdate_CalculaPosicionConOffsets()
    {
        // Crear el jugador en una posicion
        var player = new GameObject("Player");
        player.transform.position = new Vector3(5f, 2f, 0f);
        
        // Crear la camara con el script FollowObject
        var camera = new GameObject("Camera");
        var follow = camera.AddComponent<FollowObject>();
        follow.target = player.transform;
        follow.verticalOffset = 10f;
        follow.horizontalOffset = 3f;
        
        yield return null; // esperar un frame para que se ejecute LateUpdate
        
        // Verificar que la camara esta en la posicion correcta (player + offsets)
        Assert.AreEqual(8f, camera.transform.position.x, 0.1f);
        Assert.AreEqual(12f, camera.transform.position.y, 0.1f);
        
        Object.DestroyImmediate(player);
        Object.DestroyImmediate(camera);
    }
}

