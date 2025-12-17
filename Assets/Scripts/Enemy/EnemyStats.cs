using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    public float enemyAttackPoint=25;
    public Transform attackPoint;
    public float attackDistance = 1f;

    /*[Header("Menzilli Saldırı Ayarları")]
    public bool canShoot = true;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float shootRange = 6f;
    public float shootCooldown = 2f;
    public string rangedAttackTrigger = "IsAttack";
    private float nextShootTime = 0f;
    */

    public LayerMask Ground;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public float groundCheckRadius = 0.2f;
    public GameObject GroundCheck;

    [Header("Knockback Ayarları")]
    public float knockbackDuration = 0.3f;
    public float knockbackForce = 10f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool isDead = false;

    private enum EnemyState { Patrol, Chase, RangedAttack, MeleeAttack, Dead }
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
       // ValidateSettings();
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

       // DetermineState();
        ExecuteState();
    }
    public void melleAttack()
    {
        anim.SetTrigger("IsAttack");

        Collider2D[] hitPlayer = Physics2D.OverlapCircleAll(attackPoint.position, attackDistance, playerLayer);

        foreach (Collider2D player in hitPlayer)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
        }
    }

    /*void DetermineState()
    {
        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (dist <= meleeRange)
        {
            currentState = EnemyState.MeleeAttack;
        }
        else if (canShoot && dist <= shootRange && dist > meleeRange)
        {
            currentState = EnemyState.RangedAttack;
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
    */

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
            //case EnemyState.RangedAttack:
                //PerformRangedAttack();
                //break;
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

    /*void PerformRangedAttack()
    {
        FacePlayer();
        StopMovement();

        if (Time.time >= nextShootTime)
        {
            if (anim != null)
            {
                anim.ResetTrigger(rangedAttackTrigger);
                anim.SetTrigger(rangedAttackTrigger);
            }
            SpawnProjectile();
            nextShootTime = Time.time + shootCooldown;
        }
    }
    */

    /*public void SpawnProjectile()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        EnemyProjectile script = projectile.GetComponent<EnemyProjectile>();
        
        if (script != null)
        {
            script.Initialize(Mathf.Sign(transform.localScale.x));
        }
        else
        {
            Rigidbody2D projRb = projectile.GetComponent<Rigidbody2D>();
            if (projRb != null)
                projRb.linearVelocity = new Vector2(Mathf.Sign(transform.localScale.x) * 10f, 0f);
        }
    }
    */

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

    /*void ValidateSettings()
    {
        if (shootRange <= meleeRange)
            shootRange = meleeRange + 4f;

        if (detectionRange <= shootRange)
            detectionRange = shootRange + 4f;
    }
    */

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

         if (GroundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(GroundCheck.transform.position, groundCheckRadius);
        }
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackDistance);
        }

       /* if (canShoot)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, shootRange);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        if (firePoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(firePoint.position, 0.2f);
        }
        */
    }
}