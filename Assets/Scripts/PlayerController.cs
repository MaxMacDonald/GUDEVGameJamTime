using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public Rigidbody2D rb;
    [SerializeField] private MonoBehaviour weaponObject;
    public IWeapon weapon;
    Vector2 moveDirection;
    Vector2 mousePosition;

    public SurvivalTimer survivalTimer;
    public EnemySpawner enemySpawner;


    private void Start()
    {
        weapon = weaponObject as IWeapon;
    }

    void Update()
    {
        Vector2 input = new Vector2(
            Keyboard.current.dKey.isPressed ? 1 : Keyboard.current.aKey.isPressed ? -1 : 0,
            Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0
        );
        moveDirection = input.normalized;

        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Continuous fire while held
        if (Mouse.current.leftButton.isPressed)
        {
            weapon.Fire();
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;
    }


    public void TakeDamage(float damage)
    {
        survivalTimer.StopTimer();
        enemySpawner.StopSpawning();
        // Add death effects here later if needed
        Destroy(gameObject);
    }
}