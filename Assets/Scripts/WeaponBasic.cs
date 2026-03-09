using UnityEngine;

public class WeaponBasic : MonoBehaviour, IWeapon
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireForce = 20f;

    public void Fire()
    {
        GameObject bullet = Object.Instantiate(bulletPrefab, firePoint.position,firePoint.rotation);
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
    }
}
