using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    float movementDrection;
    public float speed;
    public float jumpPower;
    public float groundCheckRadius;

    bool isfaceRight = true;
    bool isgrounded;

    Rigidbody2D rb;
    Animator anim;

    public GameObject groundCheck;
    public LayerMask groundLayer;
    public InputActionReference moveAction;
    public InputActionReference jumpAction;

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
    }
    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        if (moveAction != null && moveAction.action != null)
        {
            movementDrection = moveAction.action.ReadValue<Vector2>().x;
        }
        else
        {
            movementDrection = Keyboard.current != null && Keyboard.current.dKey.isPressed ? 1f : (Keyboard.current != null && Keyboard.current.aKey.isPressed ? -1f : 0f);
        }
        rb.linearVelocity = new Vector2(movementDrection * speed, rb.linearVelocity.y);
        anim.SetFloat("runSpeed",Mathf.Abs(movementDrection * speed));
    }

    void checkAnimation()
    {
        anim.SetBool("isGrounded", isgrounded);
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
        isgrounded = Physics2D.OverlapCircle(groundCheck.transform.position, groundCheckRadius, groundLayer);
    }
    void Flip()
    {
        isfaceRight = !isfaceRight;
        Vector3 Thescale = transform.localScale;
        Thescale.x *= -1;
        transform.localScale = Thescale;
    }
    void jump()
    {
        if (isgrounded)
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
    }
}