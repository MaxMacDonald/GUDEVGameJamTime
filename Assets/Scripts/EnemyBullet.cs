using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 10f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }

}
