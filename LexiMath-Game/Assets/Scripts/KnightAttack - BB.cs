using UnityEngine;

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

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(
            centroAtaque, attackRange, enemyLayer);

        foreach (Collider2D enemigo in enemigos)
        {
            EnemyBasic basic = enemigo.GetComponent<EnemyBasic>();
            if (basic != null)
                basic.TakeDamage(attackDamage);
        }
    }

    private Vector2 ObtenerCentroAtaque()
    {
        float dir = _sprite != null && _sprite.flipX ? -1f : 1f;
        return new Vector2(
            transform.position.x + (attackOffsetX * dir),
            transform.position.y + attackOffsetY
        );
    }

    void OnDrawGizmosSelected()
    {
        if (_sprite == null) _sprite = GetComponent<SpriteRenderer>();
        Vector2 centro = ObtenerCentroAtaque();
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centro, attackRange);
    }
}
