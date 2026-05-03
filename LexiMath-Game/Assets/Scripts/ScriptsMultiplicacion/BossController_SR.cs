using UnityEngine;
using System.Collections;

public class BossController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int contactDamage = 15;
    public int soulsReward = 500;

    [Header("Movement & Detection")]
    public float moveSpeed = 2f;
    public float detectionRange = 8f;
    public float attackRange = 1.5f;

    [Header("Attack")]
    public float attackCooldown = 2f;
    public float attackDuration = 0.6f;
    public GameObject attackHitbox;

    [Header("Final Question")]
    public int damageOnFinalFail = 20;

    [Header("References")]
    public Transform player;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Rigidbody2D rb;

    private bool isAttacking = false;
    private bool isHurt = false;
    private bool isDead = false;
    private bool finalQuestionTriggered = false;
    private float lastAttackTime = -999f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    private void Update()
    {
        if (isDead || finalQuestionTriggered || player == null) return;
        if (isAttacking || isHurt) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
            if (attackHitbox != null)
                attackHitbox.transform.localPosition = new Vector3(1f, 0f, 0f);
        }
        else
        {
            spriteRenderer.flipX = true;
            if (attackHitbox != null)
                attackHitbox.transform.localPosition = new Vector3(-1f, 0f, 0f);
        }

        if (distance <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
                StartCoroutine(AttackRoutine());
        }
        else if (distance <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isRunning", false);
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        animator.SetBool("isRunning", true);
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        lastAttackTime = Time.time;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isRunning", false);
        animator.SetTrigger("isAttacking");

        yield return new WaitForSeconds(attackDuration * 0.4f);

        if (attackHitbox != null) attackHitbox.SetActive(true);

        yield return new WaitForSeconds(attackDuration * 0.2f);

        if (attackHitbox != null) attackHitbox.SetActive(false);

        yield return new WaitForSeconds(attackDuration * 0.4f);

        isAttacking = false;
    }

    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        if (isDead || finalQuestionTriggered) return;

        currentHealth -= damage;

        Vector2 knockbackDir = ((Vector2)transform.position - sourcePosition).normalized;
        rb.linearVelocity = knockbackDir * 5f;

        StartCoroutine(HurtRoutine());

        if (currentHealth <= 0)
            TriggerFinalQuestion();
    }

    private IEnumerator HurtRoutine()
    {
        isHurt = true;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isDefending", true);

        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(0.1f);

        animator.SetBool("isDefending", false);
        isHurt = false;
    }

    private void TriggerFinalQuestion()
    {
        finalQuestionTriggered = true;
        currentHealth = 1;
        rb.linearVelocity = Vector2.zero;

        animator.SetBool("isRunning", false);
        animator.SetBool("isDefending", true);

        QuestionData_SR bossQuestion = QuestionPool.GenerateBossQuestion();
        QuestionUIManager.Instance.ShowQuestion(bossQuestion, OnFinalAnswered);
    }

    private void OnFinalAnswered(bool correct)
    {
        if (correct)
        {
            StartCoroutine(DieRoutine());
        }
        else
        {
            PlayerHealth_SR.Instance.TakeDamage(damageOnFinalFail);
            currentHealth = maxHealth / 4;
            finalQuestionTriggered = false;
            animator.SetBool("isDefending", false);
        }
    }

    private IEnumerator DieRoutine()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;

        animator.SetBool("isDefending", false);
        animator.SetTrigger("isDead");

        PlayerSouls.Instance.AddSouls(soulsReward);

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
