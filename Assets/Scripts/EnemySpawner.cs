using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [System.Serializable]
    public struct SpawnEvent
    {
        public float time;
        public GameObject enemyPrefab;
        public int count;
        public float swarmRadius; // 0 = spread out, > 0 = spawn in a cluster
    }

    public List<SpawnEvent> spawnEvents;
    private Camera mainCamera;
    public int nextEventIndex = 0;
    public float elapsedTime = 0f;
    private bool running = true;
    public float spawnMargin = 1f;

    private List<Vector2[]> cachedSpawnPositions = new List<Vector2[]>();
    private void Awake()
    {
        Instance = this;
    }



    void Start()
    {
        mainCamera = Camera.main;
        spawnEvents.Sort((a, b) => a.time.CompareTo(b.time));

        foreach (SpawnEvent spawnEvent in spawnEvents)
        {
            Vector2[] positions = new Vector2[spawnEvent.count];

            if (spawnEvent.swarmRadius > 0f)
            {
                // All enemies spawn near a single point
                Vector2 swarmCenter = GenerateSpawnPosition();
                for (int i = 0; i < spawnEvent.count; i++)
                {
                    positions[i] = swarmCenter + Random.insideUnitCircle * spawnEvent.swarmRadius;
                }
            }
            else
            {
                // Each enemy spawns at its own random position
                for (int i = 0; i < spawnEvent.count; i++)
                {
                    positions[i] = GenerateSpawnPosition();
                }
            }

            cachedSpawnPositions.Add(positions);
        }
    }

    void Update()
    {
        if (RewindManager.Instance.IsBeingRewinded) return;
        if (!running) return;

        elapsedTime += Time.deltaTime;

        while (nextEventIndex < spawnEvents.Count &&
               elapsedTime >= spawnEvents[nextEventIndex].time)
        {
            SpawnEvent spawnEvent = spawnEvents[nextEventIndex];
            Vector2[] positions = cachedSpawnPositions[nextEventIndex];

            for (int i = 0; i < spawnEvent.count; i++)
            {
                RewindAbstract someObjectToSpawn = Instantiate(
                    spawnEvent.enemyPrefab,
                    positions[i],
                    Quaternion.identity
                ).GetComponent<RewindAbstract>();
                RewindManager.Instance.AddObjectForTracking(someObjectToSpawn, RewindManager.OutOfBoundsBehaviour.Disable);
            }
            nextEventIndex++;
        }
    }

    Vector2 GenerateSpawnPosition()
    {
        float camHeight = mainCamera.orthographicSize + spawnMargin;
        float camWidth = mainCamera.orthographicSize * mainCamera.aspect + spawnMargin;

        int side = Random.Range(0, 4);
        return side switch
        {
            0 => new Vector2(Random.Range(-camWidth, camWidth), camHeight),
            1 => new Vector2(Random.Range(-camWidth, camWidth), -camHeight),
            2 => new Vector2(-camWidth, Random.Range(-camHeight, camHeight)),
            _ => new Vector2(camWidth, Random.Range(-camHeight, camHeight)),
        };
    }

    public void StopSpawning()
    {
        running = false;
    }
}