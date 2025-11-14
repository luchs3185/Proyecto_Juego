using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Runtime.CompilerServices;

public class Player : MonoBehaviour
{

    [Header("Movimiento del personaje")]
    [Range(5, 20)]
    public float moveSpeed = 10;
    [Range(10, 100)]

    [Header("Salto")]
    public float jumpForce = 70;
    public int maxJumps = 1;
    public int jumpnum = 0;
    public int jumpsRemaining;
    private Rigidbody _rigidBody;
    private PlayerInput playerInput;
    private float direction;
    public static bool inGround;
    public static bool isDigging = false;
    private float moveInput;
    public float stopDelay = 0.2f;
    private float mayJump = 0f;       // Tiempo que aún puedes saltar después de salir del suelo
    public float coyoteTime = 0.1f;  // Duración de coyote time
    private float jumpBufferTimer = 0f;       // Temporizador del buffer de salto
    public float jumpBufferTime = 0.15f;      // Cuánto tiempo recordamos el input

    [Header("Salto variable")]
    [Range(0f, 1f)]
    public float jumpCutMultiplier = 0.5f;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashTime = 0.2f;
    public float dashCooldown = 0.5f; // segundos entre dashes
    private bool canDash = true;    // si puedes dashar
    private float facingDirection = 1f; // 1 = derecha, -1 = izquierda
    private SpriteRenderer _spriteRenderer;
    public float dashDirection;
    private bool isDashing = false;

    [Header("Excavar")]
    public float undergroundOffset = -0.8f;

    public float autoDigSpeed = 9f;
    private readonly string digTag = "dig";
    private Collider[] myColliders;
    private Transform digZone;
    private float digDirection;


    [Header("Animaciones")]
    public Animator _animator;
    public GameObject dashCloudPrefab;

    private readonly string undergroundLayer = "underground";
    private readonly string normalLayer = "Default";



    private bool touchedWater = false; //si toca el agua

    //TAGS
    private string groundTag = "Ground";
    private readonly string waterTag = "Water";

    void Start()
    {
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        myColliders = GetComponentsInChildren<Collider>();
        jumpsRemaining = maxJumps;


    }

    void Update()
    {
        transform.rotation = Quaternion.identity;
        direction = playerInput.actions["Movement"].ReadValue<float>();

        if (playerInput.actions["reset"].triggered || touchedWater) //se vuelve al inicio
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
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
        if (jumpBufferTimer > 0f)
        {
            if (mayJump > 0f || jumpsRemaining > 0)
            {
                TryJump();
                jumpBufferTimer = 0f;
            }
        }
        if (playerInput.actions["jump"].WasReleasedThisFrame() && _rigidBody.linearVelocity.y > 0f && jumpnum == 1)
        {
            _rigidBody.linearVelocity = new Vector2(
                _rigidBody.linearVelocity.x,
                _rigidBody.linearVelocity.y * jumpCutMultiplier
            );
        }
        if (playerInput.actions["Dash"].triggered)
        {
            StartCoroutine(Dash());
        }

        if (playerInput.actions["Dig"].triggered)
        {
            digDirection = playerInput.actions["UpDown"].ReadValue<float>();

            if (Mathf.Abs(digDirection) > 0.1f)
            {
                Dig();
            }
        }
    }
    //metodo para saber si toca el agua
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag(waterTag))
        {
            touchedWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(waterTag))
            touchedWater = false;
    }

    private void FixedUpdate()
    {
        float targetX;

        if (!isDashing)
        {
            if (Mathf.Abs(moveInput) > 0.01f)
            {
                facingDirection = Mathf.Sign(moveInput);
                targetX = moveInput * moveSpeed;
            }
            else
            {
                targetX = Mathf.Lerp(_rigidBody.linearVelocity.x, 0f, Time.fixedDeltaTime / stopDelay);
                if (Math.Abs(targetX) < 0.03f) targetX = 0;
            }
            if (!isDashing)
            {
                _rigidBody.linearVelocity = new Vector2(targetX, _rigidBody.linearVelocity.y);
                _animator.SetBool("isWalking", Math.Abs(targetX) > 0.03f);
            }
            else
            {
                _animator.SetBool("isWalking", false);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Vector3 normal = contact.normal;

            if (Vector3.Angle(normal, Vector3.up) < 30f)
            {
                {
                    _animator.SetBool("isJumping", false);
                    inGround = true;
                    jumpsRemaining = maxJumps;
                    jumpnum = 0;
                }
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag(groundTag))
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
            if (moveInput > 0) _spriteRenderer.flipX = true;
            else _spriteRenderer.flipX = false;
        }
    }

    //SALTO
    private void TryJump()
    {

        if (jumpsRemaining <= 0)
            return;
        if (inGround || mayJump > 0f || maxJumps == 2)
        {

            DoJump();
            jumpsRemaining--;
            jumpnum++;
            mayJump = 0f;
            inGround = false;

        }

    }

    private void DoJump()
    {
        _animator.SetBool("isJumping", true);
        _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, 0); // reinicia la velocidad vertical
        _rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

    }


    ///DASH
    private IEnumerator Dash()
    {
        if (!canDash)
            yield break; // no hacer nada si está en cooldown

        isDashing = true;
        canDash = false; // bloqueamos el dash
        _rigidBody.useGravity = false;

        // Guardamos la velocidad vertical actual (por si quieres restaurarla después)
        float originalY = _rigidBody.linearVelocity.y;
        float elapsed = 0f;
        _animator.SetBool("isDashing", true);
        while (elapsed < dashTime)
        {
            // Dash horizontal completamente plano
            _rigidBody.linearVelocity = new Vector3(facingDirection * dashSpeed, 0f, 0f);

            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Restauramos gravedad
        _rigidBody.useGravity = true;
        isDashing = false;

        _animator.SetBool("isDashing", false);
        // Espera cooldown antes de permitir nuevo dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }



    //EXCAVAR
    public void Dig()
    {
        if (!isDigging)
        {
            if (digDirection > 0 && IsDiggableAboveBox())
                StartDig();
            else if (digDirection < 0 && IsDiggableBelowBox())
            {
                StartDig();
            }
            else
                return;

            if (digZone != null && Mathf.Abs(digDirection) > 0.1f)
                StartCoroutine(AutoMoveVerticalDig());
        }
    }

    private void StartDig()
    {
        isDigging = true;
        gameObject.layer = LayerMask.NameToLayer(undergroundLayer);
        _rigidBody.useGravity = false;

        IgnoreTagCollisions(true);
        transform.position += new Vector3(0, undergroundOffset, 0);

        _animator.SetBool("isDigging", true);
    }

    private void StopDig()
    {
        isDigging = false;
        gameObject.layer = LayerMask.NameToLayer(normalLayer);
        _rigidBody.useGravity = true;
        IgnoreTagCollisions(false);
        foreach (var c in myColliders)
            c.enabled = true;
        _animator.SetBool("isDigging", false);

    }

    // Ignore only underground tag colls
    private void IgnoreTagCollisions(bool ignore)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag(digTag);

        foreach (GameObject obj in objs)
        {
            Collider[] targetCols = obj.GetComponentsInChildren<Collider>();

            foreach (Collider targetCol in targetCols)
            {
                foreach (Collider myCol in myColliders)
                {
                    Physics.IgnoreCollision(myCol, targetCol, ignore);
                }
            }
        }
    }

    bool IsDiggableAboveBox()
    {
        float castDistance = 1f;
        Vector3 halfExtents = new(0.5f, 0.1f, 0.5f);
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.BoxCast(origin, halfExtents, Vector3.up, out RaycastHit hit, Quaternion.identity, castDistance))
        {
            if (hit.collider.CompareTag(digTag))
            {
                digZone = hit.transform;
                return true;
            }
        }
        return false;
    }

    bool IsDiggableBelowBox()
    {
        float castDistance = 1f;
        Vector3 halfExtents = new(0.5f, 0.1f, 0.5f);
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        if (Physics.BoxCast(origin, halfExtents, Vector3.down, out RaycastHit hit, Quaternion.identity, castDistance))
        {
            if (hit.collider.CompareTag(digTag))
            {
                digZone = hit.transform;
                return true;
            }
        }
        return false;
    }

    private IEnumerator AutoMoveVerticalDig()
    {
        if (digZone == null) yield break;
        Collider digCollider = digZone.GetComponent<Collider>();
        float targetY = transform.position.y > digCollider.bounds.center.y
                        ? digCollider.bounds.min.y
                        : digCollider.bounds.max.y;

        _rigidBody.useGravity = false;

        Debug.Log(targetY);
        while (isDigging && Mathf.Abs(transform.position.y - targetY) > 0.01f)
        {

            float dir = targetY > transform.position.y ? 1f : -1f;
            Vector3 nextPos = transform.position + Vector3.up * dir * autoDigSpeed * Time.fixedDeltaTime;

            if ((dir > 0 && nextPos.y > targetY) || (dir < 0 && nextPos.y < targetY))
                nextPos.y = targetY;

            _rigidBody.MovePosition(nextPos);

            yield return new WaitForFixedUpdate();
        }

        StopDig();
    }

}
