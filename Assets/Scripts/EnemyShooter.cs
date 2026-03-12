using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemyShooter : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    public float health = 30f;
    public float moveSpeed = 2f;
    public float fireRate = 1f;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 10f;

    private Transform player;
    private float fireCooldown;
    private bool insideArena = false;
    public bool isDead = false;


    // IEnemy / IDamageable properties
    public float Health { get => health; set => health = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public bool InsideArena { get => insideArena; set => insideArena = value; }

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null) return;

        MoveTowardsPlayer();

        if (!insideArena) return;

        fireCooldown -= Time.deltaTime;
        if (fireCooldown <= 0f)
        {
            Shoot();
            fireCooldown = 1f / fireRate;
        }
    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        RewindAbstract bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<RewindAbstract>();
        RewindManager.Instance.AddObjectForTracking(bullet, RewindManager.OutOfBoundsBehaviour.DisableDestroy);
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ArenaBorder"))
        {
            insideArena = true;
            EllipseBorder border = FindFirstObjectByType<EllipseBorder>();
            if (border != null)
                border.Ripple(transform.position, 5f, pushOutward: false);
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

        // Play hit feedbacks
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