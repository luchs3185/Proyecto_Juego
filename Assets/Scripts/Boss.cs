using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Configuración")]
    public Transform player;
    public float detectionRadius = 10.0f;
    public float speed = 4.0f;
    public int health = 10;
    public AudioClip angryCat;
    public AudioClip deathCat;

    [Header("Combate")]
    public int damage = 1;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public float attackCooldown_contact = 1f;
    public Collider attackZone; // Asegúrate de que este collider cubra ambos lados o ten dos colliders
    public bool iframe;
    private float lastAttackTime = 0f;
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
    private AudioSource playerAudio;
    // NUEVO: objeto que se activará al morir
    public GameObject objectToActivateOnDeath;
    private Coroutine hitFlashCoroutine;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        bossMusic = GetComponent<AudioSource>();
        playerAudio = player.GetComponent<AudioSource>();
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
                playerAudio.Stop();
                
            }
            MoveToPlayer();
        }
        else
        {
            if (bossMusic.isPlaying)
            {
                bossMusic.Stop();
                playerAudio.Play();

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
      
        yield return new WaitForSeconds(0.8f);
        animator.ResetTrigger("attack");
        yield return new WaitForSeconds(0.2f);

        attacking = false;
    }
    public void AttackHit()
    {
        if (attackZone == null)
        {
            Debug.LogError("¡No has asignado el objeto AttackZone en el Inspector!");
            return;
        }
        Vector3 center = attackZone.bounds.center;
        Vector3 halfExtents = attackZone.bounds.extents;
        Quaternion rotation = attackZone.transform.rotation;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, rotation);

        bool hitDetected = false;
        foreach (var hit in hits)
        {
            if (hit.transform == transform) continue;

            if (hit.CompareTag("Player"))
            {
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
        if (iframe) return;
        iframe = true;
        health -= dmg;
        StartCoroutine(HitFlash());
        if (health <= 0) StartCoroutine(DieByDamage());
        StartCoroutine(InvulnerabilityCooldown());
    }

   IEnumerator HitFlash()
    {
        if (_spriteRenderer == null) yield break;

        // Si hay un flash en curso, lo detenemos y restauramos color
        if (hitFlashCoroutine != null)
        {
            StopCoroutine(hitFlashCoroutine);
            _spriteRenderer.color = Color.white; // restaurar color base
        }

        hitFlashCoroutine = StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine()
    {
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = Color.red; // cambia a rojo

        yield return new WaitForSeconds(0.15f); // duración del flash

        _spriteRenderer.color = originalColor; // restaurar color

        hitFlashCoroutine = null; // liberamos la referencia
    }


    IEnumerator DieByDamage()
    {
        dead = true;

        rb.linearVelocity = Vector3.zero;

        animator.SetBool("isWalking", false);
        animator.SetTrigger("die");
        rb.constraints = RigidbodyConstraints.FreezeAll;

        bossMusic.PlayOneShot(deathCat);
        bossMusic.Stop();
        

        if (objectToActivateOnDeath != null)
        {
            objectToActivateOnDeath.SetActive(true);
        }

        yield break;
    }
    private IEnumerator InvulnerabilityCooldown()
    {
        yield return new WaitForSeconds(0.3f);
        iframe = false;
    }

   public void FreezeAnimation()
    {
        animator.enabled = false;
    }

   private void OnCollisionStay(Collision collision)
    {
        if (dead) return;

        // Filtramos solo al jugador
        if (!collision.collider.CompareTag("Player")) return;
         Debug.Log("<color=red>Toca.</color>");
        Player p = collision.collider.GetComponent<Player>();
        if (p == null) return;

        // Cooldown para no spamear daño
        if (Time.time - lastAttackTime >= attackCooldown_contact)
        {
            lastAttackTime = Time.time;
            p.TakeDamage(damage, transform.position);
            Debug.Log("<color=red>Daño por contacto aplicado al jugador.</color>");
        }
    }

}