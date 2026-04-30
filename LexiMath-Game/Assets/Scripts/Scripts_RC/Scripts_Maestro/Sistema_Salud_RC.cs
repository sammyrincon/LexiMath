using UnityEngine;
using UnityEngine.UIElements;

public class Sistema_Salud_RC : MonoBehaviour
{
    public int saludMaxima = 100;
    public int saludActual { get; private set; }
    
    public UIDocument documentoUI;
    private VisualElement healthbar_fill; 
    private Animator animator;

    void Start()
    {
        saludActual = saludMaxima;
        animator = GetComponent<Animator>();
    }

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
        
        if (saludActual < 0) 
        {
            saludActual = 0;
        }

        ActualizarBarraVida();

        if (animator != null)
        {
            animator.SetTrigger("Hurt"); 
        }

        EventosAtaqueRC eventosAtaque = GetComponent<EventosAtaqueRC>();
        if (eventosAtaque != null)
        {
            eventosAtaque.DesactivarHitbox();
        }

        if (saludActual <= 0)
        {
            Morir();
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
        animator.SetTrigger("Death");
        Destroy(gameObject,0.5f);
        
        Destroy(gameObject, 1.5f);
    }

    public void Curar(int cantidad)
{
    saludActual += cantidad;
    if(saludActual > saludMaxima)
    {
        saludActual = saludMaxima; 
    }
    Debug.Log("¡El personaje se curó! Salud actual: " + saludActual);
}
}