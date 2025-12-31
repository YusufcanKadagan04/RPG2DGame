using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerOneWayDrop : MonoBehaviour
{
    [Header("Input Ayarı")]
    public InputActionReference dropInput; 

    private GameObject currentPlatform;
    private Collider2D playerCollider;

    void Start()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    
    private void OnEnable()
    {
        if (dropInput != null)
        {
            dropInput.action.Enable();
            dropInput.action.performed += OnDropPerformed;
        }
    }

    
    private void OnDisable()
    {
        if (dropInput != null)
        {
            dropInput.action.performed -= OnDropPerformed;
            dropInput.action.Disable();
        }
    }

    
    private void OnDropPerformed(InputAction.CallbackContext context)
    {
        if (currentPlatform != null)
        {
            StartCoroutine(DisableCollision());
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