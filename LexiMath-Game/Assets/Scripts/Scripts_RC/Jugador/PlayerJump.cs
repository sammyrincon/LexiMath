using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class PlayerJumpRC : MonoBehaviour
{
    [Header("Configuración")]
    public float jumpForce = 8f;
    public DeteccionSueloRC sueloDetector;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isAttacking = false;
    private bool canInput = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (sueloDetector == null)
            sueloDetector = GetComponentInChildren<DeteccionSueloRC>();

        if (rb == null) Debug.LogError("PlayerJumpRC: falta Rigidbody2D.");
        if (animator == null) Debug.LogError("PlayerJumpRC: falta Animator.");
        if (sueloDetector == null) Debug.LogWarning("PlayerJumpRC: asigna DeteccionSueloRC en el Inspector.");
    }

    void Update()
    {
        if (!canInput || sueloDetector == null) return;

        animator.SetBool("IsGrounded", sueloDetector.tocandoSuelo);

        if ((Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space)) && CanJump())
        {
            DoJump();
        }
    }

    bool CanJump()
    {
        if (animator == null || sueloDetector == null) return false;

        if (animator.IsInTransition(0)) return false;

        AnimatorStateInfo st = animator.GetCurrentAnimatorStateInfo(0);
        if (st.IsName("Attack") || st.IsName("Hurt") || st.IsName("Death")) return false;
        if (isAttacking) return false;

        if (animator.GetFloat("Speed") > 0.1f) return false;

        if (Mathf.Abs(rb.linearVelocity.y) > 0.1f) return false;

        return sueloDetector.tocandoSuelo;
    }

    void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        animator.ResetTrigger("Jump"); 
        animator.SetTrigger("Jump");
        Debug.Log("PlayerJumpRC: Jump disparado");
    }

    public void StartAttack() => isAttacking = true;
    public void EndAttack() => isAttacking = false;
    public void ResetJumpTrigger() => animator.ResetTrigger("Jump");

    public void DisableInput(float seconds)
    {
        if (seconds <= 0f) { canInput = false; return; }
        StartCoroutine(DisableInputCoroutine(seconds));
    }
    private System.Collections.IEnumerator DisableInputCoroutine(float seconds)
    {
        canInput = false;
        yield return new WaitForSeconds(seconds);
        canInput = true;
    }
}
