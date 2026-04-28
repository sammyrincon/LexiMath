// Assets/Scripts/Enemies/CatLicking.cs
using UnityEngine;

public class CatLicking : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
    private void Start()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
        
        if (animator != null)
            animator.Play("Cat3Lick");
    }
}