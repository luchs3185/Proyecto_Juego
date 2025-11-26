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
    public float coyoteTime = 0.5f;  // Duración de coyote time
    private float jumpBufferTimer = 0f;       // Temporizador del buffer de salto
    public float jumpBufferTime = 0.2f;      // Cuánto tiempo recordamos el input

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
    public bool dashobj = false;
    [Header("Excavar")]
    public float undergroundOffset = -0.8f;

    public float autoDigSpeed = 9f;
    private readonly string digTag = "dig";
    private Collider[] myColliders;
    private Transform digZone;
    private Vector2 digDirection;

    [Header("Animaciones")]
    public Animator _animator;
    public GameObject dashCloudPrefab;

    private readonly string undergroundLayer = "underground";
    private readonly string normalLayer = "Default";

    [Header("Vida")]
    public int life = 5;
    public int maxLife = 5;
    private bool touchWater = false; //si toca el agua
    public bool iframe = false;

    [Header("Transición de nivel")]
    public CanvasGroup fadeCanvas;   // panel negro para el fundido
    public float fadeTime = 0.3f;    // puración del fundido en segundos

    [Header("Melee")]
    public float meleeRange = 5f;
    public float meleeRadius = 5f;
    public float meleeYOffset = 0.2f;
    public int meleeDamage = 1;
    public float meleeCooldown = 0.4f;
    private float lastMeleeTime = -10f;
    public LayerMask enemyLayer;
    public bool isMeele = false;

    //TAGS
    private string groundTag = "Ground";
    private readonly string waterTag = "Water";

    [Header("UI")]
    public LifeBarController lifeBar;



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

        if (playerInput.actions["reset"].triggered) //se vuelve al inicio
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
        if (playerInput.actions["Dash"].triggered && dashobj)
        {
            StartCoroutine(Dash());
        }

        if (playerInput.actions["Dig"].triggered)
        {
            Dig();
        }

        // Melee attack
        if (playerInput.actions["Attack"].triggered && Time.time - lastMeleeTime >= meleeCooldown)
        {
            lastMeleeTime = Time.time;

            // Actualizamos la dirección antes de atacar
            facingDirection = _spriteRenderer.flipX ? 1f : -1f;

            // Activamos animación
            _animator.SetTrigger("Melee");

            // Aplicamos daño
            PerformMelee();
        }
        }
    //metodo para saber si toca el agua
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(waterTag))
        {
            touchWater = true;
            TakeDamageWater();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(waterTag))
            touchWater = false;
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
        if (IsGroundContact(collision))
        {
            _animator.SetBool("isJumping", false);
            inGround = true;
            jumpsRemaining = maxJumps;
            jumpnum = 0;
        }
    }
    bool IsGroundContact(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (Vector3.Angle(contact.normal, Vector3.up) < 30f)
            {
                return true;
            }
        }
        return false;
    }
    void OnCollisionStay(Collision collision)
    {
    // Mientras exista al menos un contacto válido con suelo = seguimos en el suelo
        if (IsGroundContact(collision))
        {
        inGround = true;
        }
    }
    void OnCollisionExit(Collision collision)
    {
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
        if (jumpsRemaining <= 0){
            return;
        }

        if (maxJumps == 1)
        {
            if(!inGround){
                return;
            }  
            DoJump();
            jumpsRemaining = 0;
            jumpnum = 1;
            Debug.Log("Salto simpple 1");
            return;
        }

        else if (maxJumps == 2)
        {
            // Primer salto (suelo o coyote time)
            if ((inGround || mayJump > 0f )&& jumpsRemaining == 2)
            {
                DoJump();
                jumpsRemaining = 1;   
                jumpnum = 1;
                mayJump = 0f;
                inGround = false;
                Debug.Log("Salto doble 1");
                return;
            }

            // Segundo salto EN AIRE
            if (jumpsRemaining == 1 && !inGround)
            {
                DoJump();
                jumpsRemaining = 0;
                jumpnum = 2;
                Debug.Log("Salto doble");
                return;
            }
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
            if (IsDiggableAboveBox() || IsDiggableBelowBox() || IsDiggableLeft() || IsDiggableRight())
                StartDig();
            else
                return;

            if (digZone != null)
            {
                if (digDirection.x == 1 || digDirection.x == -1)
                    StartCoroutine(AutoMoveHorizontalDig());
                else
                    StartCoroutine(AutoMoveVerticalDig());
            }
        }
    }

    private void StartDig()
    {
        isDigging = true;
        gameObject.layer = LayerMask.NameToLayer(undergroundLayer);
        _rigidBody.useGravity = false;

        IgnoreTagCollisions(true);

        transform.position += new Vector3(digDirection.x * undergroundOffset, digDirection.y * undergroundOffset, 0);

        _animator.SetBool("isDigging", true);
        
        playerInput.actions["Movement"].Disable();
    }

    private void StopDig()
    {
        isDigging = false;
        gameObject.layer = LayerMask.NameToLayer(normalLayer);
        foreach (var c in myColliders)
            c.enabled = true;
        IgnoreTagCollisions(false);
        _rigidBody.useGravity = true;
        _animator.SetBool("isDigging", false);
        playerInput.actions["Movement"].Enable();

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
                digDirection = new(0, 1);
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

                digDirection = new(0, -1);
                digZone = hit.transform;
                return true;
            }
        }
        return false;
    }


    bool IsDiggableRight()
    {
        float castDistance = 1f;
        Vector3 halfExtents = new Vector3(0.1f, 0.5f, 0.5f);
        Vector3 origin = transform.position + Vector3.right * 0.1f;

        if (Physics.BoxCast(origin, halfExtents, Vector3.right, out RaycastHit hit, Quaternion.identity, castDistance))
        {
            if (hit.collider.CompareTag(digTag))
            {
                digDirection = new(1, 0);
                digZone = hit.transform;
                return true;
            }
        }
        return false;
    }
    bool IsDiggableLeft()
    {
        float castDistance = 1f;
        Vector3 halfExtents = new Vector3(0.1f, 0.5f, 0.5f);
        Vector3 origin = transform.position + Vector3.left * 0.1f;

        if (Physics.BoxCast(origin, halfExtents, Vector3.left, out RaycastHit hit, Quaternion.identity, castDistance))
        {
            if (hit.collider.CompareTag(digTag))
            {
                digDirection = new(-1, 0);
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



    private IEnumerator AutoMoveHorizontalDig()
    {
        if (digZone == null) yield break;

        Collider digCollider = digZone.GetComponent<Collider>();
        float targetX = transform.position.x < digCollider.bounds.center.x
            ? digCollider.bounds.max.x
            : digCollider.bounds.min.x;

        _rigidBody.useGravity = false;

        float dir = targetX > transform.position.x ? 1f : -1f;

        while (isDigging && Mathf.Abs(_rigidBody.position.x - targetX) > 0.001f)
        {
            Vector3 nextPos = transform.position + autoDigSpeed * dir * Time.fixedDeltaTime * Vector3.right;

            if (dir > 0 && nextPos.x > targetX)
                nextPos.x = targetX;

            if    (dir < 0 && nextPos.x < targetX){
                nextPos.x = targetX;
                Debug.Log(nextPos.x + ", "+ targetX);
            }
            _rigidBody.MovePosition(nextPos);

            yield return new WaitForFixedUpdate();
        }

        StopDig();
    }



    private void TakeDamageWater()
    {      
        if(iframe){
            return;
        }
        iframe = true;
        life = life - 1;
        if (lifeBar != null) lifeBar.UpdateLife(life);
        
        if (life > 0)
        {
            RespawnAtClosest();
            StartCoroutine(InvulnerabilityCooldown()); 
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }


    public void TakeDamage(int amount)
    {
        if (iframe) return;
        iframe = true;

        life -= amount;

        if (lifeBar != null) lifeBar.UpdateLife(life);

        Vector3 knockback = new Vector3(
            Mathf.Sign(transform.position.x - transform.position.x) * 5f, // horizontal
            6f, // vertical
            0f // no necesitamos Z
        );
         _rigidBody.linearVelocity = knockback;
        
        if (life <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            StartCoroutine(InvulnerabilityCooldown());
            StartCoroutine(HitFlash());
        }
    }

    private IEnumerator HitFlash()
    {
        if (_spriteRenderer == null) yield break;

        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        _spriteRenderer.color = originalColor;
    }

    private IEnumerator InvulnerabilityCooldown()
    {
        yield return new WaitForSeconds(0.3f); // evita multi-daño al caer en el agua
        iframe = false;
    }

    private void RespawnAtClosest()
    {
        Transform respawn = GetClosestRespawnPoint();

        if (respawn != null)
        {
            _rigidBody.linearVelocity = Vector3.zero;
            transform.position = respawn.position;
        }

        StartCoroutine(RespawnDelay());
    }

    private Transform GetClosestRespawnPoint()
    {
        GameObject[] respawns = GameObject.FindGameObjectsWithTag("RespawnPoint");

        Transform closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject r in respawns)
        {
            float dist = Vector3.Distance(currentPos, r.transform.position);

            if (dist < minDistance)
            {
                minDistance = dist;
                closest = r.transform;
            }
        }
        return closest;
    }

    // teletransporte con fundido
    public IEnumerator TeleportWithFade(Vector3 targetPos)
    {
        // si no hay panel de fundido asignado, solo teletransporta sin efecto
        if (fadeCanvas == null)
        {
            _rigidBody.linearVelocity = Vector3.zero;
            transform.position = targetPos;
            yield break;
        }

        // desactivar el movimiento del jugador
        if (playerInput != null)
        {
            playerInput.actions["Movement"].Disable();
        }

        // parar el movimiento
        _rigidBody.linearVelocity = Vector3.zero;

        // fundido a negro
        yield return StartCoroutine(Fade(1f));

        // teletransportar
        transform.position = targetPos;

        // pequeña pausa opcional
        yield return new WaitForSeconds(0.05f);

        // fundido desde negro (a transparente)
        yield return StartCoroutine(Fade(0f));

        // volver a activar el movimiento
        if (playerInput != null)
        {
            playerInput.actions["Movement"].Enable();
        }
    }

    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvas == null)
        {
            yield break;
        }

        float startAlpha = fadeCanvas.alpha;
        float timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            fadeCanvas.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        fadeCanvas.alpha = targetAlpha;
    }

    public void PerformMelee()
    {
        StartCoroutine(MeleeCoroutine());
    }

    private IEnumerator MeleeCoroutine()
    {
        // Activamos la animación de melee
        _animator.SetBool("isMeele", true);
        // Calculamos el origen y aplicamos daño
        Vector3 origin = transform.position + new Vector3(facingDirection * meleeRange, meleeYOffset, 0f);
        Collider[] hits = Physics.OverlapSphere(origin, meleeRadius, enemyLayer);
        foreach (var hit in hits)
        {
            EnemyController enemy = hit.GetComponentInParent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(meleeDamage);
            }
        }

        // Esperamos la duración de la animación antes de resetear el bool
        yield return new WaitForSeconds(meleeCooldown); // o la duración real de la animación

        // Desactivamos la animación
        _animator.SetBool("isMeele", false);
    }

    private IEnumerator RespawnDelay()
    {
        playerInput.actions["Movement"].Disable();
        yield return new WaitForSeconds(0.2f);
        playerInput.actions["Movement"].Enable();
    }
}
