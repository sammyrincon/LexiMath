using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;

    [Header("Patrol")]
    public float patrolSpeed = 1.5f;
    public float patrolRange = 3f;

    [Header("Attack")]
    public float attackRange = 1f;
    public int attackDamage = 1;
    public float attackCooldown = 1.5f;
    public LayerMask playerLayer;

    [Header("Daño al jugador")]
    [Tooltip("Cantidad de HP que le quita al jugador por ataque")]
    public float danoAlJugador = 20f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private int currentHealth;
    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isDead = false;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform player;

    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = player != null ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
        }
        else if (!isAttacking)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * patrolSpeed * Time.deltaTime);

        if (animator != null)
            animator.SetFloat("Speed", patrolSpeed);

        if (transform.position.x >= startPosition.x + patrolRange)
        {
            movingRight = false;
            spriteRenderer.flipX = true;
        }
        else if (transform.position.x <= startPosition.x - patrolRange)
        {
            movingRight = true;
            spriteRenderer.flipX = false;
        }
    }

    void Attack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
            animator.SetFloat("Speed", 0);

        if (animator != null)
            animator.SetTrigger("Attack");

        // ══════════════════════════════════════════════════════
        //  NUEVO — Hacer daño al jugador
        // ══════════════════════════════════════════════════════
        if (PlayerHealth.Instance != null)
            PlayerHealth.Instance.RecibirDano(danoAlJugador);

        Invoke(nameof(FinishAttack), 0.5f);
    }

    void FinishAttack()
    {
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
            Die();
    }

    System.Collections.IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
            col.enabled = false;

        Destroy(gameObject, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 pos = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(pos + Vector3.right * patrolRange, 0.2f);
        Gizmos.DrawWireSphere(pos + Vector3.left * patrolRange, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
