using UnityEngine;

public class ArmaEnemigoRC : MonoBehaviour
{
    public int danoAtaque = 20;
    private BoxCollider2D hitbox;

    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
        ApagarHitbox();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Sistema_Salud_RC saludMael = collision.GetComponent<Sistema_Salud_RC>();
            if (saludMael != null)
            {
                saludMael.RecibirDano(danoAtaque);
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