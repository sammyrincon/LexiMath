using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Vida")]
    [SerializeField] private int maxHP = 5;

    [Header("Invencibilidad (I-Frames)")]
    [SerializeField] private float invincibilityTime = 0.6f;
    [SerializeField] private bool flashSprite = true;
    [SerializeField] private float flashInterval = 0.08f;

    [Header("Hurt Lock (bloquea control un momento)")]
    [SerializeField] private float hurtLockTime = 0.25f;

    [Header("Muerte")]
    [SerializeField] private string deathStateName = "Die";
    [SerializeField] private float deathAnimationFallbackTime = 0.8f;
    [SerializeField] private bool showRetryButton = true;
    [SerializeField] private KeyCode retryKey = KeyCode.R;
    [SerializeField] private string retryButtonText = "Reintentar";

    [Header("Knockback (empuje al recibir daño)")]
    [SerializeField] private bool useKnockback = true;
    [SerializeField] private float knockbackX = 7f;
    [SerializeField] private float knockbackY = 6f;

    [Header("Referencias (opcional)")]
    [SerializeField] private MonoBehaviour controllerToDisable; 
    [SerializeField] private Collider2D bodyCollider;          
    [SerializeField] private SpriteRenderer spriteRenderer;  
    private int hp;
    private bool invincible;
    private bool isDead;
    private bool isReloading;

    private Animator anim;
    private Rigidbody2D rb;
    private float originalGravityScale;

    public bool IsDead => isDead;
    public int HP => hp;

    void Awake()
    {
        hp = maxHP;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;

        if (controllerToDisable == null)
        {
            controllerToDisable = GetComponent<KnightController>();

            if (controllerToDisable == null)
            {
                controllerToDisable = GetComponent<PlayerController>();
            }
        }

        if (bodyCollider == null) bodyCollider = GetComponent<Collider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int amount, Vector2 hitPoint, Vector2 hitDirection)
    {
        if (isDead) return;
        if (invincible) return;

        hp -= amount;

        anim.ResetTrigger("Attack"); // opcional
        anim.SetTrigger("Hurt");

        if (useKnockback)
        {
            Vector2 dir = hitDirection.normalized;
            rb.linearVelocity = new Vector2(dir.x * knockbackX, knockbackY);
        }

        StartCoroutine(InvincibilityRoutine());
        StartCoroutine(HurtLockRoutine());

        // Muerte
        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StopAllCoroutines();
        invincible = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;

        anim.ResetTrigger("Hurt");
        anim.ResetTrigger("Attack");
        anim.SetTrigger("Die");

        if (controllerToDisable != null) controllerToDisable.enabled = false;

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        rb.gravityScale = originalGravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        StartCoroutine(FreezeDeathPoseRoutine());
    }

    private IEnumerator FreezeDeathPoseRoutine()
    {
        float waitForState = 0f;
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        while (!stateInfo.IsName(deathStateName) && waitForState < 0.2f)
        {
            yield return null;
            waitForState += Time.deltaTime;
            stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        }

        float clipLength = deathAnimationFallbackTime;

        if (stateInfo.IsName(deathStateName) && stateInfo.length > 0f)
        {
            clipLength = stateInfo.length;
        }

        yield return new WaitForSeconds(clipLength);

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        anim.Play(deathStateName, 0, 0.999f);
        anim.Update(0f);
        anim.speed = 0f;
    }

    private void Update()
    {
        if (!isDead || isReloading || !showRetryButton)
        {
            return;
        }

        if (Input.GetKeyDown(retryKey))
        {
            RetryScene();
        }
    }

    private void OnGUI()
    {
        if (!isDead || isReloading || !showRetryButton)
        {
            return;
        }

        const float labelWidth = 260f;
        const float labelHeight = 30f;
        const float buttonWidth = 220f;
        const float buttonHeight = 44f;

        Rect labelRect = new Rect(
            (Screen.width - labelWidth) * 0.5f,
            Screen.height - 110f,
            labelWidth,
            labelHeight);

        Rect buttonRect = new Rect(
            (Screen.width - buttonWidth) * 0.5f,
            Screen.height - 70f,
            buttonWidth,
            buttonHeight);

        GUI.Label(labelRect, $"Presiona {retryKey} para reiniciar");

        if (GUI.Button(buttonRect, $"{retryButtonText} ({retryKey})"))
        {
            RetryScene();
        }
    }

    private void RetryScene()
    {
        if (isReloading)
        {
            return;
        }

        isReloading = true;
        anim.speed = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private IEnumerator HurtLockRoutine()
    {
        if (controllerToDisable != null) controllerToDisable.enabled = false;
        yield return new WaitForSeconds(hurtLockTime);
        if (!isDead && controllerToDisable != null) controllerToDisable.enabled = true;
    }

    private IEnumerator InvincibilityRoutine()
    {
        invincible = true;

        if (flashSprite && spriteRenderer != null)
        {
            float t = 0f;
            while (t < invincibilityTime)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(flashInterval);
                t += flashInterval;
            }
            spriteRenderer.enabled = true;
        }
        else
        {
            yield return new WaitForSeconds(invincibilityTime);
        }

        invincible = false;
    }
}
