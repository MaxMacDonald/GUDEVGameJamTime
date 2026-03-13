using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemyBossPhase1 : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    public float health = 300f;
    public float moveSpeed = 2f;
    public float preferredDistance = 5f;    // how far it tries to stay from player
    public float distanceThreshold = 1f;    // how much leeway before it adjusts distance

    [Header("Strafing")]
    public float strafeSpeed = 3f;
    public float strafeChangeInterval = 2f; // how often it changes strafe direction
    private float strafeTimer = 0f;
    private float strafeDirection = 1f;     // 1 = clockwise, -1 = counter-clockwise

    [Header("Spawning")]
    public GameObject[] enemyPrefabs;
    public int spawnCount = 3;
    public float spawnRate = 5f;
    public float spawnRadius = 2f;
    public float spawnCooldown;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint1;
    public Transform firePoint2;
    public float bulletForce = 10f;
    public float fireRate = 1f;
    private float fireCooldown = 0f;
    private bool useFirePoint1 = true;      // alternates between fire points

    private Transform player;
    private bool insideArena = false;
    public bool isDead = false;

    private float arenaTriggerCooldown = 0f;
    private const float ARENA_TRIGGER_COOLDOWN_DURATION = 0.3f;

    // Arena constraints
    private float arenaRadiusX;
    private float arenaRadiusY;

    public float Health { get => health; set => health = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public bool InsideArena { get => insideArena; set => insideArena = value; }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        spawnCooldown = spawnRate;
        strafeTimer = strafeChangeInterval;

        // Snapshot arena size for constraint
        if (EllipseBorder.Instance != null)
        {
            arenaRadiusX = EllipseBorder.Instance.radiusX;
            arenaRadiusY = EllipseBorder.Instance.radiusY;
        }
    }

    void Update()
    {
        if (RewindManager.Instance.IsBeingRewinded) return;
        if (player == null) return;

        if (arenaTriggerCooldown > 0f)
            arenaTriggerCooldown -= Time.deltaTime;

        if (!insideArena)
        {
            MoveTowardsPlayer();
            return;
        }

        HandleMovement();
        HandleShooting();
        HandleSpawning();
    }

    void HandleMovement()
    {
        Vector2 toPlayer = (player.position - transform.position);
        float distanceToPlayer = toPlayer.magnitude;
        Vector2 directionToPlayer = toPlayer.normalized;

        // Perpendicular direction for strafing
        Vector2 strafeDir = new Vector2(-directionToPlayer.y, directionToPlayer.x) * strafeDirection;

        // Change strafe direction periodically
        strafeTimer -= Time.deltaTime;
        if (strafeTimer <= 0f)
        {
            strafeDirection *= -1f;
            strafeTimer = strafeChangeInterval + Random.Range(-0.5f, 0.5f); // slight randomness
        }

        // Combine strafe with distance keeping
        Vector2 moveDir = strafeDir;
        if (distanceToPlayer < preferredDistance - distanceThreshold)
        {
            // Too close - move away
            moveDir -= directionToPlayer;
        }
        else if (distanceToPlayer > preferredDistance + distanceThreshold)
        {
            // Too far - move closer
            moveDir += directionToPlayer;
        }

        Vector2 newPosition = (Vector2)transform.position + moveDir.normalized * moveSpeed * Time.deltaTime;

        // Constrain to arena
        newPosition = ConstrainToArena(newPosition);
        transform.position = newPosition;

        // Always face the player
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    Vector2 ConstrainToArena(Vector2 pos)
    {
        float a = arenaRadiusX * 0.85f; // stay well inside the border
        float b = arenaRadiusY * 0.85f;

        float ellipseValue = (pos.x * pos.x) / (a * a) + (pos.y * pos.y) / (b * b);

        if (ellipseValue > 1f)
        {
            float angle = Mathf.Atan2(pos.y / b, pos.x / a);
            pos = new Vector2(
                a * Mathf.Cos(angle),
                b * Mathf.Sin(angle)
            );
        }

        return pos;
    }

    void HandleShooting()
    {
        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    void Shoot()
    {
        // Alternate between fire points
        Transform currentFirePoint = useFirePoint1 ? firePoint1 : firePoint2;
        useFirePoint1 = !useFirePoint1;

        // Aim towards player
        Vector2 directionToPlayer = ((Vector2)player.position - (Vector2)currentFirePoint.position).normalized;
        float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg - 90f;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        RewindAbstract bullet = Instantiate(bulletPrefab, currentFirePoint.position, rotation).GetComponent<RewindAbstract>();
        RewindManager.Instance.AddObjectForTracking(bullet, RewindManager.OutOfBoundsBehaviour.DisableDestroy);
        bullet.GetComponent<Rigidbody2D>().AddForce(rotation * Vector2.up * bulletForce, ForceMode2D.Impulse);
    }

    void HandleSpawning()
    {
        spawnCooldown -= Time.deltaTime;
        if (spawnCooldown <= 0f)
        {
            SpawnEnemies();
            spawnCooldown = spawnRate;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = ((Vector2)player.position - (Vector2)transform.position).normalized;
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
            GameObject prefabToSpawn = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

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
            if (arenaTriggerCooldown > 0f) return;
            arenaTriggerCooldown = ARENA_TRIGGER_COOLDOWN_DURATION;

            insideArena = true;
            EllipseBorder.Instance?.Ripple(transform.position, 10f, pushOutward: false);
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
        {
            if (arenaTriggerCooldown > 0f) return;
            arenaTriggerCooldown = ARENA_TRIGGER_COOLDOWN_DURATION;
            insideArena = false;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        var getHitFeedback = GetComponent<MMF_Player>();
        getHitFeedback?.PlayFeedbacks();

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