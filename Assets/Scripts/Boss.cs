using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 10.0f;
    public float speed = 3.0f;
    public int health = 1;

    private Rigidbody rb;
    private Vector3 movement;
    private Collider col;
    private bool dead = false;
    private enum State { Idle, Move, Attack };
    private State state;

    public float attackCooldown = 1f;
    private bool inRange = false;
    // Ver si esta rotatndo
    private bool flipping = false;

    [Header("Ataque")]
    public int damage = 1;
    public float attackRange = 5f;
    private float lastAttackTime = 0f;
    private float lastFlipTime = 0f;
    private bool attacking = false;
    private float flipDelay;
    private float dir = 1;
    public Collider attackZone;
    private Animator animator;
    private SpriteRenderer _spriteRenderer;

    // NUEVO: objeto que se activará al morir
    public GameObject objectToActivateOnDeath;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        dir = rb.transform.localScale.x > 0 ? -1 : 1;
    }


    void FixedUpdate()
    {
        if (dead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRadius)
        {
            inRange = true;
        }
        else
        {
            inRange = false;
            return;
        }

        if (flipping)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("change_dir") && stateInfo.normalizedTime >= 1f)
            {
                lastFlipTime = Time.time;
                animator.SetBool("rotate", false);
                flipping = false;
            }
            return;
        }
        else if (attacking)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("attack") && stateInfo.normalizedTime >= 1f)
            {
                attacking = false;
                animator.SetBool("attack", false);
                state = State.Idle;
            }
            else
            {
                state = State.Attack;
            }
        }

        else if (distanceToPlayer > attackRange + 0.2)
        {
            state = State.Move;
        }
        else
        {
            if (state != State.Attack)
            {
                state = (Random.Range(0, 4) == 0) ? State.Attack : State.Idle;
            }
        }
        HandleFlip();
        switch (state)
        {
            case State.Idle:
                {
                    break;
                }
            case State.Attack:
                {
                    if (!attacking)
                        StartAttack();
                    break;
                }
            case State.Move:
                {
                    MoveToPlayer();
                    break;
                }
        }
        UpdateDirection();
    }

    private void MoveToPlayer()
    {
        float distance = Mathf.Abs(player.position.x - transform.position.x);

        if (distance > attackRange)
        {
            animator.SetBool("isWalking", true);
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(player.position.x, transform.position.y, transform.position.z),
                speed * Time.deltaTime
            );
        }
        else
        {
            animator.SetBool("isWalking", false);
            state = State.Idle;
        }
    }


    void HandleFlip()
    {
        if (Time.time - lastFlipTime >= flipDelay)
        {

            float direction = player.position.x - transform.position.x;
            flipDelay = Random.Range(0.6f, 2.5f);
            if ((direction > 0 && dir < 0) || (direction < 0 && dir > 0))
            {
                flipping = true;
                animator.SetTrigger("rotate");
            }
        }
    }
    public void Flip()
    {
        dir = dir > 0 ? -1 : 1;
        lastFlipTime = Time.time;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
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

        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player != null && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            player.TakeDamage(damage, transform.position);
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

    private void StartAttack()
    {
        animator.SetTrigger("attack");
        state = State.Attack;
        lastAttackTime = Time.time;
        attacking = true;
    }
    public void Attack()
    {
        BoxCollider col = attackZone.GetComponent<BoxCollider>();
        if (col == null) return;

        Vector3 center = col.bounds.center;
        Vector3 halfExtents = col.bounds.extents;
        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask("Default"));


        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player")) 
            {
                Player playerHealth = hit.GetComponent<Player>();
                if (playerHealth != null)
                {
                    Debug.Log("Player damaged");
                    playerHealth.TakeDamage(damage, transform.position);
                }
            }
        }
    }
    public void EndAttack()
    {
        attacking = false;
        animator.SetBool("attack", false);
        Debug.Log("end");
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

        // NUEVO: activamos el objeto oculto
        if (objectToActivateOnDeath != null)
        {
            objectToActivateOnDeath.transform.position = transform.position;
            objectToActivateOnDeath.transform.rotation = transform.rotation;
            objectToActivateOnDeath.SetActive(true);
        }

        Destroy(gameObject);
    }
    void UpdateDirection()
    {
        float dx = player.position.x - transform.position.x;

        if (dx > 0)
            animator.SetInteger("direction", 1);
        else
            animator.SetInteger("direction", -1);
    }

}
