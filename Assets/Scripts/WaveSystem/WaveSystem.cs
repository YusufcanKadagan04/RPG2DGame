using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSystem : MonoBehaviour
{
    [Header("Ayarlar")]
    public GameObject bossPrefab;
    public Transform spawnPointParents;
    public GameObject battleGate;

    [Header("Durum Takibi")]
    public int currentWave = 0;
    public bool isWaveActive = false;

    private List<Transform> spawnPoints = new List<Transform>();
    public List<GameObject> activeEnemies = new List<GameObject>();
    public List<GameObject> normalEnemyPrefabs = new List<GameObject>();

    private int totalWaves = 4;

    void Start()
    {
        foreach (Transform child in spawnPointParents)
        {
            spawnPoints.Add(child);
        }
        if (battleGate != null)
        {
            battleGate.SetActive(false);
        }
    }

    public void StartBattle()
    {
        if (isWaveActive == true) return;

        isWaveActive = true;
        currentWave = 1;

        if (battleGate != null)
        {
            battleGate.SetActive(true);
        }

        Debug.Log("Savaş Başlladı! Kapılar Kapandı!!!!");
        StartCoroutine(WaveLogic());
    }

    void SpawnSingleEnemy(GameObject enemyToSpawn)
    {
        int randomIndex = Random.Range(0, spawnPoints.Count);

        Transform selectedPoint = spawnPoints[randomIndex];
        GameObject newEnemy = Instantiate(enemyToSpawn, selectedPoint.position, Quaternion.identity);
        activeEnemies.Add(newEnemy);
    }
    IEnumerator WaveLogic()
    {
        yield return new WaitForSeconds(1f);

        while (currentWave <= totalWaves)
        {
            if (currentWave == totalWaves)
            {
                Debug.Log("BOSS GELİYOR!");
                SpawnSingleEnemy(bossPrefab);
            }
            else
            {
                Debug.Log("Dalga: " + currentWave + " başladı.");

                int enemyCount = currentWave * 2;

                for (int i = 0; i < enemyCount; i++)
                {
                    int randomIndex = Random.Range(0, normalEnemyPrefabs.Count);
                    SpawnSingleEnemy(normalEnemyPrefabs[randomIndex]);
                    yield return new WaitForSeconds(0.5f);
                }
            }
            yield return new WaitUntil(() => activeEnemies.Count == 0);
            currentWave++;
        }
        Debug.Log("Tüm dalgalar bitti! Savaş Kazanıldı.");
        if (battleGate != null) battleGate.SetActive(false);
    }
    public void RemoveEnemyFromList(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        Debug.Log("Düşman öldü. Kalan: " + activeEnemies.Count);
    }
}
