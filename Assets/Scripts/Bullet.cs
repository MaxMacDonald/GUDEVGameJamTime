using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    private float disableTimer = -1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyShooter enemy = collision.gameObject.GetComponent<EnemyShooter>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ArenaBorder"))
        {
            GetComponent<Collider2D>().enabled = false;
            disableTimer = 5f;
        }
    }
    private void Update()
    {
        if (RewindManager.Instance.IsBeingRewinded) return;

        if (disableTimer > 0f)
        {
            disableTimer -= Time.deltaTime;
            if (disableTimer <= 0f)
            {
                GetComponent<Collider2D>().enabled = true;
                disableTimer = -1f;
                gameObject.SetActive(false);
            }
        }
    }
}