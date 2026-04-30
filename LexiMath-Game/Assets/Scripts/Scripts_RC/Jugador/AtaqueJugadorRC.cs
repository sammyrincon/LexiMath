using UnityEngine;

public class AtaqueJugador_RC : MonoBehaviour
{
    private Animator animador;

    void Start()
    {
        animador = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animador.SetTrigger("Attack");
        }
    }
}