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
    public int damageOnFinalFail = 50;
    
    [Header("References")]
    public Transform player;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    private Rigidbody2D rb;
    
    private bool isAttacking = false;
    private bool isHurt = false;
    private bool isDead = false;
    private bool finalQuestionTriggered = false;
    private float lastAttackTime = -999f;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
        
        // Voltear sprite y hitbox según dirección al jugador
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
            // Idle
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
        animator.SetBool("isAttacking", true);
        
        yield return new WaitForSeconds(attackDuration * 0.4f);
        
        if (attackHitbox != null) attackHitbox.SetActive(true);
        
        yield return new WaitForSeconds(attackDuration * 0.2f);
        
        if (attackHitbox != null) attackHitbox.SetActive(false);
        
        yield return new WaitForSeconds(attackDuration * 0.4f);
        
        animator.SetBool("isAttacking", false);
        isAttacking = false;
    }
    
    public void TakeDamage(int damage, Vector2 sourcePosition)
    {
        if (isDead || finalQuestionTriggered) return;

        currentHealth -= damage;

        // Knockback: empujar al boss en dirección opuesta al golpe
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

        // Flash más pronunciado
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = Color.white;

        // Reducir el tiempo total de hurt para que el boss reaccione rápido
        yield return new WaitForSeconds(0.1f);

        animator.SetBool("isDefending", false);
        isHurt = false;
    }
    
    private void TriggerFinalQuestion()
    {
        finalQuestionTriggered = true;
        currentHealth = 1;
        rb.linearVelocity = Vector2.zero;
        
        // Boss arrodillado/agonizando
        animator.SetBool("isDefending", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isRunning", false);
        
        QuestionData bossQuestion = QuestionPool.GenerateBossQuestion();
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
            currentHealth = maxHealth / 2;
            finalQuestionTriggered = false;
            animator.SetBool("isDefending", false);
            Debug.Log("¡Fallaste la pregunta! El boss recupera fuerzas.");
        }
    }
    
    private IEnumerator DieRoutine()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        
        // Disparar animación de muerte
        animator.SetBool("isDefending", false);
        animator.SetBool("isDead", true);
        
        PlayerSouls.Instance.AddSouls(soulsReward);
        Debug.Log("¡VICTORIA! Boss derrotado.");
        
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