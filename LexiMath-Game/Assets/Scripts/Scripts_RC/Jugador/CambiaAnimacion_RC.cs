using UnityEngine;

public class CambiaAnimacion_RC : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private EstadoPersonaje estadoPersonaje;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        estadoPersonaje = GetComponentInChildren<EstadoPersonaje>();
    }

    void Update()
    {
        animator.SetFloat("velocidad", Mathf.Abs(rb.linearVelocityX));

        if (rb.linearVelocityX < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb.linearVelocityX > 0.1f)
        {
            spriteRenderer.flipX = false;
        }

        animator.SetBool("enPiso", estadoPersonaje.estaEnPiso);
    }
}