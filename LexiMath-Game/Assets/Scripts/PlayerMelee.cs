using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public float attackRate = 0.5f;
    public LayerMask enemyLayer;

    private float nextAttackTime = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z))
            && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + attackRate;
        }
    }
    void Attack()
    {
        if (animator != null)
            animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyBasic>()?.TakeDamage(attackDamage);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}