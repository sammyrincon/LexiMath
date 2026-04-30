using UnityEngine;

public class EventosAtaqueRC : MonoBehaviour
{
    public ArmaEnemigoRC hitboxArma;

    void Start()
    {
        DesactivarHitbox();
    }

    public void ActivarHitbox()
    {
        if (hitboxArma != null)
        {
            hitboxArma.ActivarHitbox();
        }
    }

    public void DesactivarHitbox()
    {
        if (hitboxArma != null)
        {
            hitboxArma.ApagarHitbox();
        }
    }
}