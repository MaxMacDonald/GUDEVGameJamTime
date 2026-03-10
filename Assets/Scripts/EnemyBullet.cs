using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float damage = 10f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamage(damage);
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
