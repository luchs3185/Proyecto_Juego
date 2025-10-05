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
    public float jumpForce = 20;
    public int maxJumps = 2;
    public int jumpsRemaining;
    private Rigidbody _rigidBody;
    private PlayerInput playerInput;
    private float direction;
    public static bool inGround;
    private float moveInput;
    public float stopDelay = 0.2f;
    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        jumpsRemaining = maxJumps;
    }

    void Update()
    {
        direction = playerInput.actions["Movement"].ReadValue<float>();

        if (playerInput.actions["jump"].triggered)
        {
            TryJump();
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

        DoJump();
        jumpsRemaining--;
        inGround = false;
    }

    private void DoJump()
    {
        _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, 0);
        _rigidBody.AddForce(100 * jumpForce * transform.up);
    }

    public void Crouch()
    {

    }
}
