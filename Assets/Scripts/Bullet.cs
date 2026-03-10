using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyShooter enemy = collision.gameObject.GetComponent<EnemyShooter>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ArenaBorder"))
        {
            GetComponent<Collider2D>().enabled = false;
            Destroy(gameObject, 5f);
        }
    }
}