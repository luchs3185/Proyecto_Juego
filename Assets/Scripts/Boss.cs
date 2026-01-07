using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Configuración")]
    public Transform player;
    public float detectionRadius = 10.0f;
    public float speed = 4.0f;
    public int health = 1;
    public AudioClip angryCat;
    public AudioClip deathCat;

    [Header("Combate")]
    public int damage = 1;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public Collider attackZone; // Asegúrate de que este collider cubra ambos lados o ten dos colliders

    // Referencias
    private Rigidbody rb;
    private Animator animator;
    private SpriteRenderer _spriteRenderer;
    private AudioSource bossMusic;

    [Header("Animación de Giro")]
    // AJUSTA ESTO: ¿Cuántos segundos dura tu animación de giro exactos?
    public float turnDuration = 0.55f;

    // Estado interno
    private bool dead = false;
    private bool attacking = false;
    private bool isTurning = false;
    private float nextAttackTime = 0f;
    private int currentDirection = 1; // 1 Derecha, -1 Izquierda

    // NUEVO: objeto que se activará al morir
    public GameObject objectToActivateOnDeath;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        bossMusic = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (dead || player == null) return;

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

        // LÓGICA DE ATAQUE CORREGIDA
        if (distanceX <= attackRange && distanceY < 3.0f)
        {
            animator.SetBool("isWalking", false);
            rb.linearVelocity = Vector3.zero;
            if (Time.time >= nextAttackTime)
            {
                bossMusic.PlayOneShot(angryCat);
                StartAttack();
            }
        }
        else if (totalDistance <= detectionRadius)
        {
            if (!bossMusic.isPlaying)
            {
                bossMusic.Play();
                MusicManager.Instance.StopMusic();
            }
            MoveToPlayer();
        }
        else
        {
            if (bossMusic.isPlaying)
            {
                bossMusic.Stop();
                MusicManager.Instance.PlayMusic();

            }
            animator.SetBool("isWalking", false);
        }
    }

    private void StartAttack()
    {
        if (attacking) return; // Doble seguridad

        attacking = true;
        rb.linearVelocity = Vector3.zero;
        animator.SetBool("isWalking", false);

        // Limpiamos el trigger antes de activarlo para que no se acumule
        animator.ResetTrigger("attack");
        animator.SetTrigger("attack");


        StartCoroutine(SequenceAttack());
    }

    IEnumerator SequenceAttack()
    {
        // 1. Tiempo de espera: Debe ser igual a la duración de tu clip de ataque
        // Mira en Unity cuánto dura tu animación (ej. 0.8s) y ponlo aquí.
        yield return new WaitForSeconds(0.8f);

        // 2. IMPORTANTE: Forzamos al Animator a volver a un estado neutro
        animator.ResetTrigger("attack");

        // 3. Pequeña pausa de seguridad para que los logs no se saturen
        yield return new WaitForSeconds(0.2f);

        attacking = false;
    }
    public void AttackHit()
    {
        Debug.Log("Procesando hit");
        // 1. Verificación de seguridad
        if (attackZone == null)
        {
            Debug.LogError("¡No has asignado el objeto AttackZone en el Inspector!");
            return;
        }

        // 2. Obtenemos los datos exactos del Collider del objeto Attack Zone
        // Usamos el centro real en el mundo y sus dimensiones (extents)
        Vector3 center = attackZone.bounds.center;
        Vector3 halfExtents = attackZone.bounds.extents;
        Quaternion rotation = attackZone.transform.rotation;

        // 3. Realizamos la detección física en esa caja exacta
        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);

        bool hitDetected = false;
        foreach (var hit in hits)
        {
            // Ignoramos al propio Boss si tiene el tag de Player por error (poco común)
            if (hit.transform == transform) continue;

            if (hit.CompareTag("Player"))
            {
                // Buscamos el script Player en el objeto o sus padres
                Player p = hit.GetComponent<Player>() ?? hit.GetComponentInParent<Player>();

                if (p != null)
                {
                    p.TakeDamage(damage, transform.position);
                    Debug.Log("<color=green>¡Impacto detectado mediante Attack Zone!</color>");
                    hitDetected = true;
                }
            }
        }

        if (!hitDetected)
        {
            Debug.Log("<color=yellow>El ataque no alcanzó a nadie en el Attack Zone.</color>");
        }
    }

    // Dibuja el área en el editor para que puedas ajustarla visualmente
    private void OnDrawGizmosSelected()
    {
        if (attackZone != null)
        {
            Gizmos.color = Color.red;
            // Obtenemos la matriz de transformación del objeto para dibujar el cubo rotado
            Matrix4x4 cubeMatrix = Matrix4x4.TRS(attackZone.bounds.center, attackZone.transform.rotation, Vector3.one);
            Gizmos.matrix = cubeMatrix;

            // Dibujamos el cubo con el tamaño del collider
            Gizmos.DrawWireCube(Vector3.zero, attackZone.bounds.size);
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
        bossMusic.PlayOneShot(deathCat);
        MusicManager.Instance.PlayMusic();
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

        bossMusic.Stop();

        Destroy(gameObject);
    }
}