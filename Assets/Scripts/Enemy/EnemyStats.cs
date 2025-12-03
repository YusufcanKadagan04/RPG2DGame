using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public float enemyHealth=100f;
    public float enemyspeed = 2f;  
    float currentHealth;
    GameObject player;
    
    private float walkDirection = 1f;  
    private float walkDistance = 2f;   
    private Vector2 startPosition;
    private float distanceWalked = 0f;
    public Transform Player;

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;
    public float knockbackDuration = 0.3f;
    public float knockbackForce = 15f;

    Animator anim;
    Rigidbody2D rb;
    RaycastHit2D hit;
    public Transform attackPoint;
    public float attackDistance=3f;

    void Start()
    {
        currentHealth = enemyHealth;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        player = GameObject.FindWithTag("Player");
        startPosition = transform.position;
    }   
    void Update()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.deltaTime;
            if (knockbackTimer <= 0)
            {
                isKnockedBack = false;
                rb.linearVelocity = Vector2.zero; 
            }
            return; 
        }

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < attackDistance)
            {
                ChasePlayer();
            }
            else
            {
                Walk(); 
            }
        }
        else
        {
            Walk();
        }
    }

    void ChasePlayer()
    {
        float directionToPlayer = player.transform.position.x - transform.position.x;

        if (directionToPlayer > 0)
        {
            walkDirection = 1f;
        }
        else
        {
            walkDirection = -1f; 
        }

        if ((walkDirection > 0 && transform.localScale.x < 0) || (walkDirection < 0 && transform.localScale.x > 0))
        {
            flip();
        }

        rb.linearVelocity = new Vector2(walkDirection * enemyspeed, rb.linearVelocity.y);
        anim.SetFloat("enemyWalk", Mathf.Abs(walkDirection));
        
        Debug.Log("Enemy chasing Player");
    }

    void Walk()
    {
        if (distanceWalked >= walkDistance)
        {
            flip();
            walkDirection *= -1f;  
            distanceWalked = 0f;
        }
        rb.linearVelocity = new Vector2(walkDirection * enemyspeed, rb.linearVelocity.y);
        distanceWalked += enemyspeed * Time.deltaTime;
        anim.SetFloat("enemyWalk", Mathf.Abs(walkDirection));
        
    }
    void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void ApplyKnockback(float knockbackDir)
    {
        
        rb.linearVelocity = new Vector2(knockbackDir * knockbackForce, rb.linearVelocity.y);
        isKnockedBack = true;
        knockbackTimer = knockbackDuration;
        
        Debug.Log($"Knockback! Direction: {(knockbackDir > 0 ? "Right" : "Left")}");
    }

    public void TakeDamage(float damage = 25f)
    {
        currentHealth -= damage;
        anim.SetTrigger("enemyth");
        
        
        if (Player.position.x < transform.position.x)
        {
            ApplyKnockback(1f); 
        }
        else
        {
            ApplyKnockback(-1f); 
        }

        Debug.Log($"Damage taken! Health: {currentHealth}/{enemyHealth}");

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
