using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float projectileSpeed = 5;
    public float projectileDamage = 0.01f;
    public float lifetime = 3f;
    
    private Rigidbody2D rb;
    private float direction;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        Destroy(gameObject, lifetime);
    }

    public void Initialize(float dir)
    {
        direction = dir;
        
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * projectileSpeed, 0f);
        }

        if (direction < 0)
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController playerController = collision.GetComponent<PlayerController>();
            
            if (playerController != null)
            {
                playerController.TakeDamage(projectileDamage, transform.position);
            }

            Destroy(gameObject);
        }
        else if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}