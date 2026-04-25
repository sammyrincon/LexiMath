using UnityEngine;

public class EnemigoAzulCombat : MonoBehaviour
{
    [Header("Hitbox de ataque (GameObject hijo con Collider2D Trigger)")]
    [SerializeField] private GameObject hitbox;

    void Awake()
    {
        ActivarHitboxSiempre();
    }

    void OnEnable()
    {
        ActivarHitboxSiempre();
    }

    private void ActivarHitboxSiempre()
    {
        if (hitbox != null)
            hitbox.SetActive(true);
    }

    public void EnableHitbox()
    {
        if (hitbox != null)
            hitbox.SetActive(true);
    }

    public void DisableHitbox()
    {
        if (hitbox != null)
            hitbox.SetActive(true); 
    }
    public void DisableHitboxForDeath()
    {
        if (hitbox != null)
            hitbox.SetActive(false);
    }
} 

