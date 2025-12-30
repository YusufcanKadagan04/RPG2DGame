using UnityEngine;

public class BossStats : MonoBehaviour
{
    [Header("Boss Stats")]
    public float bossHealth = 300f;
    public float bossSpeed = 3.5f;
    public float bossDamage = 25f;
    public int attackAnimationCount = 2;

    [Header("Detection")]
    public float detectionRangeX = 12f;
    public float detectionRangeY = 2.5f;
    public float meleeRange = 2f;

    [Header("Patrol")]
    public float patrolDistance = 6f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Attack")]
    public float attackRate = 1.2f;
    public Transform attackPoint;
    public float attackRadius = 1.2f;
    public LayerMask playerLayer;

    [Header("Effects")]
    public GameObject bloodEffectPrefab;
    public Transform bloodSpawnPoint;

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
        currentHealth = bossHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
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
            if (diffX <= meleeRange) AttackPlayer();
            else ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, 1.5f, groundLayer);

        if (!isGroundAhead || distanceWalked >= patrolDistance)
        {
            Flip();
            walkDirection *= -1f;
            distanceWalked = 0f;
        }

        rb.linearVelocity = new Vector2(walkDirection * bossSpeed, rb.linearVelocity.y);
        distanceWalked += bossSpeed * Time.deltaTime;
        anim.SetFloat("IsRun", 1f);
    }

    void ChasePlayer()
    {
        FacePlayer();
        bool isGroundAhead = Physics2D.Raycast(groundCheck.position, Vector2.down, 1.5f, groundLayer);
        
        if (isGroundAhead)
        {
            rb.linearVelocity = new Vector2(walkDirection * bossSpeed, rb.linearVelocity.y);
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
            int randomAttack = Random.Range(1, attackAnimationCount + 1); 
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
            hitPlayer.GetComponent<PlayerController>()?.TakeDamage(bossDamage);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (bloodEffectPrefab != null)
        {
            Instantiate(bloodEffectPrefab, bloodSpawnPoint != null ? bloodSpawnPoint.position : transform.position, Quaternion.identity);
        }

        anim.SetTrigger("IsHurt");
        StopMoving(); 

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("IsDead", true);
        StopMoving();
        rb.bodyType = RigidbodyType2D.Static;
        Destroy(gameObject, 5f);
    }

    void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetFloat("IsRun", 0f);
    }

    void FacePlayer()
    {
        if (playerTransform.position.x > transform.position.x && transform.localScale.x < 0) Flip();
        else if (playerTransform.position.x < transform.position.x && transform.localScale.x > 0) Flip();
    }

    void Flip()
    {
        walkDirection = (transform.localScale.x > 0) ? -1 : 1;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector3(detectionRangeX * 2, detectionRangeY * 2, 0));
    }
}