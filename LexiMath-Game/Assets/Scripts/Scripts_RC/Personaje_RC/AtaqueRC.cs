using UnityEngine;

public class AtaqueRC : MonoBehaviour
{
    public int danoAtaque = 25;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo hace daño a los objetos con el Tag "Enemy"
        if (collision.CompareTag("Enemy"))
        {
            Sistema_Salud_RC saludEnemigo = collision.GetComponent<Sistema_Salud_RC>();
            
            if (saludEnemigo != null)
            {
                saludEnemigo.RecibirDano(danoAtaque);
            }
        }
    }
} 