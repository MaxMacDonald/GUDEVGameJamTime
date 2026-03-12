using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemySeeker : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    public float health = 30f;
    public float moveSpeed = 2f;
    public float fireRate = 1f;

 

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

    }

    void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ArenaBorder"))
        {
            insideArena = true;
            EllipseBorder border = FindFirstObjectByType<EllipseBorder>();
            if (border != null)
                border.Ripple(transform.position, 2f, pushOutward: false);
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