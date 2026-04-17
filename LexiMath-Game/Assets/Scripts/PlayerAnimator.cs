using UnityEngine;

/// <summary>
/// PlayerAnimator — LexiMath
/// 
/// Lee el estado de PlayerMovement y actualiza los parámetros del Animator.
/// 
/// SETUP EN UNITY:
///   1. Selecciona el Knight en Hierarchy
///   2. Add Component → PlayerAnimator
///   (Automáticamente detecta PlayerMovement y Animator del mismo GameObject)
/// </summary>
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private PlayerMovement _movimiento;
    private Animator       _anim;

    void Awake()
    {
        _movimiento = GetComponent<PlayerMovement>();
        _anim       = GetComponent<Animator>();
    }

    void Update()
    {
        // Velocidad horizontal → controla Idle vs Walk
        _anim.SetFloat("Speed", Mathf.Abs(_movimiento.VelocidadH));

        // En suelo → controla Jump
        _anim.SetBool("IsGrounded", _movimiento.EstaEnSuelo);
    }

    /// <summary>
    /// Llama esto desde PlayerAttack cuando el jugador presiona X.
    /// </summary>
    public void TriggerAtaque()
    {
        _anim.SetTrigger("Attack");
    }
}
