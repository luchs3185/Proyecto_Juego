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
		// Jugador simplificado: solo un Rigidbody 3D sin gravedad
		var playerGO = new GameObject("PlayerPhysicsOnly");
		var rb = playerGO.AddComponent<Rigidbody>();
		rb.useGravity = false;
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
