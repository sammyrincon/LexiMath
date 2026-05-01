using UnityEngine;

public class EventosAtaqueRC : MonoBehaviour
{
    public GameObject hitboxAtaque;

    private void Start()
    {
        DesactivarHitbox();
    }

    public void ActivarHitbox()
    {
        if (hitboxAtaque != null)
        {
            hitboxAtaque.SetActive(true);
        }
    }

    public void DesactivarHitbox()
    {
        if (hitboxAtaque != null)
        {
            hitboxAtaque.SetActive(false);
        }
    }
} 