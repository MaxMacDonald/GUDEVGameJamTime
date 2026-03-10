using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public struct SpawnEvent
    {
        public float time;
        public GameObject enemyPrefab;
        public int count;
    }

    public List<SpawnEvent> spawnEvents;

    private Camera mainCamera;
    private SurvivalTimer survivalTimer;
    private int nextEventIndex = 0;
    private float elapsedTime = 0f;
    private bool running = true;

    public float spawnMargin = 1f; // How far outside the camera to spawn

    void Start()
    {
        mainCamera = Camera.main;
        // Sort events by time so you can add them in any order in the Inspector
        spawnEvents.Sort((a, b) => a.time.CompareTo(b.time));
    }

    void Update()
    {
        if (!running) return;

        elapsedTime += Time.deltaTime;

        while (nextEventIndex < spawnEvents.Count &&
               elapsedTime >= spawnEvents[nextEventIndex].time)
        {
            SpawnEvent spawnEvent = spawnEvents[nextEventIndex];
            for (int i = 0; i < spawnEvent.count; i++)
            {
                Instantiate(spawnEvent.enemyPrefab, GetSpawnPosition(), Quaternion.identity);
            }
            nextEventIndex++;
        }
    }

    Vector2 GetSpawnPosition()
    {
        float camHeight = mainCamera.orthographicSize + spawnMargin;
        float camWidth = mainCamera.orthographicSize * mainCamera.aspect + spawnMargin;

        // Pick a random side: 0=top, 1=bottom, 2=left, 3=right
        int side = Random.Range(0, 4);
        return side switch
        {
            0 => new Vector2(Random.Range(-camWidth, camWidth), camHeight),   // top
            1 => new Vector2(Random.Range(-camWidth, camWidth), -camHeight),  // bottom
            2 => new Vector2(-camWidth, Random.Range(-camHeight, camHeight)), // left
            _ => new Vector2(camWidth, Random.Range(-camHeight, camHeight)),  // right
        };
    }

    public void StopSpawning()
    {
        running = false;
    }
}