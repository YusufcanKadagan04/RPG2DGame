using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    float nextAttack=0f;
    float movementDrection;

    public bool isMoving = false;

    public float speed =3;
    public float jumpPower=5;
    public float groundCheckRadius;
    public float attackRate=2f;
    public float attackDistance;
    public float attackDamage = 25f;

    public Camera mainCamera;

    public bool isfaceRight = true;
    bool isGrounded;

    Rigidbody2D rb;
    Animator anim;

    RaycastHit2D hit;

    public GameObject groundCheck;
    public LayerMask groundLayer;
    public LayerMask enemyLayers;
    public Transform attackPoint;

    
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference attackAction;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        checkroatation();
        jump();
        checkSurface();
        checkAnimation();
        cameraMovement();
        
        if (Time.time >= nextAttack)
        {
            if (attackAction.action != null)
            {
                if (attackAction.action.triggered)
                {
                    attack();
                    nextAttack = Time.time + 1f / attackRate;
                }
            }
        }
    }
    void FixedUpdate()
    {
      Movement();

    }
    void cameraMovement()
    {
        Vector3 cameraPosition = mainCamera.transform.position;
        cameraPosition.x = transform.position.x;
        mainCamera.transform.position = cameraPosition;
    }
    void Movement()
    {
        isMoving = movementDrection != 0;

        if (moveAction != null && moveAction.action != null)
        {
            movementDrection = moveAction.action.ReadValue<Vector2>().x;
        }
        else
        {
            movementDrection = Keyboard.current != null && Keyboard.current.dKey.isPressed ? 1f : (Keyboard.current != null && Keyboard.current.aKey.isPressed ? -1f : 0f);
        }
        rb.linearVelocity = new Vector2(movementDrection * speed, rb.linearVelocity.y);
        anim.SetFloat("walk",Mathf.Abs(movementDrection * speed));
        
    }

    void checkAnimation()
    {
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
    }
    void checkroatation()
    {
        if (isfaceRight &&  movementDrection < 0)
        {
            Flip();
        }
        else if (!isfaceRight && movementDrection > 0)
        {
            Flip();
        }
    }

    void checkSurface()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);
    }
    void Flip()
    {
        isfaceRight = !isfaceRight;
        Vector3 Thescale = transform.localScale;
        Thescale.x *= -1;
        transform.localScale = Thescale;
    }
    public void attack()
    {
        float numb = Random.Range(0,2);  //Burayı sonra değiştireceğim attack2 yi daha güçlü bir saldırı seçeneği olarak yapabilirim.Şuanlık rastgele yaptım.
        if (numb == 0)
            anim.SetTrigger("attack1");
        else if (numb == 1)
        {
            anim.SetTrigger("attack2");
        }

        RaycastHit2D[] hitEnemies = Physics2D.CircleCastAll(attackPoint.position, attackDistance, Vector2.zero, 0.5f, enemyLayers);
        foreach (RaycastHit2D enemy in hitEnemies)
        {
            EnemyStats enemyStats = enemy.transform.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                Debug.Log("Hit " + enemy.transform.name);
                enemyStats.TakeDamage(attackDamage);
            }
        }
    } 
    void jump()
    {
        if (isGrounded)
        {
            if (jumpAction.action != null)
        {
            if (jumpAction.action.triggered)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            }
        }
        else if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
        }
        }
    }

    void OnEnable()
    {
        if (moveAction != null && moveAction.action != null) moveAction.action.Enable();
        if (jumpAction != null && jumpAction.action != null) jumpAction.action.Enable();
    }

    void OnDisable()
    {
        if (moveAction != null && moveAction.action != null) moveAction.action.Disable();
        if (jumpAction != null && jumpAction.action != null) jumpAction.action.Disable();
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.transform.position, groundCheckRadius);
        Gizmos.DrawWireSphere(attackPoint.position, attackDistance);
    }
}