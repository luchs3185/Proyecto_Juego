using UnityEngine;
using System.Collections;

public class WaterfallBlock : MonoBehaviour
{
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float speed;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public float respawnDelay = 5;
    private bool isFalling = true;

    public void Init(Vector3 start, float verticalOffset, float moveSpeed)
    {
        startPosition = start;
        targetPosition = start + new Vector3(0, verticalOffset, 0);
        speed = moveSpeed;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        transform.position = startPosition;

        if(animator!=null)
            animator.Play("Waterfall_Fall",0,0f);
    }

    void Update()
    {
        if (isFalling)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                StartCoroutine(ResetPosition());
            }
        }
    }

    IEnumerator ResetPosition()
    {
        isFalling = false;
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(respawnDelay);

        transform.position = startPosition;
        spriteRenderer.enabled = true;

        if(animator!=null)
            animator.Play("Waterfall_Fall",0,0f);

        isFalling = true;
    }
}
