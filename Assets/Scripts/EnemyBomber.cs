using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemyBomber : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    public float health = 30f;
    public float moveSpeed = 2f;

    [Header("Bomb")]
    public GameObject bulletPrefab;
    public int bombSize = 6;
    public float bulletForce = 10f;

    private Transform player;
 
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

    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Explode()
    {
        float angleStep = 360f / bombSize;
        float randomOffset = Random.Range(0f, 360f);

        for (int i = 0; i < bombSize; i++)
        {
            float angle = i * angleStep + randomOffset;
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector2 direction = rotation * Vector2.up;

            RewindAbstract bullet = Instantiate(bulletPrefab, transform.position, rotation).GetComponent<RewindAbstract>();
            RewindManager.Instance.AddObjectForTracking(bullet, RewindManager.OutOfBoundsBehaviour.DisableDestroy);
            bullet.GetComponent<Rigidbody2D>().AddForce(direction * bulletForce, ForceMode2D.Impulse);
        }
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
        Explode();
        VFXManager.Instance.PlayEnemyDeathVFX(transform.position);
        gameObject.SetActive(false);

    }
}