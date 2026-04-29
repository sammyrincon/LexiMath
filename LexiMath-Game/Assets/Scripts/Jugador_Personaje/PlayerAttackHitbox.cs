using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask enemyLayer;

    private readonly HashSet<IDamageable> damagedThisSwing = new HashSet<IDamageable>();

    private void OnEnable()
    {
        damagedThisSwing.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (enemyLayer.value != 0 && ((1 << other.gameObject.layer) & enemyLayer) == 0) return;

        IDamageable dmg = other.GetComponent<IDamageable>();
        if (dmg == null) dmg = other.GetComponentInParent<IDamageable>();

        if (dmg != null && !damagedThisSwing.Contains(dmg))
        {
            damagedThisSwing.Add(dmg);

            Vector2 dir = (other.transform.position - transform.position).normalized;
            dmg.TakeDamage(damage, transform.position, dir);
        }
    }
}
