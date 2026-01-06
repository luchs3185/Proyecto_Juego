using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Configuración")]
    public Transform player;
    public float detectionRadius = 10.0f;
    public float speed = 4.0f;
    public int health = 1;

    [Header("Combate")]
    public int damage = 1;
    public float attackRange = 5f;
    public float attackCooldown = 1.2f;
    public Collider attackZone; // Asegúrate de que este collider cubra ambos lados o ten dos colliders
    
    // Referencias
    private Rigidbody rb;
    private Animator animator;
    private SpriteRenderer _spriteRenderer;

    [Header("Animación de Giro")]
    // AJUSTA ESTO: ¿Cuántos segundos dura tu animación de giro exactos?
    public float turnDuration = 0.55f;

    // Estado interno
    private bool dead = false;
    private bool attacking = false;
    private bool isTurning = false;
    private float lastAttackTime = 0f;
    private int currentDirection = 1; // 1 Derecha, -1 Izquierda

    // NUEVO: objeto que se activará al morir
    public GameObject objectToActivateOnDeath;

   void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (dead || player == null) return;

        // Si está girando o ya está en medio de una animación de ataque, no hace nada nuevo
        if (isTurning || attacking) 
        {
            rb.linearVelocity = Vector3.zero; 
            return;
        }

        CheckForFlip();
        if (isTurning) return;

        float distanceX = Mathf.Abs(transform.position.x - player.position.x);
        float distanceY = Mathf.Abs(transform.position.y - player.position.y);
        float totalDistance = Vector3.Distance(transform.position, player.position);

        // CONDICIÓN DE ATAQUE: Solo si está en rango Y ha pasado el cooldown
        if (distanceX <= attackRange && distanceY < 3.0f)
        {
            if (Time.time - lastAttackTime >= attackCooldown)
            {
                StartAttack();
            }
            else
            {
                // Si está en rango pero el cooldown no ha pasado, se queda quieto (Idle)
                animator.SetBool("isWalking", false);
            }
        }
        // MOVIMIENTO: Solo si NO está en rango de ataque pero sí en rango de detección
        else if (totalDistance <= detectionRadius)
        {
            MoveToPlayer();
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void StartAttack()
    {
        attacking = true;
        lastAttackTime = Time.time;
        animator.SetBool("isWalking", false); // Asegurar que deja de caminar
        animator.SetTrigger("attack");
        
        // El tiempo de EndAttack debe ser igual o un poco menor a tu clip de animación
        Invoke("EndAttack", 0.8f); 
    }

    public void EndAttack()
    {
        attacking = false;
    }

    // ESTA ES LA FUNCIÓN QUE DEBE ESTAR EN EL ANIMATION EVENT
    public void AttackHit()
    {
        if (attackZone == null) return;

        // Usamos Bounds para crear la caja de detección basada en el collider verde
        Vector3 center = attackZone.bounds.center;
        Vector3 halfExtents = attackZone.bounds.extents;

        // Detectamos todo en el área
        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity);

        foreach (var hit in hits)
        {
            // IMPORTANTE: Verifica que tu jugador tenga el TAG "Player"
            if (hit.CompareTag("Player"))
            {
                // Buscamos el script Player en el objeto o en sus padres
                var p = hit.GetComponentInParent<Player>();
                if (p != null)
                {
                    p.TakeDamage(damage, transform.position);
                    Debug.Log("¡Golpeado!");
                }
            }
        }
    }

    private void CheckForFlip()
    {
        float deltaX = player.position.x - transform.position.x;
        if (Mathf.Abs(deltaX) > 0.3f)
        {
            int desiredDirection = deltaX > 0 ? 1 : -1;
            if (desiredDirection != currentDirection)
            {
                StartCoroutine(PerformTurn(desiredDirection));
            }
        }
    }

    IEnumerator PerformTurn(int newDir)
    {
        isTurning = true;
        animator.SetBool("isWalking", false);
        animator.SetInteger("Direction", newDir);
        yield return new WaitForSeconds(turnDuration);
        currentDirection = newDir;
        isTurning = false;
    }

    private void MoveToPlayer()
    {
        animator.SetBool("isWalking", true);
        Vector3 targetPos = new Vector3(player.position.x, transform.position.y, transform.position.z);
        rb.MovePosition(Vector3.MoveTowards(rb.position, targetPos, speed * Time.fixedDeltaTime));
    }

    public void TakeDamage(int dmg) 
    {
        health -= dmg;
        StartCoroutine(HitFlash());
        if (health <= 0) StartCoroutine(DieByDamage());
    }

    IEnumerator HitFlash()
    {
        if (_spriteRenderer != null)
        {
            Color originalColor = _spriteRenderer.color;
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            _spriteRenderer.color = originalColor;
        }
    }

    IEnumerator DieByDamage()
    {
        dead = true;
        animator.SetBool("isWalking", false);
        if (GetComponent<Collider>()) GetComponent<Collider>().enabled = false;
        
        // Efecto de aplastamiento
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(originalScale.x, originalScale.y * 0.2f, originalScale.z);
        
        float duration = 0.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }

        if (objectToActivateOnDeath != null)
        {
            objectToActivateOnDeath.transform.position = transform.position;
            objectToActivateOnDeath.SetActive(true);
        }

        Destroy(gameObject);
    }
}