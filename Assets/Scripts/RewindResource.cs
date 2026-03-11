using UnityEngine;
using UnityEngine.UI;

public class RewindResource : MonoBehaviour
{
    [Header("Settings")]
    public float maxRewindSeconds = 5f;
    public float rechargeRate = 0.3f; // seconds recharged per real second
    public float rechargeDelay = 0.5f;

    public float currentRewind;
    private float rechargeDelayTimer = 0f;
    [SerializeField] public Image barFill;

    public void Initialise(Image fill)
    {
        barFill = fill;
        currentRewind = maxRewindSeconds;
    }

    void Update()
    {
        if (RewindManager.Instance.IsBeingRewinded) return;

        // Recharge over time
        if (currentRewind < maxRewindSeconds)
        {
            if (rechargeDelayTimer > 0f)
            {
                rechargeDelayTimer -= Time.deltaTime;
            }
            else
            {
                currentRewind += rechargeRate * Time.deltaTime;
                currentRewind = Mathf.Min(currentRewind, maxRewindSeconds);
            }
        }

        UpdateBar();
    }

    public bool CanRewind()
    {
        return currentRewind > 0f;
    }

    public void ConsumeRewind(float amount)
    {
        currentRewind -= amount;
        currentRewind = Mathf.Max(currentRewind, 0f);
        rechargeDelayTimer = rechargeDelay;
        UpdateBar();
    }

    void UpdateBar()
    {
        if (barFill != null)
            barFill.fillAmount = currentRewind / maxRewindSeconds;
    }
}