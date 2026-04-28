using UnityEngine;

public class enemigo_animacionyseguimiento_mike : MonoBehaviour
{
    private Transform player;
    private float speed = 3f;
    private float followRange = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer sprite;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        HandleAnimation();
        FlipSprite();
    }

    void FixedUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= followRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    void HandleAnimation()
    {
        float currentSpeed = rb.linearVelocity.magnitude;

        animator.SetFloat("Speed", currentSpeed);
        animator.SetBool("isMoving", currentSpeed > 0.1f);
    }

    void FlipSprite()
    {
        // Si el jugador está a la derecha → mirar derecha
        if (player.position.x > transform.position.x)
        {
            sprite.flipX = true;

        }
        else if(player.position.x < transform.position.x)
        {
            sprite.flipX = false;
        }
    }
}
