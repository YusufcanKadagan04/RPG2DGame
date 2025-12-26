using UnityEngine;

public class PatrolSaw : MonoBehaviour
{
    [Header("Ayarlar")]
    public float donmeHizi = 300f;
    public float gitmeHizi = 3f;

    [Header("Duraklar Listesi")]
    public Transform[] duraklar; 

    private int suankiDurakSirasi = 0;
    private Transform hedefDurak;

    void Start()
    {
        if (duraklar.Length > 0)
        {
            transform.position = duraklar[0].position;
            HedefiGuncelle();
        }
    }

    void Update()
    {
        if (duraklar.Length == 0) return;

        transform.Rotate(0, 0, donmeHizi * Time.deltaTime);

        transform.position = Vector3.MoveTowards(transform.position, hedefDurak.position, gitmeHizi * Time.deltaTime);

        if (Vector3.Distance(transform.position, hedefDurak.position) < 0.1f)
        {
            HedefiGuncelle();
        }
    }

    void HedefiGuncelle()
    {
        suankiDurakSirasi++;

        if (suankiDurakSirasi >= duraklar.Length)
        {
            suankiDurakSirasi = 0;
        }

        hedefDurak = duraklar[suankiDurakSirasi];
    }

    private void OnDrawGizmos()
    {
        if (duraklar == null || duraklar.Length < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < duraklar.Length; i++)
        {
            Vector3 simdiki = duraklar[i].position;
            Vector3 sonraki = duraklar[(i + 1) % duraklar.Length].position;
            Gizmos.DrawLine(simdiki, sonraki);
        }
    }
}