using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using System.Reflection;
using UnityEditor.Animations;
using UnityEngine.InputSystem;

public class PlayerFunctionalTest{
    [UnityTest]
    public IEnumerator TryJump_ConsumeUnSalto()
    {
        var playerObj = new GameObject("PlayerSimple");
        var body = playerObj.AddComponent<Rigidbody>();
        body.useGravity = false;

        var player = playerObj.AddComponent<Player>();
        player.maxJumps = 2;
        player.jumpsRemaining = 2;
        Player.inGround = true;

        // Para desactivar input y animación automática
        player.enabled = false;

        // Asignar referencias requeridas 
        typeof(Player).GetField("_rigidBody", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(player, body);
        typeof(Player).GetField("_animator", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.SetValue(player, playerObj.AddComponent<Animator>());

        yield return null;

        int antes = player.jumpsRemaining;
        var tryJump = typeof(Player).GetMethod("TryJump", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        tryJump.Invoke(player, null);

        yield return null;

        Assert.AreEqual(antes - 1, player.jumpsRemaining, "TryJump() no consumió un salto.");

        Object.DestroyImmediate(playerObj);
    }

    [UnityTest]
    public IEnumerator DoJump_AplicaFuerzaVertical()
    {
        var playerObj = new GameObject("PlayerSimple");
        var body = playerObj.AddComponent<Rigidbody>();
        body.useGravity = false;

        var player = playerObj.AddComponent<Player>();
        player.jumpForce = 70f;

        player.enabled = false;

        typeof(Player).GetField("_rigidBody", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(player, body);
        typeof(Player).GetField("_animator", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.SetValue(player, playerObj.AddComponent<Animator>());

        body.linearVelocity = new Vector3(0, -10f, 0); // se le agrega una velocidad vertical hacia abajo

        yield return null;

        var doJump = typeof(Player).GetMethod("DoJump", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        doJump.Invoke(player, null);

        yield return new WaitForFixedUpdate(); // esperar un frame de física

        float velY = body.linearVelocity.y;
        Assert.Greater(velY, 0f, "DoJump() no aplicó fuerza vertical hacia arriba.");

        Object.DestroyImmediate(playerObj);
    }

    [UnityTest]
    public IEnumerator OnTriggerEnter_DetectaAgua()
    {
        var playerObj = new GameObject("PlayerSimple");
        var body = playerObj.AddComponent<Rigidbody>();
        var player = playerObj.AddComponent<Player>();

        player.enabled = false;

        typeof(Player).GetField("_rigidBody", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(player, body);

        // Crear InputActionAsset con todas las acciones necesarias
        var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        var map = new InputActionMap("Gameplay");
        map.AddAction("Movement", InputActionType.Value).AddBinding("<Gamepad>/leftStick");
        map.AddAction("reset", InputActionType.Button).AddBinding("<Keyboard>/r");
        map.AddAction("jump", InputActionType.Button).AddBinding("<Keyboard>/space");
        map.AddAction("Dash", InputActionType.Button).AddBinding("<Keyboard>/shift");
        map.AddAction("Dig", InputActionType.Button).AddBinding("<Keyboard>/ctrl");
        map.AddAction("Attack", InputActionType.Button).AddBinding("<Keyboard>/x");
        asset.AddActionMap(map);

        var input = playerObj.AddComponent<PlayerInput>();
        input.actions = asset;
        input.defaultActionMap = "Gameplay";

        typeof(Player).GetField("playerInput", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(player, input);

        var waterObj = new GameObject("Water");
        waterObj.tag = "Water";
        waterObj.AddComponent<BoxCollider>();
        waterObj.GetComponent<Collider>().isTrigger = true;

        //Asegura que el jugador no ha tocado agua antes
        typeof(Player).GetField("touchWater", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.SetValue(player, false);

        yield return null;

        //para simular la colisión con el agua
        var onTrigger = typeof(Player).GetMethod("OnTriggerEnter", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        onTrigger.Invoke(player, new object[] { waterObj.GetComponent<Collider>() });

        yield return null;

        bool touchedWater = (bool)(typeof(Player).GetField("touchWater", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)?.GetValue(player) ?? false);

        Assert.IsTrue(touchedWater, "OnTriggerEnter no detectó la colisión con un objeto con tag 'Water'");

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(waterObj);
    }

    [UnityTest]
    public IEnumerator OnCollisionEnter_ReseteaSaltos()
    {
        var playerObj = new GameObject("PlayerSimple");
        var body = playerObj.AddComponent<Rigidbody>();
        playerObj.AddComponent<CapsuleCollider>();
        body.useGravity = true;

        var player = playerObj.AddComponent<Player>();
        player.maxJumps = 2;
        player.jumpsRemaining = 0;
        Player.inGround = false;

        player.enabled = false;

        var animator = playerObj.AddComponent<Animator>();
        var controller = new AnimatorController();
        controller.AddLayer("Base Layer");
        controller.AddParameter("isJumping", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
        animator.runtimeAnimatorController = controller;

        //PlayerInput con mapa mínimo para evadir errores
        var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        var map = new InputActionMap("Gameplay");
        map.AddAction("Movement", InputActionType.Value).AddBinding("<Gamepad>/leftStick");
        map.AddAction("reset", InputActionType.Button).AddBinding("<Keyboard>/r");
        map.AddAction("jump", InputActionType.Button).AddBinding("<Keyboard>/space");
        map.AddAction("Dash", InputActionType.Button).AddBinding("<Keyboard>/shift");
        map.AddAction("Dig", InputActionType.Button).AddBinding("<Keyboard>/ctrl");
        map.AddAction("Attack", InputActionType.Button).AddBinding("<Keyboard>/x");
        asset.AddActionMap(map);

        var input = playerObj.AddComponent<PlayerInput>();
        input.actions = asset;
        input.defaultActionMap = "Gameplay";

        typeof(Player).GetField("_rigidBody", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(player, body);
        typeof(Player).GetField("_animator", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)?.SetValue(player, animator);

        var sueloObj = new GameObject("Suelo");
        sueloObj.tag = "Ground";
        var sueloCollider = sueloObj.AddComponent<BoxCollider>();
        sueloCollider.size = new Vector3(10, 1, 10);
        sueloObj.transform.position = new Vector3(0, 0, 0);

        playerObj.transform.position = new Vector3(0, 2, 0); // encima del suelo

        player.enabled = true;

        float timeout = 5f;
        while (!Player.inGround && timeout > 0f)//esperar a que toque el suelo
        {
            yield return new WaitForFixedUpdate();
            timeout -= Time.fixedDeltaTime;
        }

        Assert.IsTrue(Player.inGround, "OnCollisionEnter no detectó la colisión con el suelo.");
        Assert.AreEqual(player.maxJumps, player.jumpsRemaining, "OnCollisionEnter no reseteó los saltos restantes al máximo.");

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(sueloObj);
    }

    [UnityTest]
    public IEnumerator OnCollisionExit_DesactivaInGround(){
        var playerObj = new GameObject("PlayerSimple");
        var body = playerObj.AddComponent<Rigidbody>();
        body.useGravity = true;
        playerObj.AddComponent<CapsuleCollider>();

        var player = playerObj.AddComponent<Player>();
        player.maxJumps = 2;
        player.jumpsRemaining = 0;
        Player.inGround = false;

        // Asignar campos necesarios para evitar errores
        var spriteRenderer = playerObj.AddComponent<SpriteRenderer>();
        var animator = playerObj.AddComponent<Animator>();
        var controller = new AnimatorController();
        controller.AddLayer("Base Layer");
        controller.AddParameter("isJumping", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isWalking", AnimatorControllerParameterType.Bool);
        controller.AddParameter("isDashing", AnimatorControllerParameterType.Bool);
        animator.runtimeAnimatorController = controller;

        player._animator = animator;

        var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        var map = new InputActionMap("Gameplay");
        map.AddAction("Movement", InputActionType.Value).AddBinding("<Gamepad>/leftStick");
        map.AddAction("reset", InputActionType.Button).AddBinding("<Keyboard>/r");
        map.AddAction("jump", InputActionType.Button).AddBinding("<Keyboard>/space");
        map.AddAction("Dash", InputActionType.Button).AddBinding("<Keyboard>/shift");
        map.AddAction("Dig", InputActionType.Button).AddBinding("<Keyboard>/ctrl");
        map.AddAction("Attack", InputActionType.Button).AddBinding("<Keyboard>/x");
        asset.AddActionMap(map);
        var input = playerObj.AddComponent<PlayerInput>();
        input.actions = asset;
        input.defaultActionMap = "Gameplay";

        typeof(Player).GetField("_rigidBody", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(player, body);
        typeof(Player).GetField("_spriteRenderer", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(player, spriteRenderer);
        typeof(Player).GetField("_animator", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(player, animator);
        typeof(Player).GetField("playerInput", BindingFlags.NonPublic | BindingFlags.Instance)?.SetValue(player, input);

        var sueloObj = new GameObject("Suelo");
        sueloObj.tag = "Ground";
        var sueloCollider = sueloObj.AddComponent<BoxCollider>();
        sueloCollider.size = new Vector3(10, 1, 10);
        sueloObj.transform.position = Vector3.zero;

        // jugador sobre el suelo
        playerObj.transform.position = new Vector3(0, 2, 0);

        player.enabled = true;

        // Esperar a que toque el suelo
        float timeout = 5f;
        while (!Player.inGround && timeout > 0f)
        {
            yield return new WaitForFixedUpdate();
            timeout -= Time.fixedDeltaTime;
        }

        Assert.IsTrue(Player.inGround, "El jugador no detectó el suelo.");

        // Simular salto para saltar del suelo
        body.AddForce(Vector3.up * 10f, ForceMode.Impulse);

        // Esperar a que se separe del suelo
        timeout = 5f;
        while (Player.inGround && timeout > 0f)
        {
            yield return new WaitForFixedUpdate();
            timeout -= Time.fixedDeltaTime;
        }

        Assert.IsFalse(Player.inGround, "El jugador no salió del suelo (OnCollisionExit no se activó).");

        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(sueloObj);
    }

}