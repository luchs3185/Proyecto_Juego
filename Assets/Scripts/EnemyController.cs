using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 10.0f;
    public float speed = 3.0f;
    public int health = 1;

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

    private Animator _animator;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        rb.isKinematic = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        _animator = GetComponent<Animator>();

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

    public void TakeDamage(int amount)
    {
        if (dead) return;
        health -= amount;
        if (health <= 0)
        {
            StartCoroutine(DieByDamage());
        }
        else
        {
            StartCoroutine(HitFlash());
        }
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
        _animator.SetBool("Dead", true);
        if (col != null)
            col.enabled = false;

        speed = 0f;
        rb.isKinematic = true;

        if (_animator != null)
        {
            _animator.SetTrigger("hit");
        }

        yield return new WaitForSeconds(0.5f); // Esperar a que se vea la animación
        Destroy(gameObject);
    }
}