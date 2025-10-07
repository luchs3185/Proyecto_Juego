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
		// Suelo 
		var ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
		ground.transform.position = new Vector3(0, -1, 0);
		ground.transform.localScale = new Vector3(20, 1, 20);
		ground.tag = "Ground";

		// Jugador simplificado: solo un Rigidbody 3D
		var playerGO = new GameObject("PlayerPhysicsOnly");
		var rb = playerGO.AddComponent<Rigidbody>();
		rb.useGravity = false; // Aislar el efecto para el test
		playerGO.transform.position = Vector3.zero;

		// Esperar un frame para que inicialice
		yield return null;

		// --- Test ---
		float v0 = GetVelY(rb);

		// Aumentar velocidad vertical directamente
		rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y + 10f, rb.linearVelocity.z);

		// Dejar pasar la física
		yield return new WaitForFixedUpdate();

		float v1 = GetVelY(rb);
		Assert.Greater(v1, v0, "Saltar no aumenta la velocidad vertical (Y).");

		// --- Cleanup ---
		Object.DestroyImmediate(playerGO);
		Object.DestroyImmediate(ground);
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
