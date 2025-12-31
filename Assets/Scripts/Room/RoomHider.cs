using UnityEngine;
using System.Collections;

public class RoomHider : MonoBehaviour
{
    public SpriteRenderer darknessOverlay;
    public GameObject enemiesParent;
    public float fadeSpeed = 5f;

    private void Start()
    {
        if (darknessOverlay != null)
        {
            Color c = darknessOverlay.color;
            c.a = 1f;
            darknessOverlay.color = c;
        }

        if (enemiesParent != null) enemiesParent.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoom(0f));
            
            if (enemiesParent != null) enemiesParent.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(FadeRoom(1f));
            
            if (enemiesParent != null) enemiesParent.SetActive(false);
        }
    }

    IEnumerator FadeRoom(float targetAlpha)
    {
        if (darknessOverlay == null) yield break;

        Color currentColor = darknessOverlay.color;

        while (Mathf.Abs(currentColor.a - targetAlpha) > 0.01f)
        {
            currentColor.a = Mathf.MoveTowards(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
            darknessOverlay.color = currentColor;
            yield return null;
        }

        currentColor.a = targetAlpha;
        darknessOverlay.color = currentColor;
    }
}