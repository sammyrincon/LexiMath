using UnityEngine;

/// <summary>
/// KnightAttack — LexiMath
/// 
/// Va en el Knight. Cuando presiona X busca enemigos cerca y les hace daño.
/// 
/// SETUP EN UNITY:
///   1. Add Component en el Knight → KnightAttack
///   2. Configura:
///        • Attack Range  → radio del ataque (0.8 ~ 1.5 recomendado)
///        • Attack Damage → daño por golpe (1 recomendado, 3 golpes matan)
///        • Enemy Layer   → selecciona "Enemy"
///   3. NO asignes Attack Point manualmente — se genera un punto frente al Knight
/// </summary>
public class KnightAttack : MonoBehaviour
{
    [Header("Ataque")]
    [Tooltip("Daño que hace cada golpe")]
    public int attackDamage = 1;

    [Tooltip("Radio del área de ataque")]
    public float attackRange = 1f;

    [Tooltip("Distancia del punto de ataque desde el centro del Knight")]
    public float attackOffsetX = 0.6f;

    [Tooltip("Altura del punto de ataque")]
    public float attackOffsetY = 0f;

    [Header("Cooldown")]
    public float attackCooldown = 0.5f;

    [Header("Capa de enemigos")]
    public LayerMask enemyLayer;

    // ── Privados ─────────────────────────────────────────────
    private SpriteRenderer _sprite;
    private float _nextAttackTime = 0f;

    void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && Time.time >= _nextAttackTime)
        {
            HacerAtaque();
            _nextAttackTime = Time.time + attackCooldown;
        }
    }

    private void HacerAtaque()
    {
        Vector2 centroAtaque = ObtenerCentroAtaque();

        // Detectar enemigos en el área
        Collider2D[] enemigos = Physics2D.OverlapCircleAll(
            centroAtaque, attackRange, enemyLayer);

        foreach (Collider2D enemigo in enemigos)
        {
            EnemyBasic basic = enemigo.GetComponent<EnemyBasic>();
            if (basic != null)
                basic.TakeDamage(attackDamage);
        }
    }

    /// <summary>
    /// Calcula el punto central del ataque frente al Knight
    /// (cambia según la dirección en la que mira).
    /// </summary>
    private Vector2 ObtenerCentroAtaque()
    {
        float dir = _sprite != null && _sprite.flipX ? -1f : 1f;
        return new Vector2(
            transform.position.x + (attackOffsetX * dir),
            transform.position.y + attackOffsetY
        );
    }

    // ── Gizmo para ver el área en Scene view ─────────────────
    void OnDrawGizmosSelected()
    {
        if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
        Vector2 centro = ObtenerCentroAtaque();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centro, attackRange);
    }
}
