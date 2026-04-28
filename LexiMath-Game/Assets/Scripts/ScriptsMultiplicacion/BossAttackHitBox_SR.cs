using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    public int damage = 20;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth ph = other.GetComponent<PlayerHealth>();
            if (ph != null) ph.TakeDamage(damage);
        }
    }
}