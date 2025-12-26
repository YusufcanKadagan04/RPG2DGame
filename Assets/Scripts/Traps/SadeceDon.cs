using UnityEngine;

public class SadeceDon : MonoBehaviour
{
    public float donmeHizi = 300f;

    void Update()
    {
        transform.Rotate(0, 0, donmeHizi * Time.deltaTime);
    }
}