using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemigoAzulIA : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float speed = 2f;

    [Header("Patrulla sin puntos (Opción 2)")]
    [SerializeField] private float patrolDistance = 2.5f; // desde donde nació
    [SerializeField] private bool patrolWhenNoPlayer = true;

    [Header("Detección del jugador")]
    [SerializeField] private Transform player;
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float attackRange = 1.1f;
    [SerializeField] private float chaseRange = 3f;

    [Header("Ataque")]
    [SerializeField] private float attackCooldown = 1.0f;

    [Header("Flip / Orientación")]
    [Tooltip("Actívalo si tu sprite mira a la derecha por defecto. Si mira a la izquierda, desactívalo.")]
    [SerializeField] private bool spriteMiraDerechaPorDefecto = true;

    [Header("Animator (nombres deben coincidir con tu Animator)")]
    [SerializeField] private string walkBoolName = "isWalking";
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private string attackStateName = "Attack_1";
    [SerializeField] private string hurtStateName = "Hurt";
    [SerializeField] private string dieStateName  = "Die";

    private Animator anim;
    private Rigidbody2D rb;

    private float cooldownTimer;
    private int direction = 1; // 1 derecha, -1 izquierda
    private bool isDead;

    private float startX;
    private Vector3 baseScale;

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        startX = transform.position.x;
        baseScale = transform.localScale;

        rb.freezeRotation = true;
    }

    void Update()
    {
        if (isDead) return;

        cooldownTimer -= Time.deltaTime;

        // Buscar player por tag si no está asignado
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }

        // Si no hay player, patrullar o quedarse quieto
        if (player == null)
        {
            if (patrolWhenNoPlayer) PatrolByDistance();
            else StopMoving();
            return;
        }

        // ✅ Distancia horizontal (mejor en plataformas)
        float distX = Mathf.Abs(transform.position.x - player.position.x);
        bool playerInChase  = distX <= chaseRange;
        bool playerInAttack = distX <= attackRange;

        // Si está atacando/herido/muriendo: NO se mueve
        if (IsInState(attackStateName) || IsInState(hurtStateName) || IsInState(dieStateName))
        {
            StopMoving();
            return;
        }

        // Atacar si está en rango y cooldown listo
        if (playerInAttack && cooldownTimer <= 0f)
        {
            FacePlayer();
            anim.SetTrigger(attackTriggerName);
            cooldownTimer = attackCooldown;

            StopMoving();
            return;
        }

        // Perseguir si está cerca
        if (playerInChase)
        {
            FacePlayer();
            Move(direction);
            return;
        }

        // Si no está cerca: patrullar
        if (patrolWhenNoPlayer) PatrolByDistance();
        else StopMoving();
    }

    private void Move(int dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        anim.SetBool(walkBoolName, true);
        ApplyFlip(dir);
    }

    private void StopMoving()
    {
        anim.SetBool(walkBoolName, false);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void PatrolByDistance()
    {
        float leftLimit  = startX - patrolDistance;
        float rightLimit = startX + patrolDistance;

        if (transform.position.x <= leftLimit) direction = 1;
        else if (transform.position.x >= rightLimit) direction = -1;

        Move(direction);
    }

    private void FacePlayer()
    {
        if (player == null) return;

        direction = (player.position.x >= transform.position.x) ? 1 : -1;
        ApplyFlip(direction);
    }

    private void ApplyFlip(int dir)
    {
        // Mantener escala original (por si tu prefab no es 1,1,1)
        float x = Mathf.Abs(baseScale.x);

        // Si mira a la derecha por defecto: dir=1 => +x, dir=-1 => -x
        // Si mira a la izquierda por defecto, se invierte.
        float sign = spriteMiraDerechaPorDefecto ? dir : -dir;

        transform.localScale = new Vector3(x * sign, baseScale.y, baseScale.z);
    }

    private bool IsInState(string stateName)
    {
        if (string.IsNullOrEmpty(stateName)) return false;
        return anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

    public void SetDead()
    {
        isDead = true;
        anim.SetBool(walkBoolName, false);
        rb.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        // Rango chase (línea amarilla)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(
            new Vector3(transform.position.x - chaseRange, transform.position.y, 0),
            new Vector3(transform.position.x + chaseRange, transform.position.y, 0)
        );

        // Rango attack (línea roja)
        Gizmos.color = Color.red;
        Gizmos.DrawLine(
            new Vector3(transform.position.x - attackRange, transform.position.y + 0.1f, 0),
            new Vector3(transform.position.x + attackRange, transform.position.y + 0.1f, 0)
        );

        // Patrulla (línea cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            new Vector3(transform.position.x - patrolDistance, transform.position.y - 0.2f, 0),
            new Vector3(transform.position.x + patrolDistance, transform.position.y - 0.2f, 0)
        );
    }
}