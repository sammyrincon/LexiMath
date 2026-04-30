using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovimientoJugador_RC : MonoBehaviour
{
    public float velocidad = 5f;
    private Rigidbody2D rb;
    private Animator animador;
    private SpriteRenderer spriteRenderer;
    private float movimientoX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animador = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        movimientoX = Input.GetAxisRaw("Horizontal");

        if (movimientoX != 0)
        {
            animador.SetFloat("Speed", 1f);
        }
        else
        {
            animador.SetFloat("Speed", 0f);
        }

        if (movimientoX < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (movimientoX > 0)
        {
            spriteRenderer.flipX = false;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movimientoX * velocidad, rb.linearVelocity.y);
    }
}