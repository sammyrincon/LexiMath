// Assets/Scripts/Player/PlayerControler_SR.cs
using UnityEngine;

public class PlayerController2_Sr : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float acceleration = 50f;
    public float deceleration = 60f;
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    private Vector2 input;
    private Vector2 currentVelocity;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        
        if (input.sqrMagnitude > 1f)
            input.Normalize();
        
        // Voltear sprite según dirección
        if (input.x > 0.01f)
            spriteRenderer.flipX = false;
        else if (input.x < -0.01f)
            spriteRenderer.flipX = true;
        
        UpdateAnimations();
    }
    
    private void FixedUpdate()
    {
        Vector2 targetVelocity = input * moveSpeed;
        float rate = input.sqrMagnitude > 0.01f ? acceleration : deceleration;
        currentVelocity = Vector2.MoveTowards(currentVelocity, targetVelocity, rate * Time.fixedDeltaTime);
        
        rb.linearVelocity = currentVelocity;
    }
    
    private void UpdateAnimations()
    {
        if (animator == null) return;
        bool running = currentVelocity.sqrMagnitude > 0.1f;
        animator.SetBool("isRunning", running);
    }
}