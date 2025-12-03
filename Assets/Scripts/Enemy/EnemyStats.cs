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
        player = GameObject.FindWithTag("Player");
        startPosition = transform.position;
    }   
    void Update()
    {
        // Player'a olan mesafeyi kontrol et
        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // Eğer Player yakınsa takip et
            if (distanceToPlayer < attackDistance)
            {
                ChasePlayer();
            }
            else
            {
                Walk(); // Yoksa normal yürüyüş yap
            }
        }
        else
        {
            Walk();
        }
    }

    void ChasePlayer()
    {
        // Player'ın yönünü belirle
        float directionToPlayer = player.transform.position.x - transform.position.x;

        // Eğer Player sağda ise sağa, solda ise sola git
        if (directionToPlayer > 0)
        {
            walkDirection = 1f; // Sağa
        }
        else
        {
            walkDirection = -1f; // Sola
        }

        // Yüz yönü kontrol et
        if ((walkDirection > 0 && transform.localScale.x < 0) || (walkDirection < 0 && transform.localScale.x > 0))
        {
            flip();
        }

        // Player'ı takip et
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
    public void TakeDamage(float damage = 25f)
    {
        currentHealth -= damage;
        anim.SetTrigger("enemyth");
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
