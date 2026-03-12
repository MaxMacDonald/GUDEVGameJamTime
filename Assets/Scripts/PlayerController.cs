using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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

    [Header("Death Settings")]
    public float slowMotionScale = 0.2f;        // how slow time gets on hit
    public float rewindWindowDuration = 2f;     // real-time seconds player has to rewind
    public Animator playerAnimator;             // leave space for death animation

    Vector2 moveDirection;
    Vector2 mousePosition;
    bool isRewinding = false;
    float rewindValue = 0f;
    bool isInDeathWindow = false;
    bool isDead = false;

   
    public EllipseBorder ellipseBorder;

    private void Start()
    {
        weapon = weaponObject as IWeapon;
        rewindResource = GameManager.Instance.GetComponent<RewindResource>();
        survivalTimer = SurvivalTimer.Instance.GetComponent<SurvivalTimer>();
        enemySpawner = EnemySpawner.Instance.GetComponent<EnemySpawner>();
        ellipseBorder = EllipseBorder.Instance.GetComponent<EllipseBorder>();
    }

    void Update()
    {
        if (isDead) return;

        Vector2 input = new Vector2(
            Keyboard.current.dKey.isPressed ? 1 : Keyboard.current.aKey.isPressed ? -1 : 0,
            Keyboard.current.wKey.isPressed ? 1 : Keyboard.current.sKey.isPressed ? -1 : 0
        );
        moveDirection = input.normalized;
        mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        if (Mouse.current.leftButton.isPressed && !isRewinding && !isInDeathWindow)
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

            // If player rewinds during death window, cancel death
            if (isInDeathWindow)
            {
                CancelDeath();
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

                // If they stop rewinding during death window, restore normal speed
                if (isInDeathWindow)
                    Time.timeScale = slowMotionScale;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead || isInDeathWindow) return;

        if (rewindResource.CanRewind())
        {
            StartCoroutine(DeathWindow());
        }
        else
        {
            // No rewind resource left - die immediately
            Die();
        }
    }

    IEnumerator DeathWindow()
    {
        isInDeathWindow = true;

        // Slow motion
        Time.timeScale = slowMotionScale;

        // TODO: Play hit animation here
        // if (playerAnimator != null) playerAnimator.SetTrigger("Hit");

        // Wait for rewind window in real time (unscaled)
        float elapsed = 0f;
        while (elapsed < rewindWindowDuration)
        {
            // If they started rewinding, keep waiting until they stop
            if (isRewinding)
            {
                elapsed = 0f; // reset timer while rewinding
            }

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Window expired and they didn't rewind far enough - die
        if (isInDeathWindow)
        {
            Die();
        }
    }

    void CancelDeath()
    {
        isInDeathWindow = false;
        Time.timeScale = 1f;
    }

    void Die()
    {
        isDead = true;
        isInDeathWindow = false;
        Time.timeScale = 1f;

        survivalTimer.StopTimer();
        enemySpawner.StopSpawning();

        // TODO: Play death animation here
        // if (playerAnimator != null) playerAnimator.SetTrigger("Die");

        gameObject.SetActive(false);
    }

    void ConstrainToArena()
    {
        Vector2 pos = rb.position;

        float a = ellipseBorder.radiusX;
        float b = ellipseBorder.radiusY;

        float ellipseValue = (pos.x * pos.x) / (a * a) + (pos.y * pos.y) / (b * b);

        if (ellipseValue > 1f)
        {
            // Project back onto ellipse edge
            float angle = Mathf.Atan2(pos.y / b, pos.x / a);
            Vector2 closestPoint = new Vector2(
                a * Mathf.Cos(angle),
                b * Mathf.Sin(angle)
            );

            // Calculate inward normal
            Vector2 inwardNormal = -new Vector2(
                Mathf.Cos(angle) / a,
                Mathf.Sin(angle) / b
            ).normalized;

            // Bounce velocity off the border
            rb.position = closestPoint;
            rb.linearVelocity = Vector2.Reflect(rb.linearVelocity, inwardNormal) * 0.5f;
        }
    }

    private void FixedUpdate()
    {
        if (isRewinding || isDead) return;
        rb.linearVelocity = new Vector2(moveDirection.x * moveSpeed, moveDirection.y * moveSpeed);
        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;
        ConstrainToArena();
    }
}