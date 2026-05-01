using UnityEngine;
using UnityEngine.UIElements;

public class Sistema_Salud_RC : MonoBehaviour
{
    public int saludMaxima = 100;
    public int saludActual { get; private set; }
    
    public UIDocument documentoUI;
    public float tiempoMuerteJugador = 5.0f;
    public float tiempoMuerteEnemigo = 0.5f;

    [Header("Efectos de Sonido")]
    public AudioClip sonidoRecibirDano;
    public AudioClip sonidoMuerte;

    private VisualElement healthbar_fill; 
    private Animator animador;
    private AudioSource reproductorSonido; 

    void Start()
    {
        saludActual = saludMaxima;
        animador = GetComponent<Animator>();
        reproductorSonido = GetComponent<AudioSource>(); 

        if (documentoUI != null)
        {
            var root = documentoUI.rootVisualElement;
            healthbar_fill = root.Q<VisualElement>("healthbar-fill");
        }

        ActualizarBarraVida();
    }

    public void RecibirDano(int cantidad)
    {
        saludActual -= cantidad;
        
        if (saludActual <= 0) 
        {
            saludActual = 0;
            ActualizarBarraVida();
            Morir();
        }
        else
        {
            ActualizarBarraVida();
            if (animador != null)
            {
                animador.SetTrigger("Hurt"); 
            }
            
            // Reproducir sonido de golpe
            if (reproductorSonido != null && sonidoRecibirDano != null)
            {
                reproductorSonido.PlayOneShot(sonidoRecibirDano);
            }
        }
    }

    private void ActualizarBarraVida()
    {
        if (healthbar_fill != null)
        {
            float porcentaje = ((float)saludActual / saludMaxima) * 100f;
            healthbar_fill.style.width = new Length(porcentaje, LengthUnit.Percent);
        }
    }

    private void Morir()
    {
        if (animador != null)
        {
            animador.SetTrigger("Death");
        }

        if (reproductorSonido != null && sonidoMuerte != null)
        {
            reproductorSonido.PlayOneShot(sonidoMuerte);
        }

        if (gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, tiempoMuerteJugador);
        }
        else
        {
            Destroy(gameObject, tiempoMuerteEnemigo);
        }
    }

    public void Curar(int cantidad)
    {
        // ... (Tu código de curar se queda igual)
        saludActual += cantidad;
        if (saludActual > saludMaxima) saludActual = saludMaxima;
        ActualizarBarraVida();
    }
}