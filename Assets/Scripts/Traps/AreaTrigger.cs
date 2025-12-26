using UnityEngine;

public class AreaTrigger : MonoBehaviour
{
    public TrapManager trapManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            trapManager.ActivateTraps();
            Destroy(gameObject); 
        }
    }
}