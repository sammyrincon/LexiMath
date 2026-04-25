using System.Collections;
using UnityEngine;

public class EnemyBasic : MonoBehaviour, IDamageable
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

    [Header("Tutorial")]
    [SerializeField] private TutorialManager tutorialManager;
    [SerializeField] private float destroyDelay = 0.8f;

    [Header("Animation States")]
    [SerializeField] private string idleStateName = "MushroomIdle";
    [SerializeField] private string walkStateName = "MushroomWalk";
    [SerializeField] private string attackStateName = "MushroomAttack";
    [SerializeField] private string deathStateName = "MushroomDeath";

    private int currentHealth;
    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isDead;
    private bool isAttacking;
    private float nextAttackTime;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform player;
    private Collider2D[] collidersCache;

    private void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        collidersCache = GetComponentsInChildren<Collider2D>();

        if (tutorialManager == null)
        {
            tutorialManager = FindObjectOfType<TutorialManager>();
        }

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        float distanceToPlayer = player != null
            ? Vector2.Distance(transform.position, player.position)
            : Mathf.Infinity;

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            Attack();
        }
        else if (!isAttacking)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * patrolSpeed * Time.deltaTime);

        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(direction) * patrolSpeed);
        }

        PlayStateIfNeeded(walkStateName);

        if (spriteRenderer != null)
        {
            // Este mushroom mira a la derecha cuando flipX es true.
            spriteRenderer.flipX = movingRight;
        }

        if (transform.position.x >= startPosition.x + patrolRange)
        {
            movingRight = false;
        }
        else if (transform.position.x <= startPosition.x - patrolRange)
        {
            movingRight = true;
        }
    }

    private void Attack()
    {
        isAttacking = true;
        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.ResetTrigger("Die");
            animator.SetTrigger("Attack");
        }

        PlayStateNow(attackStateName);

        DealDamageToPlayer();
        Invoke(nameof(FinishAttack), 0.35f);
    }

    private void DealDamageToPlayer()
    {
        if (player == null)
        {
            return;
        }

        if (Vector2.Distance(transform.position, player.position) > attackRange + 0.2f)
        {
            return;
        }

        if (playerLayer.value != 0 && ((1 << player.gameObject.layer) & playerLayer) == 0)
        {
            return;
        }

        IDamageable damageable =
            player.GetComponent<IDamageable>() ??
            player.GetComponentInChildren<IDamageable>() ??
            player.GetComponentInParent<IDamageable>();

        if (damageable != null)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = player.position.x >= transform.position.x;
            }

            Vector2 direction = (player.position - transform.position).normalized;
            damageable.TakeDamage(attackDamage, transform.position, direction);
        }
    }

    private void FinishAttack()
    {
        isAttacking = false;
        PlayStateIfNeeded(idleStateName);
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDirection)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= amount;
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Compatibilidad con cualquier script viejo que siga llamando TakeDamage(int)
    public void TakeDamage(int amount)
    {
        TakeDamage(amount, transform.position, Vector2.zero);
    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Die");
        }

        PlayStateNow(deathStateName);

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }

        if (collidersCache != null)
        {
            foreach (Collider2D enemyCollider in collidersCache)
            {
                enemyCollider.enabled = false;
            }
        }

        tutorialManager?.OnEnemyKilled();
        Destroy(gameObject, destroyDelay);
    }

    private void PlayStateIfNeeded(string stateName)
    {
        if (animator == null || string.IsNullOrWhiteSpace(stateName))
        {
            return;
        }

        int stateHash = Animator.StringToHash(stateName);
        if (!animator.HasState(0, stateHash))
        {
            return;
        }

        AnimatorStateInfo currentState = animator.GetCurrentAnimatorStateInfo(0);
        if (currentState.shortNameHash == stateHash)
        {
            return;
        }

        animator.Play(stateHash, 0, 0f);
    }

    private void PlayStateNow(string stateName)
    {
        if (animator == null || string.IsNullOrWhiteSpace(stateName))
        {
            return;
        }

        int stateHash = Animator.StringToHash(stateName);
        if (!animator.HasState(0, stateHash))
        {
            return;
        }

        animator.Play(stateHash, 0, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 patrolCenter = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawWireSphere(patrolCenter + Vector3.right * patrolRange, 0.15f);
        Gizmos.DrawWireSphere(patrolCenter + Vector3.left * patrolRange, 0.15f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
