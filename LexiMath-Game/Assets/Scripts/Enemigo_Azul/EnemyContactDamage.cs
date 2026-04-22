using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitCooldown = 0.6f;
    [SerializeField] private LayerMask playerLayer;

    private float timer;

    private void Update()
    {
        if (timer > 0f) timer -= Time.deltaTime;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (timer > 0f) return;

        // 1) Filtrado por layer
        if (((1 << other.gameObject.layer) & playerLayer) == 0) return;

        // 2) Buscar IDamageable de forma robusta (padre/hijo)
        IDamageable dmg =
            other.GetComponent<IDamageable>() ??
            other.GetComponentInParent<IDamageable>() ??
            other.GetComponentInChildren<IDamageable>();

        if (dmg != null)
        {
            Vector2 dir = (other.transform.position - transform.position).normalized;
            dmg.TakeDamage(damage, transform.position, dir);
            timer = hitCooldown;
        }
    }
}