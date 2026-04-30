using UnityEngine;

public class EventosAtaqueRC : MonoBehaviour
{
    public GameObject hitboxAtaque;

    private void Start()
    {
        hitboxAtaque.SetActive(false);
    }

    public void ActivarHitbox()
    {
        hitboxAtaque.SetActive(true);
    }

    public void DesactivarHitbox()
    {
        hitboxAtaque.SetActive(false);
    }
}