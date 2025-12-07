using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public GameObject groundCheck;
    public LayerMask groundLayer;

    [Header("Attack Settings")]
    public Transform attackPoint;
    public LayerMask enemyLayers;

    [Header("Camera")]
    public Camera mainCamera;

    public bool isFaceRight = true;
    private bool isGrounded;

    private Rigidbody2D rb;
    private Animator anim;

    private InputController inputController;

    void Start()
    {
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

    void UpdateCameraPosition()
    {
        if (mainCamera != null)
        {
            Vector3 cameraPosition = mainCamera.transform.position;
            cameraPosition.x = transform.position.x;
            mainCamera.transform.position = cameraPosition;
        }
    }

    void HandleMovement()
    {
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
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
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
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);
        }
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
            if (inputController != null && inputController.Attack)
            {
                PerformAttack();
                nextAttack = Time.time + 1f / attackRate;
            }
        }
    }

    public void PerformAttack()
    {
        anim.SetTrigger("isAttack1");

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

    void HandleJump()
    {
        if (isGrounded && inputController != null)
        {
            if (inputController.Jump)
            {
                Debug.Log("Zıpladı");
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
        }
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackDistance);
        }
    }
}