using UnityEngine;

public class EnemySeeker : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    public float health = 20f;
    public float moveSpeed = 3f;
    public bool isDead = false;
    public bool insideArena = false;

    public float Health { get => health; set => health = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public bool InsideArena { get => insideArena; set => insideArena = value; }

    private Transform player;
    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (player == null) return;

        MoveTowardsPlayer();

    }

    void FixedUpdate()
    {
   
        if (player == null) return;

        
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
        gameObject.SetActive(false);
    }
}