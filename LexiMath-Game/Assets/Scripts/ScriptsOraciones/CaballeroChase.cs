// Assets/Scripts/Enemies/CaballeroChase.cs
using UnityEngine;

public class CaballeroChase : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public Transform[] patrolPoints;
    
    [Header("Detection")]
    public float visionRange = 6f;
    public float visionAngle = 90f; // ángulo del cono de visión en grados
    public LayerMask playerLayer;
    public LayerMask obstacleLayer; // walls que bloquean visión
    
    [Header("References")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    
    private Rigidbody2D rb;
    private Transform player;
    private int currentPatrolIndex = 0;
    private bool isChasing = false;
    private Vector2 facingDirection = Vector2.right;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }
    
    private void Update()
    {
        if (player == null) return;
        
        bool canSeePlayer = CanSeePlayer();
        
        if (canSeePlayer)
        {
            isChasing = true;
            ChasePlayer();
        }
        else if (isChasing)
        {
            // Pierde de vista al jugador, regresa a patrullar
            isChasing = false;
        }
        else
        {
            Patrol();
        }
        
        UpdateAnimation();
    }
    
    private bool CanSeePlayer()
    {
        Vector2 toPlayer = (Vector2)player.position - (Vector2)transform.position;
        float distance = toPlayer.magnitude;
        
        if (distance > visionRange) return false;
        
        // Check ángulo del cono
        float angle = Vector2.Angle(facingDirection, toPlayer.normalized);
        if (angle > visionAngle * 0.5f) return false;
        
        // Raycast para verificar que no hay paredes en medio
        RaycastHit2D hit = Physics2D.Raycast(transform.position, toPlayer.normalized, distance, obstacleLayer);
        if (hit.collider != null) return false; // hay pared en medio
        
        return true;
    }
    
    private void ChasePlayer()
    {
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
        UpdateFacing(direction);
    }
    
    private void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        Transform target = patrolPoints[currentPatrolIndex];
        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;
        UpdateFacing(direction);
        
        // Cambiar de punto cuando esté cerca
        if (Vector2.Distance(transform.position, target.position) < 0.3f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }
    
    private void UpdateFacing(Vector2 dir)
    {
        if (dir.sqrMagnitude < 0.01f) return;
        facingDirection = dir;
        
        if (dir.x > 0.01f) spriteRenderer.flipX = false;
        else if (dir.x < -0.01f) spriteRenderer.flipX = true;
    }
    
    private void UpdateAnimation()
    {
        if (animator == null) return;
        bool moving = rb.linearVelocity.sqrMagnitude > 0.1f;
        animator.SetBool("isRunning", moving);
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerLives.Instance.LoseLife();
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Vision range
        Gizmos.color = isChasing ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
        
        // Vision cone
        Vector2 facing = Application.isPlaying ? facingDirection : Vector2.right;
        float halfAngle = visionAngle * 0.5f * Mathf.Deg2Rad;
        Vector2 leftRay = new Vector2(
            facing.x * Mathf.Cos(halfAngle) - facing.y * Mathf.Sin(halfAngle),
            facing.x * Mathf.Sin(halfAngle) + facing.y * Mathf.Cos(halfAngle)
        );
        Vector2 rightRay = new Vector2(
            facing.x * Mathf.Cos(-halfAngle) - facing.y * Mathf.Sin(-halfAngle),
            facing.x * Mathf.Sin(-halfAngle) + facing.y * Mathf.Cos(-halfAngle)
        );
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)leftRay * visionRange);
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)rightRay * visionRange);
        
        // Patrol points
        if (patrolPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var p in patrolPoints)
                if (p != null) Gizmos.DrawWireSphere(p.position, 0.2f);
        }
    }
}