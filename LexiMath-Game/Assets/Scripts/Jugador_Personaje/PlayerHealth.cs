using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private int maxHP = 5;

    [Header("Invencibilidad (I-Frames)")]
    [SerializeField] private float invincibilityTime = 0.6f;
    [SerializeField] private bool flashSprite = true;
    [SerializeField] private float flashInterval = 0.08f;

    [Header("Hurt Lock (bloquea control un momento)")]
    [SerializeField] private float hurtLockTime = 0.25f;

    [Header("Knockback (empuje al recibir daño)")]
    [SerializeField] private bool useKnockback = true;
    [SerializeField] private float knockbackX = 7f;
    [SerializeField] private float knockbackY = 6f;

    [Header("Referencias (opcional)")]
    [SerializeField] private MonoBehaviour controllerToDisable; 
    [SerializeField] private Collider2D bodyCollider;          
    [SerializeField] private SpriteRenderer spriteRenderer;  
    private int hp;
    private bool invincible;
    private bool isDead;

    private Animator anim;
    private Rigidbody2D rb;

    public bool IsDead => isDead;
    public int HP => hp;

    void Awake()
    {
        hp = maxHP;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDirection)
    {
        if (isDead) return;
        if (invincible) return;

        hp -= amount;

        anim.ResetTrigger("Attack"); // opcional
        anim.SetTrigger("Hurt");

        if (useKnockback)
        {
            Vector2 dir = hitDirection.normalized;
            rb.linearVelocity = new Vector2(dir.x * knockbackX, knockbackY);
        }

        StartCoroutine(InvincibilityRoutine());
        StartCoroutine(HurtLockRoutine());

        // Muerte
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.SetTrigger("Die");

        if (controllerToDisable != null) controllerToDisable.enabled = false;

        if (bodyCollider != null) bodyCollider.enabled = false;

        rb.linearVelocity = Vector2.zero;
        rb.simulated = true; // o false si quieres congelarlo
    }

    private IEnumerator HurtLockRoutine()
    {
        if (controllerToDisable != null) controllerToDisable.enabled = false;
        yield return new WaitForSeconds(hurtLockTime);
        if (!isDead && controllerToDisable != null) controllerToDisable.enabled = true;
    }

    private IEnumerator InvincibilityRoutine()
    {
        invincible = true;

        if (flashSprite && spriteRenderer != null)
        {
            float t = 0f;
            while (t < invincibilityTime)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(flashInterval);
                t += flashInterval;
            }
            spriteRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityTime);
        }

        invincible = false;
    }
}