using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Player : MonoBehaviour
{

    [Header("Movimiento del personaje")]
    [Range(5, 20)]
    public float moveSpeed = 10;
    [Range(10, 100)]
    public float jumpForce = 65;
    public int maxJumps = 1;
    public int jumpsRemaining;
    private Rigidbody _rigidBody;
    private PlayerInput playerInput;
    private float direction;
    public static bool inGround;
    private float moveInput;
    public float stopDelay = 0.2f;
    private float mayJump = 0f;       // Tiempo que aún puedes saltar después de salir del suelo
    public float coyoteTime = 0.5f;  // Duración de coyote time
    private float jumpBufferTimer = 0f;       // Temporizador del buffer de salto
    public float jumpBufferTime = 0.01f;      // Cuánto tiempo recordamos el input
    
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        transform.rotation = Quaternion.identity; 
        direction = playerInput.actions["Movement"].ReadValue<float>();
        
        if (inGround)
        {
            mayJump = coyoteTime;  // Resetear el coyote time cuando estés en el suelo
        }
        else
        {
            mayJump -= Time.deltaTime;  // Contar hacia atrás cuando no estás en el suelo
            mayJump = Mathf.Max(mayJump, 0f); // evitar negativos
        }
            if (playerInput.actions["jump"].triggered)
            {
                jumpBufferTimer = jumpBufferTime;
                   
                
        }
        jumpBufferTimer -= Time.deltaTime;
             
                jumpBufferTimer = Mathf.Max(jumpBufferTimer, 0f);
        if (jumpBufferTimer > 0f && (mayJump > 0f || maxJumps == 2))
        {
            TryJump();
            jumpBufferTimer = 0f; // consumimos el buffer
        }
    }

    private void FixedUpdate()
    {
        float targetX;

        if (Mathf.Abs(moveInput) > 0.01f)
        {
            targetX = moveInput * moveSpeed;
        }
        else
        {
                targetX = Mathf.Lerp(_rigidBody.linearVelocity.x, 0f, Time.fixedDeltaTime / stopDelay);
        }
        if (targetX != 0)
            _rigidBody.linearVelocity = new Vector2(targetX, _rigidBody.linearVelocity.y);
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;

            if (Vector3.Angle(normal, Vector3.up) < 30f)
            {
                {
                    inGround = true;
                    jumpsRemaining = maxJumps;
                }
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Ground"))
            inGround = false;
    }
    public void Move(CallbackContext context)
    {
        if (context.canceled)
        {
            moveInput = 0f;
        }
        else if (context.performed)
        {
            moveInput = context.ReadValue<float>();
        }
    }
    private void TryJump()
    {
         
    if (jumpsRemaining <= 0)
            return;
    if((maxJumps == 1 && mayJump > 0f ) || maxJumps==2){
        
            DoJump();
            jumpsRemaining--;
            mayJump = 0f;
            inGround = false;
           
        }
       
    }

    private void DoJump()
    {
      _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, 0); // reinicia la velocidad vertical
    _rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public void Crouch()
    {

    }
}
