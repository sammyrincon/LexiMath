using UnityEngine;

// Script para el combate del jugador. 


public class PlayerCombat : MonoBehaviour
{
    [Header("Projectile")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float fireRate = 0.5f;

    private float nextFireTime = 0f;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Z))
            && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();

        float direction = spriteRenderer.flipX ? -1f : 1f;
        projRb.linearVelocity = new Vector2(direction * projectileSpeed, 0f);

        if (direction < 0)
            projectile.transform.localScale = new Vector3(-1, 1, 1);

        Destroy(projectile, 3f);

        if (animator != null)
            animator.SetTrigger("Attack");
    }
}



