using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    public WaveSystem waveManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            waveManager.StartBattle();
            gameObject.SetActive(false);
        }
    }
    
}    