using UnityEngine;
using UnityEngine.UIElements;

public class PlayerHealth : MonoBehaviour
{
    private VisualElement healthFill;
    
    public float maxHealth = 100f;
    private float currentHealth;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        healthFill = root.Q<VisualElement>("healthbar-fill");

        currentHealth = maxHealth;
        ActualizarBarra();
    }

    public void RecibirDano(float cantidad)
    {
        currentHealth -= cantidad;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        ActualizarBarra();

        if (currentHealth <= 0)
        {
            Debug.Log("Has perdido");
        }
    }

    private void ActualizarBarra()
    {
        if (healthFill != null)
        {
            float porcentaje = (currentHealth / maxHealth) * 100;
            healthFill.style.width = new Length(porcentaje, LengthUnit.Percent);
        }
    }
}