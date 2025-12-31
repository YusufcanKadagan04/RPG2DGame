using UnityEngine;
using System.Collections;

public class PlayerOneWayDrop : MonoBehaviour
{
    private GameObject currentPlatform;
    private Collider2D playerCollider; // Türü genel Collider2D yaptık (Box, Capsule fark etmez)

    void Start()
    {
        // Player'ın üzerindeki collider'ı otomatik bul
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (currentPlatform != null)
            {
                StartCoroutine(DisableCollision());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWay"))
        {
            currentPlatform = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("OneWay"))
        {
            currentPlatform = null;
        }
    }

    IEnumerator DisableCollision()
    {
        Collider2D platformCollider = currentPlatform.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);

        yield return new WaitForSeconds(0.5f);

        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
    }
}