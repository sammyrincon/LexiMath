using UnityEngine;
using System.Collections;

public class PlayerControler_SR : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 50f;
    public float deceleration = 60f;
    
    [Header("Combat")]
    public float attackDuration = 0.25f;        // ← más rápido
    public float damageDelay = 0.05f;           // ← daño casi instantáneo
    public KeyCode attackKey = KeyCode.X;
    public int attackDamage = 20;
    public float attackRange = 1.5f;            // ← rango más generoso
    public LayerMask enemyLayer;
    public Transform attackPoint;
    
    [Header("Feedback")]
    public float hitStopDuration = 0.08f;       // pausa al golpear (game feel clásico)
    public float screenShakeIntensity = 0.1f;   // opcional
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    private Vector2 input;
    private Vector2 currentVelocity;
    private bool isAttacking;
    private float attackTimer;
    private bool damageApplied;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        if (!isAttacking)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            
            if (input.sqrMagnitude > 1f)
                input.Normalize();
            
            if (Input.GetKeyDown(attackKey))
            {
                StartAttack();
            }
            
            // Flip sprite y attack point
            if (input.x > 0.01f)
            {
                spriteRenderer.flipX = false;
                if (attackPoint != null)
                    attackPoint.localPosition = new Vector3(0.7f, 0f, 0f);
            }
            else if (input.x < -0.01f)
            {
                spriteRenderer.flipX = true;
                if (attackPoint != null)
                    attackPoint.localPosition = new Vector3(-0.7f, 0f, 0f);
            }
        }
        else
        {
            input = Vector2.zero;
            attackTimer -= Time.deltaTime;
            
            // Aplicar daño en el momento correcto del swing
            if (!damageApplied && attackTimer <= attackDuration - damageDelay)
            {
                DealDamage();
                damageApplied = true;
            }
            
            if (attackTimer <= 0f)
                EndAttack();
        }
        
        UpdateAnimations();
    }
    
    private void FixedUpdate()
    {
        Vector2 targetVelocity = input * moveSpeed;
        float rate = input.sqrMagnitude > 0.01f ? acceleration : deceleration;
        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);
        
        rb.linearVelocity = currentVelocity;
    }
    
    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        damageApplied = false;
        animator.SetBool("isAttacking", true);
    }
    
    private void DealDamage()
    {
        Vector2 attackPosition;
        if (attackPoint != null)
            attackPosition = attackPoint.position;
        else
        {
            float direction = spriteRenderer.flipX ? -1f : 1f;
            attackPosition = (Vector2)transform.position + new Vector2(direction * 0.7f, 0f);
        }
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, attackRange, enemyLayer);
        
        bool hitSomething = false;
        foreach (var hit in hits)
        {
            BossController boss = hit.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(attackDamage, transform.position);
                hitSomething = true;
                Debug.Log($"¡Golpe al boss! -{attackDamage} HP. Boss HP: {boss.currentHealth}");
            }
        }
        
        // Hit stop: si conectaste el golpe, pequeña pausa para sentir el impacto
        if (hitSomething)
        {
            StartCoroutine(HitStop());
        }
    }
    
    private IEnumerator HitStop()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }
    
    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("isAttacking", false);
    }
    
    private void UpdateAnimations()
    {
        bool running = currentVelocity.sqrMagnitude > 0.1f && !isAttacking;
        animator.SetBool("isRunning", running);
    }
    
    public void TakeDamage()
    {
        StartCoroutine(HurtRoutine());
    }
    
    private IEnumerator HurtRoutine()
    {
        animator.SetBool("isHurt", true);
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("isHurt", false);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector2 pos = attackPoint != null ? (Vector2)attackPoint.position : (Vector2)transform.position;
        Gizmos.DrawWireSphere(pos, attackRange);
    }
}