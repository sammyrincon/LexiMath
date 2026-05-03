using UnityEngine;

public class AtaqueEnemigoRC : MonoBehaviour
{
    [Header("Daño del Arma")]
    public int danoAtaque = 10;
    
    private Collider2D hitbox;

    void Start()
    {
        hitbox = GetComponent<Collider2D>();
        
        ApagarHitbox();
    }

    #pragma warning disable IDE0051
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Sistema_Salud_RC saludJugador = collision.GetComponent<Sistema_Salud_RC>();
            
            if (saludJugador != null)
            {
                saludJugador.RecibirDano(danoAtaque);
                
                ApagarHitbox();
            }
        }
    }
    #pragma warning restore IDE0051

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