using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5.0f;
    public Rigidbody2D rb;

    [Header("Weapon")]
    [SerializeField] private MonoBehaviour weaponObject;
    public IWeapon weapon;

    [Header("Rewind")]
    [SerializeField] float rewindIntensity = 0.02f;

    [Header("Rewind Resource")]
    public RewindResource rewindResource;

    [Header("References")]
    public SurvivalTimer survivalTimer;
    public EnemySpawner enemySpawner;

    Vector2 moveDirection;
    Vector2 mousePosition;
    bool isRewinding = false;
    float rewindValue = 0f;

    private void Start()
    {
        weapon = weaponObject as IWeapon;
        rewindResource = GameManager.Instance.GetComponent<RewindResource>();
        survivalTimer = SurvivalTimer.Instance.GetComponent<SurvivalTimer>();
        enemySpawner = EnemySpawner.Instance.GetComponent<EnemySpawner>();
    }

    void Update()
    {
        Vector2 input = new Vector2(
            Keyboard.current.dKey.isPressed ? 1 : Keyboard.current.aKey.isPressed ? -1 : 0,
            Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0
        );
        moveDirection = input.normalized;
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.isPressed && !isRewinding)
        {
            weapon.Fire();
        }

        HandleRewind();
    }

    void HandleRewind()
    {
        if (Keyboard.current.leftShiftKey.isPressed && rewindResource.CanRewind())
        {
            rewindValue += rewindIntensity;
            rewindResource.ConsumeRewind(rewindIntensity);

            if (!isRewinding)
            {
                RewindManager.Instance.StartRewindTimeBySeconds(rewindValue);
                isRewinding = true;
                SFXManager.Instance.PlayRewindFeedbacks();
            }
            else
            {
                if (RewindManager.Instance.HowManySecondsAvailableForRewind > rewindValue)
                    RewindManager.Instance.SetTimeSecondsInRewind(rewindValue);
            }
        }
        else
        {
            if (isRewinding)
            {
                RewindManager.Instance.StopRewindTimeBySeconds();
                rewindValue = 0f;
                isRewinding = false;
                SFXManager.Instance.StopRewindFeedbacks();
            }
        }
    }

    private void FixedUpdate()
    {
        if (isRewinding) return;

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