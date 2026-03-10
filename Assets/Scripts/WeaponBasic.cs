using UnityEngine;

public class WeaponBasic : MonoBehaviour, IWeapon
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletForce = 20f;

    [Header("Fire Settings")]
    public float fireRate = 0.05f;
    public int bulletsPerShot = 1;
    public float spreadAngle = 15f;

    private float fireCooldown;

    public void Fire()
    {
        if (fireCooldown > 0f) return;

        for (int i = 0; i < bulletsPerShot; i++)
        {
            float angle = GetSpreadAngle(i);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, angle) * firePoint.rotation;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, spreadRotation);
            bullet.GetComponent<Rigidbody2D>().AddForce(spreadRotation * Vector2.up * bulletForce, ForceMode2D.Impulse);
        }

        fireCooldown = fireRate;
    }

    void Update()
    {
        if (fireCooldown > 0f)
            fireCooldown -= Time.deltaTime;
    }

    float GetSpreadAngle(int bulletIndex)
    {
        // Box-Muller transform for normal distribution
        float u1 = 1f - Random.value;
        float u2 = 1f - Random.value;
        float normalRandom = Mathf.Sqrt(-2f * Mathf.Log(u1)) * Mathf.Sin(2f * Mathf.PI * u2);

        // normalRandom is mostly between -1 and 1, scale by half spread
        return normalRandom * (spreadAngle / 2f);
    }
}