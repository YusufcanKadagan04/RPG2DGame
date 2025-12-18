using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Temel İstatistikler")]
    public float enemyHealth = 100f;
    public float enemySpeed = 2f;
    public float enemyDamage = 10f;
    private float currentHealth;

    [Header("Devriye Ayarları")]
    public float patrolDistance = 3f;
    private float walkDirection = 1f;
    private Vector2 startPosition;
    private float distanceWalked = 0f;

    [Header("Algılama Ayarları")]
    public float detectionRange = 10f;
    public float meleeRange = 1.5f;

    [Header("Yakın Saldırı Ayarları")]
    public float meleeCooldown = 1f;
    public string meleeAttackTrigger = "IsAttack";
    private float nextMeleeTime = 0f;
    public float enemyAttackPoint = 25;
    public Transform attackPoint;
    public float attackDistance = 1f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;
    public GameObject GroundCheck;
    public LayerMask playerLayer;

    [Header("Knockback Ayarları")]
    public float knockbackDuration = 0.3f;
    public float knockbackForce = 10f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool isDead = false;

    private enum EnemyState { Patrol, Chase, MeleeAttack, Dead }
    private EnemyState currentState = EnemyState.Patrol;

    void Start()
    {
        currentHealth = enemyHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        FindPlayer();
        startPosition = transform.position;
        ValidateSettings();
    }

    void Update()
    {
        if (isDead || currentState == EnemyState.Dead)
        {
            StopMovement();
            return;
        }

        if (isKnockedBack)
        {
            HandleKnockback();
            return;
        }

        if (playerTransform == null)
        {
            FindPlayer();
            if (playerTransform == null)
            {
                Patrol();
                return;
            }
        }

        DetermineState();
        ExecuteState();
    }

    void DetermineState()
    {
        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist <= meleeRange)
        {
            currentState = EnemyState.MeleeAttack;
        }
        else if (dist <= detectionRange)
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Patrol;
        }
    }

    void ExecuteState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.MeleeAttack:
                PerformMeleeAttack();
                break;
        }
    }

    void Patrol()
    {
        if (distanceWalked >= patrolDistance)
        {
            Flip();
            walkDirection *= -1f;
            distanceWalked = 0f;
        }

        Move(walkDirection);
        distanceWalked += enemySpeed * Time.deltaTime;
        SetWalkAnimation(true);
    }

    void Chase()
    {
        FacePlayer();
        Move(walkDirection);
        SetWalkAnimation(true);
    }

    void Move(float direction)
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(direction * enemySpeed, rb.linearVelocity.y);
    }

    void StopMovement()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        SetWalkAnimation(false);
    }

    void FacePlayer()
    {
        if (playerTransform == null) return;

        float dir = playerTransform.position.x - transform.position.x;
        float target = dir > 0 ? 1f : -1f;

        if (Mathf.Sign(transform.localScale.x) != target)
            Flip();

        walkDirection = target;
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void PerformMeleeAttack()
    {
        FacePlayer();
        StopMovement();

        if (Time.time >= nextMeleeTime)
        {
            if (anim != null)
            {
                anim.ResetTrigger(meleeAttackTrigger);
                anim.SetTrigger(meleeAttackTrigger);
            }
            nextMeleeTime = Time.time + meleeCooldown;
        }
    }

    // Animation Event'te çağrılacak
    public void DealMeleeDamage()
    {
        if (playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);
        if (dist <= meleeRange * 1.5f)
        {
            PlayerController ps = playerTransform.GetComponent<PlayerController>();
            if (ps != null) ps.TakeDamage(enemyDamage);
        }
    }

    public void TakeDamage(float damage = 25f)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (anim != null)
        {
            anim.ResetTrigger("IsHurt");
            anim.SetTrigger("IsHurt");
        }

        ApplyKnockback();

        if (currentHealth <= 0)
            Die();
    }

    void ApplyKnockback()
    {
        float dir = -Mathf.Sign(transform.localScale.x);
        if (playerTransform != null)
            dir = playerTransform.position.x < transform.position.x ? 1f : -1f;

        if (rb != null)
            rb.linearVelocity = new Vector2(dir * knockbackForce, rb.linearVelocity.y);

        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
    }

    void HandleKnockback()
    {
        knockbackTimer -= Time.deltaTime;
        if (knockbackTimer <= 0)
        {
            isKnockedBack = false;
            StopMovement();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        currentState = EnemyState.Dead;

        if (anim != null)
            anim.SetTrigger("IsDead");

        StopMovement();

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 2f);
    }

    void SetWalkAnimation(bool isWalking)
    {
        if (anim != null)
            anim.SetFloat("IsRun", isWalking ? 1f : 0f);
    }

    void FindPlayer()
    {
        GameObject obj = GameObject.FindWithTag("Player");
        if (obj != null)
            playerTransform = obj.transform;
    }

    void ValidateSettings()
    {
        if (detectionRange <= meleeRange)
            detectionRange = meleeRange + 4f;
    }

    private void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Ground check
        if (GroundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(GroundCheck.transform.position, groundCheckRadius);
        }

        // Attack point
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackDistance);
        }

        // Melee range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}