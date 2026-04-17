using System.Collections;
using UnityEngine;

/// <summary>
/// PlayerAttack — LexiMath
/// Maneja ÚNICAMENTE el ataque con X.
/// 
/// SETUP EN UNITY:
///   1. Add Component → PlayerAttack en el Knight
///   (detecta automáticamente PlayerAnimator del mismo GameObject)
/// </summary>
[RequireComponent(typeof(PlayerAnimator))]
public class PlayerAttack : MonoBehaviour
{
    [Header("Ataque")]
    public float cooldownAtaque = 0.5f;

    // Control externo (TutorialManager lo usa)
    [HideInInspector] public bool puedeAtacar = true;

    private PlayerAnimator _animador;
    private bool _ataqueDisponible = true;

    void Awake()
    {
        _animador = GetComponent<PlayerAnimator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && puedeAtacar && _ataqueDisponible)
            StartCoroutine(EjecutarAtaque());
    }

    private IEnumerator EjecutarAtaque()
    {
        _ataqueDisponible = false;

        // Disparar animación
        _animador.TriggerAtaque();

        yield return new WaitForSeconds(cooldownAtaque);
        _ataqueDisponible = true;
    }
}
