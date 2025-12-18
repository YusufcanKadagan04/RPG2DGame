using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float nextAttack = 0f;
    private float movementDirection;

    public bool isMoving = false;

    [Header("Movement Settings")]
    public float walkSpeed = 1f;
    public float runSpeed = 3f;
    public float currentSpeed;
    public float jumpPower = 5f;

    [Header("Combat Settings")]
    public float attackRate = 2f;
    public float attackDistance = 1f;
    public float attackDamage = 25f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.2f;
    public GameObject GroundCheck;
    public LayerMask groundLayer;

    [Header("Attack Settings")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Health Settings")]
    public float maxHealth = 10000f;
    public float currentHealth;

    [Header("Damage Settings")]
    public float invincibilityTime = 1f;
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;

    [Header("Knockback Settings")]
    public float knockbackForce = 10f;
    public float knockbackDuration = 0.2f;
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    public bool isFaceRight = true;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator anim;
    private InputController inputController;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inputController = FindObjectOfType<InputController>();


        if (inputController == null)
        {
            Debug.LogError("InputController bulunamadı! Lütfen sahnede InputController olduğundan emin olun.");
        }
    }

    void Update()
    {
        UpdateInvincibility();
        UpdateKnockback();
        CheckRotation();
        HandleJump();
        CheckSurface();
        UpdateAnimation();
        UpdateCameraPosition();
        HandleAttack();

    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void UpdateInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    void UpdateKnockback()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            }
        }
    }

    void UpdateCameraPosition()
    {
        if (mainCamera != null)
        {
            Vector3 cameraPosition = mainCamera.transform.position;
            cameraPosition.x = transform.position.x;
            cameraPosition.y = transform.position.y;
            mainCamera.transform.position = cameraPosition;
        }
    }

    private void SaldiriKontrolu()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackDistance, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(attackDamage);
            }
        }
        

    }
    void HandleMovement()
    {
        if (isKnockedBack)
        {
            return;
        }

        if (inputController != null)
        {
            movementDirection = inputController.Movement.x;

            if (inputController.IsRunning && Mathf.Abs(movementDirection) > 0.01f)
            {
                currentSpeed = runSpeed;  
                anim.SetBool("IsRun", true); 
            }
            else
            {
                currentSpeed = walkSpeed; 
                anim.SetBool("IsRun", false); 
            }
        }
        else
        {
            movementDirection = 0f;
            currentSpeed = walkSpeed;
        }

        isMoving = Mathf.Abs(movementDirection) > 0.01f;

        rb.linearVelocity = new Vector2(movementDirection * currentSpeed, rb.linearVelocity.y);
        
        anim.SetFloat("walk", Mathf.Abs(movementDirection * currentSpeed));
    }

    void UpdateAnimation()
    {
        anim.SetBool("isGrounded", isGrounded);
    }

    void CheckRotation()
    {
        if (isFaceRight && movementDirection < -0.01f)
        {
            Flip();
        }
        else if (!isFaceRight && movementDirection > 0.01f)
        {
            Flip();
        }
    }

    void CheckSurface()
    {
        if (GroundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(GroundCheck.transform.position, groundCheckRadius, groundLayer);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isInvincible)
        {
            return;
        }
        Vector2 damageSource = transform.position + (isFaceRight ? Vector3.right : Vector3.left);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);



        anim.ResetTrigger("IsHurt");
        anim.SetTrigger("IsHurt");

        ApplyKnockback(damageSource);

        isInvincible = true;
        invincibilityTimer = invincibilityTime;

        if (currentHealth <= 0f)
        {
            anim.SetTrigger("IsDead");
            Die();
        }
    }

    void ApplyKnockback(Vector2 damageSourcePosition)
    {
        float knockbackDirection = transform.position.x > damageSourcePosition.x ? 1f : -1f;
        rb.linearVelocity = new Vector2(knockbackDirection * knockbackForce, rb.linearVelocity.y);
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;


    }

    void Die()
    {
        

        Destroy(gameObject, 0.9f);
    }

    void Flip()
    {
        isFaceRight = !isFaceRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void HandleAttack()
    {
        if (Time.time >= nextAttack)
        {
            if (inputController == null) return;


            if (inputController.RandomAttack)
            {
                int randomID = Random.Range(1, 3);
                anim.SetInteger("AttackID", randomID);
                ExecuteAttack();
                nextAttack = Time.time + 1f / attackRate;
            }
            
            else if (inputController.HeavyAttack)
            {
                anim.SetInteger("AttackID", 3);
                ExecuteAttack();
                nextAttack = Time.time + 1f / attackRate;
            }
            
            else if (inputController.Attack)
            {
                ExecuteAttack();
                nextAttack = Time.time + 1f / attackRate;
            }
        }
    }

    private void ExecuteAttack()
    {
        anim.SetTrigger("Attack");

        SaldiriKontrolu();
    }
    void HandleJump()
    {
        if (isGrounded && inputController != null)
        {
            if (inputController.Jump)
            {

                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            }
        }
    }

    private void OnDrawGizmos()
    {
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
    }

    public bool IsKnockedBack()
    {
        return isKnockedBack;
    }
}