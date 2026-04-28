using UnityEngine;
using System.Collections;

public class CatWander : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.5f;
    public float minWalkTime = 1.5f;
    public float maxWalkTime = 4f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;
    
    [Header("Patrol Range")]
    public float patrolRange = 5f;     // qué tanto se puede alejar del spawn (en X)
    public bool horizontalOnly = true; // solo izquierda/derecha, o también arriba/abajo
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    private Vector2 spawnPosition;
    private Vector2 currentDirection;
    private bool isWalking;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spawnPosition = transform.position;
    }
    
    private void Start()
    {
        StartCoroutine(WanderRoutine());
    }
    
    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            // Fase de caminar
            PickRandomDirection();
            isWalking = true;
            UpdateAnimation();
            
            float walkTime = Random.Range(minWalkTime, maxWalkTime);
            float elapsed = 0f;
            
            while (elapsed < walkTime)
            {
                // Si se aleja demasiado del spawn, regresar
                float distanceFromSpawn = transform.position.x - spawnPosition.x;
                if (Mathf.Abs(distanceFromSpawn) > patrolRange)
                {
                    // Forzar dirección de regreso
                    currentDirection.x = distanceFromSpawn > 0 ? -1f : 1f;
                    UpdateFlip();
                }
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            // Fase de idle (parado)
            isWalking = false;
            UpdateAnimation();
            rb.linearVelocity = Vector2.zero;
            
            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            yield return new WaitForSeconds(idleTime);
        }
    }
    
    private void FixedUpdate()
    {
        if (isWalking)
            rb.linearVelocity = currentDirection * moveSpeed;
        else
            rb.linearVelocity = Vector2.zero;
    }
    
    private void PickRandomDirection()
    {
        if (horizontalOnly)
        {
            // Solo izquierda o derecha
            currentDirection = Random.value > 0.5f ? Vector2.right : Vector2.left;
        }
        else
        {
            // 4 direcciones
            int dir = Random.Range(0, 4);
            switch (dir)
            {
                case 0: currentDirection = Vector2.right; break;
                case 1: currentDirection = Vector2.left; break;
                case 2: currentDirection = Vector2.up; break;
                case 3: currentDirection = Vector2.down; break;
            }
        }
        
        UpdateFlip();
    }
    
    private void UpdateFlip()
    {
        // Voltear sprite según dirección horizontal
        if (currentDirection.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (currentDirection.x < -0.01f)
            spriteRenderer.flipX = true;
    }
    
    private void UpdateAnimation()
    {
        if (animator == null) return;
        
        if (isWalking)
            animator.Play("CatRun");
        else
            animator.Play("Cat3Idle");
    }
    
    // Para ver el rango de patrullaje en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = Application.isPlaying ? (Vector3)spawnPosition : transform.position;
        Gizmos.DrawWireCube(center, new Vector3(patrolRange * 2, 1f, 0));
    }
}