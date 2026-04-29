using UnityEngine;

public class AtaqueJugadorRC : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public int danoAtaque = 20;

    private Collider2D hitbox;
    private Animator animator;
    private PlayerJumpRC playerJump;

    void Start()
    {
        hitbox = GetComponent<Collider2D>();
        ApagarHitbox();

        animator = GetComponentInParent<Animator>();
        playerJump = GetComponentInParent<PlayerJumpRC>();

        if (animator == null)
            Debug.LogWarning("AtaqueJugadorRC: no se encontró Animator en el padre.");
        if (playerJump == null)
            Debug.LogWarning("AtaqueJugadorRC: no se encontró PlayerJumpRC en el padre.");
    }

    public void IniciarAtaque()
    {
        if (playerJump != null)
        {
            playerJump.StartAttack();       
            playerJump.ResetJumpTrigger();  
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void FinalizarAtaque()
    {
        ApagarHitbox();

        if (playerJump != null)
        {
            playerJump.EndAttack();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Sistema_Salud_RC saludEnemigo = collision.GetComponent<Sistema_Salud_RC>();

            if (saludEnemigo != null)
            {
                saludEnemigo.RecibirDano(danoAtaque);
                ApagarHitbox();
            }
        }
    }

    public void PrenderHitbox()
    {
        if (hitbox != null)
        {
            hitbox.enabled = true;
        }
    }

    public void ApagarHitbox()
    {
        if (hitbox != null)
        {
            hitbox.enabled = false;
        }
    }
}
