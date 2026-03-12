using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemyHex : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    public float health = 300f;
    public float moveSpeed = 2f;

    [Header("Spawning")]
    public GameObject[] enemyPrefabs; // drag in different enemy prefabs to choose from
    public int spawnCount = 3;
    public float spawnRate = 5f; // how often it spawns in seconds
    public float spawnRadius = 2f; // how far from the hex enemies spawn

    public float spawnCooldown;
    private Transform player;
    private bool insideArena = false;
    public bool isDead = false;

    public float Health { get => health; set => health = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public bool InsideArena { get => insideArena; set => insideArena = value; }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        spawnCooldown = spawnRate;
    }

    void Update()
    {
        if (RewindManager.Instance.IsBeingRewinded) return;
        if (player == null) return;

        MoveTowardsPlayer();

        if (!insideArena) return;

        spawnCooldown -= Time.deltaTime;
        if (spawnCooldown <= 0f)
        {
            SpawnEnemies();
            spawnCooldown = spawnRate;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void SpawnEnemies()
    {
        float angleStep = 360f / spawnCount;
        float randomOffset = Random.Range(0f, 360f);

        for (int i = 0; i < spawnCount; i++)
        {
            // Pick a random enemy prefab from the array
            GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Spread spawns evenly in a circle around the hex
            float angle = (i * angleStep + randomOffset) * Mathf.Deg2Rad;
            Vector2 spawnPosition = (Vector2)transform.position + new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            ) * spawnRadius;

            RewindAbstract spawnedEnemy = Instantiate(
                prefabToSpawn,
                spawnPosition,
                Quaternion.identity
            ).GetComponent<RewindAbstract>();

            RewindManager.Instance.AddObjectForTracking(spawnedEnemy, RewindManager.OutOfBoundsBehaviour.Disable);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ArenaBorder"))
        {
            insideArena = true;
            return;
        }
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(10f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ArenaBorder"))
            insideArena = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0f)
        {
            isDead = true;
            Die();
        }
    }

    public void Die()
    {
        VFXManager.Instance.PlayEnemyDeathVFX(transform.position);
        gameObject.SetActive(false);
    }
}