using UnityEngine;

public class EnemyShooter : MonoBehaviour
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
            insideArena = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ArenaBorder"))
            insideArena = false;
    }

    public void TakeDamage(float damage)
    {
        //if (isDead) return;

        health -= damage;
        if (health <= 0f)
        {
            isDead = true;
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
    }
}