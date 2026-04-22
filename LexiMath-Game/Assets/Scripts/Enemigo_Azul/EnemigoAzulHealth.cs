using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemigoAzulHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private int maxHP = 3;

    [Header("Invencibilidad (evita hits múltiples instantáneos)")]
    [SerializeField] private float invincibilityTime = 0.15f;

    [Header("Referencias (opcional pero recomendado)")]
    [Tooltip("IA del enemigo. Se desactiva al morir.")]
    [SerializeField] private MonoBehaviour aiToDisable;

    [Tooltip("Combat del enemigo (para apagar hitbox al morir).")]
    [SerializeField] private EnemigoAzulCombat combat;

    [Tooltip("Collider del cuerpo (NO el trigger de daño). Se mantiene activo para que NO atraviese el suelo.")]
    [SerializeField] private Collider2D bodyCollider;

    [Header("Destruir al final de Die")]
    [Tooltip("Tiempo extra después de terminar la animación (normalmente 0).")]
    [SerializeField] private float destroyDelayExtra = 0f;

    private Animator anim;
    private Rigidbody2D rb;

    private int hp;
    private float invTimer;
    private bool dead;

    public bool IsDead => dead;
    public int HP => hp;

    private void Awake()
    {
        hp = maxHP;

        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (aiToDisable == null) aiToDisable = GetComponent<EnemigoAzulIA>();
        if (combat == null) combat = GetComponent<EnemigoAzulCombat>();
        if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();

        // Recomendado para 2D
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (invTimer > 0f) invTimer -= Time.deltaTime;
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDirection)
    {
        if (dead) return;
        if (invTimer > 0f) return;

        hp -= amount;
        invTimer = invincibilityTime;

        // Animación hurt
        anim.SetTrigger("Hurt");

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (dead) return;
        dead = true;

        anim.ResetTrigger("Hurt");
        anim.SetTrigger("Die");

        if (aiToDisable != null) aiToDisable.enabled = false;

        if (combat != null)
        {
            combat.DisableHitboxForDeath();
            combat.enabled = false;
        }


        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.gravityScale = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;


    }


    public void DestroyAfterDeath()
    {
        Destroy(gameObject, destroyDelayExtra);
    }
} 