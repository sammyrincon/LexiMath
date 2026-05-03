using UnityEngine;
public class ArmaEnemigoRC : MonoBehaviour
{
    public int danoDelGolpe = 30;
    private Collider2D hitbox;

    void Start()
    {
        hitbox = GetComponent<Collider2D>();
        ApagarHitbox();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Sistema_Salud_RC saludJugador = collision.GetComponent<Sistema_Salud_RC>();
            if (saludJugador != null)
            {
                saludJugador.RecibirDano(danoDelGolpe);

                ApagarHitbox();
            }
        }
    }

    public void ActivarHitbox()
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
