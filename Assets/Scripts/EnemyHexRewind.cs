using UnityEngine;

public class EnemyHexRewind : RewindAbstract
{
    private EnemyHex enemyHex;
    private CircularBuffer<float> trackedHealth;
    private CircularBuffer<float> trackedIsDead;
    private CircularBuffer<float> trackedCooldown;

    private void Awake()
    {
        enemyHex = GetComponent<EnemyHex>();
        if (enemyHex == null)
            Debug.LogError($"EnemyHexRewind on {gameObject.name} couldn't find EnemyHex!");
    }

    private void Start()
    {
        trackedHealth = new CircularBuffer<float>();
        trackedIsDead = new CircularBuffer<float>();
        trackedCooldown = new CircularBuffer<float>();
    }

    public override void Track()
    {
        if (enemyHex == null) return;
        trackedHealth.WriteLastValue(enemyHex.health);
        trackedIsDead.WriteLastValue(enemyHex.isDead ? 1f : 0f);
        trackedCooldown.WriteLastValue(enemyHex.spawnCooldown);
    }

    public override void Rewind(float seconds)
    {
        if (enemyHex == null) return;
        enemyHex.health = trackedHealth.ReadFromBuffer(seconds);
        enemyHex.isDead = trackedIsDead.ReadFromBuffer(seconds) > 0.5f;
        enemyHex.spawnCooldown = trackedCooldown.ReadFromBuffer(seconds);
    }
}