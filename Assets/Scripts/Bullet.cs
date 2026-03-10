using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);

        EnemyShooter enemy = collision.gameObject.GetComponent<EnemyShooter>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }

}
