using UnityEngine;
using System.Collections;

public class EnemyStats : MonoBehaviour
{
    [Header("Stats")]
    public float enemyHealth = 100f;
    public float enemySpeed = 2f;
    public float enemyDamage = 10f;

    [Header("Detection")]
    public float detectionRangeX = 8f;
    public float detectionRangeY = 1.5f;
    public float meleeRange = 1.2f;

    [Header("Patrol & Obstacles")]
    public float patrolDistance = 4f;
    public Transform groundCheck;
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public LayerMask groundLayer;

    [Header("Attack")]
    public float attackRate = 1.5f;
    public Transform attackPoint;
    public float attackRadius = 0.8f;
    public LayerMask playerLayer;

    [Header("Effects")]
    public GameObject bloodEffectPrefab;
    public Transform bloodSpawnPoint;

    [Header("Defense & Physics")]
    [Range(0f, 1f)] public float defendChance = 0.2f;
    public float knockbackForce = 5f;

    private float currentHealth;
    private float nextAttackTime = 0f;
    private float walkDirection = 1f;
    private float distanceWalked = 0f;
    private bool isDead = false;
    private bool isKnockedBack = false;
    private Transform playerTransform;
    private Animator anim;
    private Rigidbody2D rb;

    void Start()
    {
        currentHealth = enemyHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        walkDirection = (transform.localScale.x > 0) ? 1f : -1f;
    }

    void Update()
    {
        if (isDead || isKnockedBack) return;

        if (playerTransform == null)
        {
            Patrol();
            return;
        }

        float diffX = Mathf.Abs(transform.position.x - playerTransform.position.x);
        float diffY = Mathf.Abs(transform.position.y - playerTransform.position.y);

        if (diffY <= detectionRangeY && diffX <= detectionRangeX)
        {
            if (CanSeePlayer())
            {
                if (diffX <= meleeRange) AttackPlayer();
                else ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }
        else
        {
            Patrol();
        }
    }

    bool CanSeePlayer()
    {
        Vector2 start = transform.position; 
        Vector2 end = playerTransform.position;
        RaycastHit2D hit = Physics2D.Linecast(start, end, groundLayer);
        return hit.collider == null;
    }

    void Patrol()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        bool isWallAhead = Physics2D.Raycast(wallCheck.position, Vector2.right * walkDirection, wallCheckDistance, groundLayer);

        if (!isGroundAhead || isWallAhead || distanceWalked >= patrolDistance)
        {
            Flip();
            distanceWalked = 0f;
        }

        rb.linearVelocity = new Vector2(walkDirection * enemySpeed, rb.linearVelocity.y);
        distanceWalked += enemySpeed * Time.deltaTime;
        anim.SetFloat("IsRun", 1f);
    }

    void ChasePlayer()
    {
        FacePlayer();
        
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, 1f, groundLayer);
        bool isWallAhead = Physics2D.Raycast(wallCheck.position, Vector2.right * walkDirection, wallCheckDistance, groundLayer);
        
        if (isGroundAhead && !isWallAhead)
        {
            rb.linearVelocity = new Vector2(walkDirection * enemySpeed, rb.linearVelocity.y);
            anim.SetFloat("IsRun", 1f);
        }
        else
        {
            StopMoving();
        }
    }

    void AttackPlayer()
    {
        StopMoving();
        FacePlayer();

        if (Time.time >= nextAttackTime)
        {
            int randomAttack = Random.Range(1, 4); 
            anim.SetInteger("AttackID", randomAttack);
            anim.SetTrigger("Attack");
            nextAttackTime = Time.time + attackRate;
        }
    }

    public void DealMeleeDamage()
    {
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRadius, playerLayer);
        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerController>()?.TakeDamage(enemyDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        if (Random.value < defendChance)
        {
            anim.SetTrigger("IsDefend");
            return;
        }

        currentHealth -= damage;

        if (bloodEffectPrefab != null)
            Instantiate(bloodEffectPrefab, bloodSpawnPoint != null ? bloodSpawnPoint.position : transform.position, Quaternion.identity);

        anim.SetTrigger("IsHurt");
        StartCoroutine(KnockbackRoutine());

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("IsDead", true);
        StopMoving();
        rb.bodyType = RigidbodyType2D.Static;
        Destroy(gameObject, 3f);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        anim.SetFloat("IsRun", 0f);
    }

    void FacePlayer()
    {
        if (playerTransform.position.x > transform.position.x && transform.localScale.x < 0) Flip();
        else if (playerTransform.position.x < transform.position.x && transform.localScale.x > 0) Flip();
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        
        walkDirection = (scale.x > 0) ? 1f : -1f;
    }
    
    IEnumerator KnockbackRoutine()
    {
        isKnockedBack = true;
        float dir = (transform.localScale.x > 0) ? -1 : 1;
        rb.linearVelocity = new Vector2(dir * knockbackForce, rb.linearVelocity.y);
        yield return new WaitForSeconds(0.2f);
        isKnockedBack = false;
        rb.linearVelocity = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = transform.position;
        Vector3 leftBound = center + Vector3.left * patrolDistance;
        Vector3 rightBound = center + Vector3.right * patrolDistance;
        
        Gizmos.DrawLine(leftBound, rightBound);
        Gizmos.DrawWireSphere(leftBound, 0.2f);
        Gizmos.DrawWireSphere(rightBound, 0.2f);

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
        
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down);
        }

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * walkDirection * wallCheckDistance);
        }
    }
}