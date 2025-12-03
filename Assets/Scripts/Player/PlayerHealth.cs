using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public Image healthBar;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        if (maxHealth > 0)
        {
            healthBar.fillAmount = currentHealth / 100;
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentHealth -= collision.gameObject.GetComponent<EnemyStats>().enemyDamage;
            
            if (currentHealth <= 0)
            {
                Destroy(gameObject);
            }
        }
    }   

}
