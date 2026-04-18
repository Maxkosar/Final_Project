using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [Header("Items que pueden aparecer")]
    [SerializeField] private List<GameObject> itemPrefabs;

    [Header("Puntos de aparición")]
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Timing")]
    [SerializeField] private float minSpawnTime = 5f;
    [SerializeField] private float maxSpawnTime = 15f;

    [Header("Límite de items en escena")]
    [SerializeField] private int maxItemsAtOnce = 3;

    private List<GameObject> activeItems = new List<GameObject>();
    private bool isSpawning = true;

    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (!isSpawning) break;

            activeItems.RemoveAll(item => item == null);

            if (activeItems.Count >= maxItemsAtOnce) continue;

            SpawnRandomItem();
        }
    }

    void SpawnRandomItem()
    {
        if (itemPrefabs.Count == 0 || spawnPoints.Count == 0)
        {
            Debug.LogWarning("ItemSpawner: faltan prefabs o puntos de spawn!");
            return;
        }

        GameObject randomItem  = itemPrefabs[Random.Range(0, itemPrefabs.Count)];
        Transform  randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject spawned = Instantiate(randomItem, randomPoint.position, Quaternion.identity);
        activeItems.Add(spawned);

        Debug.Log($"Spawneó: {randomItem.name} en {randomPoint.name}");
    }

    // Llamado por ArenaController al terminar la partida
    public void StopSpawning()
    {
        isSpawning = false;

        // Destruimos los items que quedaron en escena
        foreach (var item in activeItems)
            if (item != null) Destroy(item);

        activeItems.Clear();
    }
}