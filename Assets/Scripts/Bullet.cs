using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 10f;
    private float disableTimer = -1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IEnemy enemy = collision.gameObject.GetComponent<IEnemy>();
            if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
        gameObject.SetActive(false);
        VFXManager.Instance.PlayHitVFX(transform.position,transform.rotation);


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("ArenaBorder"))
        {
            // Play a hit wall VFX
            VFXManager.Instance.PlayHitWallVFX(transform.position);
            // Cause a ripple on the border
            EllipseBorder border = FindFirstObjectByType<EllipseBorder>();
            if (border != null)
                border.Ripple(transform.position, 5f);

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