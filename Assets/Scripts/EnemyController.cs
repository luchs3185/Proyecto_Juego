using System.Collections;
using UnityEngine;
using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 10.0f;
    public float speed = 3.0f;
    public int health = 2;

    private Rigidbody rb;
    private Vector3 movement;
    private Collider col;
    private bool dead = false;

    public int damage = 1;
    public float attackCooldown = 1f;
    private float lastAttackTime = 0f;

    public GameObject walkableSpace;
    private Collider walkableCollider;

    private SpriteRenderer _spriteRenderer;
    private int patrolDirection = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (walkableSpace != null)
        {
            walkableCollider = walkableSpace.GetComponent<Collider>();
        }
    }

    void FixedUpdate()
    {
        if (dead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            movement = new Vector3(direction.x, 0, direction.z);
        }
        else
        {
            movement = new Vector3(patrolDirection, 0, 0);
        }
        if (movement.x > 0.1f)
        {
            _spriteRenderer.flipX = true; 
        }
        else if (movement.x < -0.1f)
        {
            _spriteRenderer.flipX = false;
        }

        Vector3 nextPosition = rb.position + movement * speed * Time.fixedDeltaTime;

        if (walkableCollider != null)
        {
            if (walkableCollider.bounds.Contains(nextPosition))
            {
                rb.MovePosition(nextPosition);
            }
            else if (distanceToPlayer > detectionRadius)
            {
                patrolDirection *= -1;
            }
        }
    }

    public void TakeDamage(int amount, Vector3 damageSource)
    {
        if (dead) return;

        health -= amount;

        Vector3 dir = (transform.position - damageSource).normalized;
        rb.isKinematic = false;
        rb.AddForce(new Vector3(dir.x * 4f, 2f, 0), ForceMode.Impulse);
        StartCoroutine(RecoverKinematic());

        if (health <= 0)
            StartCoroutine(DieByDamage());
        else
            StartCoroutine(HitFlash());
    }


    void OnCollisionStay(Collision collision)
    {
        if (dead) return;

        Player p = collision.gameObject.GetComponentInParent<Player>();
        if (p != null && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            p.TakeDamage(1, transform.position);
        }
    }

   IEnumerator HitFlash()
    {
        Color original = _spriteRenderer.color;
        _spriteRenderer.color = Color.white; // mejor que rojo
        yield return new WaitForSeconds(0.06f);
        _spriteRenderer.color = original;
    }

    IEnumerator DieByDamage()
    {
        dead = true;
        if (col != null) col.enabled = false;
        speed = 0f;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = new Vector3(originalScale.x, originalScale.y * 0.2f, originalScale.z);
        float duration = 0.12f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / duration);
            yield return null;
        }

        yield return new WaitForSeconds(0.12f);
        Destroy(gameObject);
    }

    IEnumerator RecoverKinematic()
    {
        yield return new WaitForSeconds(0.12f);
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
}