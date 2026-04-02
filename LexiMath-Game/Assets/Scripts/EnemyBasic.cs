using UnityEngine;

public class EnemyBasic : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 3;
    public float patrolSpeed = 1.5f;
    public float patrolRange = 3f;

    private int currentHealth;
    private Vector3 startPosition;
    private bool movingRight = true;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        startPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        float direction = movingRight ? 1f : -1f;
        transform.Translate(Vector2.right * direction * patrolSpeed * Time.deltaTime);

        if (transform.position.x >= startPosition.x + patrolRange)
        {
            movingRight = false;
            spriteRenderer.flipX = true;
        }
        else if (transform.position.x <= startPosition.x - patrolRange)
        {
            movingRight = true;
            spriteRenderer.flipX = false;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
            Die();
    }

    System.Collections.IEnumerator DamageFlash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    void Die()
    {
        Destroy(gameObject, 0.3f);
    }
}