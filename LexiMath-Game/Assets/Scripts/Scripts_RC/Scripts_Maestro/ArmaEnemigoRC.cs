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
    // Este metodo se detecta la colision con el jugador y se le aplica daño utilizando el sistema de salud del jugador
    // La variable collision representa el objeto con el que se ha colisionado, en este caso se espera que sea el jugador
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
    // Este metodo se encarga de activar la hitbox del arma para que pueda causar daño al jugador durante un ataque
    public void ActivarHitbox()
    {
        if (hitbox != null)
        {
            hitbox.enabled = true; 
        }
    }
    // Este metodo se encarga de apagar la hitbox del arma para evitar que cause daño multiple en un solo ataque 
    
    public void ApagarHitbox()
    {
        if (hitbox != null)
        {
            hitbox.enabled = false; 
        }
    }
}